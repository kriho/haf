﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HAF {

  public enum ConfigurationStage {
    Startup,
    Composition,
    Configuration,
    WindowInitialization,
    Running,
    Exiting,
  }

  /// <summary>
  /// maintains the composition container and handles application start and exit routines
  /// </summary>
  public static class Core {
    private class ServiceRegistration {
      public IService Service { get; private set; }
      public int Priority { get; private set; }

      public ServiceRegistration(IService service, int priority) {
        this.Service = service;
        this.Priority = priority;
      }
    }

    /// <summary>
    /// Absolute path to the directory that contains all configuration files of the application.
    /// </summary>
    public static string ConfigurationDirectory { get; set; }

    /// <summary>
    /// Absolute path to the directory that contains all extension files of the application.
    /// </summary>
    public static string ExtensionsDirectory { get; set; }

    /// <summary>
    /// Absolute path to the application installation directory.
    /// </summary>
    public static string ApplicationDirectory => AppDomain.CurrentDomain.BaseDirectory;

    /// <summary>
    /// The composition container that contains all services and parts.
    /// </summary>
    public static CompositionContainer Container { get; private set; }

    private static readonly List<ServiceRegistration> serviceRegistrations = new List<ServiceRegistration>();

    private static readonly List<string> containerErrors = new List<string>();

    /// <summary>
    /// List of all errors that occurred during composition of the plugins.
    /// </summary>
    public static IReadOnlyList<string> ContainerErrors => Core.containerErrors;

    private static List<Tuple<ConfigurationStage, Action>> compositionActions = new List<Tuple<ConfigurationStage, Action>>();

    /// <summary>
    /// The current configuration stage of the application. It is advanced in <c>ConfigureContainer()</c> and <c>ShowWindow()</c> and can be used to schedule actions using <c>StageAction()</c>.
    /// </summary>
    public static ConfigurationStage Stage { get; private set; } = ConfigurationStage.Startup;

    private static List<ComposablePartDefinition> GetDuplicateServicePartDefinitions(ComposablePartCatalog catalog) {
      var serviceTypeIdentities = new List<object>();
      var result = new List<ComposablePartDefinition>();
      foreach(var partDefinition in catalog) {
        foreach(var exportDefinition in partDefinition.ExportDefinitions) {
          if(exportDefinition.ContractName.EndsWith("Service")) {
            if(exportDefinition.Metadata.TryGetValue("ExportTypeIdentity", out object typeIdentity)) {
              if(serviceTypeIdentities.Contains(typeIdentity)) {
                result.Add(partDefinition);
              } else {
                serviceTypeIdentities.Add(typeIdentity);
              }
            }
          }
        }
      }
      return result;
    }

    /// <summary>
    /// Enters configuration stage <c>Composition</c> and configurates composition the container by composing parts from all provided assemblies.<br/>
    /// Loads all available extension from the <c>ExtensionsDirectory</c> before loading any other extensions.<br/>
    /// After the provided assemblies are composed, the HAF assembly is composed and the configuration stage is advanced to <c>Configuration</c>.
    /// </summary>
    public static void ConfigureContainer(params string[] assemblyNames) {
      // enter composition stage
      HAF.Core.EnterStage(HAF.ConfigurationStage.Composition);
      // aggregate all catalogs
      var catalog = new AggregateCatalog();
      foreach(var filePath in Directory.GetFiles(Core.ExtensionsDirectory, "*.dll", SearchOption.TopDirectoryOnly)) {
        try {
          var pluginCatalog = new AssemblyCatalog(Assembly.LoadFile(filePath));
          var test = pluginCatalog.FirstOrDefault();
          catalog.Catalogs.Add(pluginCatalog);
        } catch(ReflectionTypeLoadException ex) {
          foreach(var loaderException in ex.LoaderExceptions) {
            containerErrors.Add($"failed to load plugin \"{Path.GetFileNameWithoutExtension(filePath)}\": {loaderException.Message}");
          }
          File.Move(filePath, filePath.Replace(".dll", ".dll.broken"));
        }
      }
      foreach(var assemblyName in assemblyNames) {
        catalog.Catalogs.Add(new AssemblyCatalog(Assembly.Load(assemblyName)));
      }
      catalog.Catalogs.Add(new AssemblyCatalog(Assembly.Load("HAF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")));
      // filter out all duplicate service exports, only the first export of a service export type identity remains
      // note that design time services have highest priority, then extension services and lastly application services
      var duplicateServicePartDefinitions = Core.GetDuplicateServicePartDefinitions(catalog);
      var filteredCatalog = new FilteredCatalog(catalog, definition => !duplicateServicePartDefinitions.Contains(definition));
      Core.Container = new CompositionContainer(filteredCatalog);
    }

    /// <summary>
    /// Enters configuration stage <c>WindowInitialization</c> and shows the provided window.<br/>
    /// The configuration stage is advanced to <c>Running</c> when the window fires the <c>SourceInitialized</c> event.
    /// </summary>
    public static void ShowWindow(Window window) {
      HAF.Core.EnterStage(HAF.ConfigurationStage.WindowInitialization);
      window.SourceInitialized += (s, e) => {
        HAF.Core.EnterStage(HAF.ConfigurationStage.Running);
      };
      window.Show();
    }

    /// <summary>
    /// Enters configuration stage <c>Configuration</c> and loads all configuration for registered services.<br/>
    /// </summary>
    public static void LoadServiceConfiguration() {
      // enter configuration state
      HAF.Core.EnterStage(HAF.ConfigurationStage.Configuration);
    }

    /// <summary>
    /// Enters configuration stage <c>Exiting</c>.
    /// </summary>
    public static void Exit() {
      HAF.Core.EnterStage(HAF.ConfigurationStage.Exiting);
    }

    /// <summary>
    /// Register a service to load its configuration from the <c>settings.xml</c> file in the <c>ConfigurationDirectory</c> when the configuration stage is advanced to <c>Configuration</c> and store its configuration when the configuration stage is set to <c>Exiting</c>.<br/>
    /// The service must override <c>LoadConfiguration()</c> and <c>SaveConfiguration()</c> to interact with the configuration.
    /// </summary>
    /// <param name="priority">Order in which configuration is loaded and stored. Lower priority means the service is loaded earlier and stored later.</param>
    /// <exception cref="InvalidOperationException">When the service was already registered.</exception>
    public static void RegisterService(IService service, int priority = 0) {
      if(Core.serviceRegistrations.Any(r => r.Service == service)) {
        throw new InvalidOperationException($"the service {service.GetType().Name} is already registered in the HAF configuration");
      }
      Core.serviceRegistrations.Add(new ServiceRegistration(service, priority));
    }

    /// <summary>
    /// Register an action to be executed when the configuration stage is advanced to the provided value. When the current configuration stage is equal or greater then the provided value, the action is executed directly.
    /// </summary>
    public static void StageAction(ConfigurationStage stage, Action action) {
      if(stage <= Core.Stage) {
        // execute directly as stage is current or passed
        action();
      }
      // defer action
      compositionActions.Add(new Tuple<ConfigurationStage, Action>(stage, action));
    }

    private static void EnterStage(ConfigurationStage stage) {
      if(stage <= Core.Stage) {
        throw new InvalidOperationException("the new configuration stage must be further along then the current stage");
      }
      Core.Stage = stage;
      foreach(var action in compositionActions.Where(a => a.Item1 == Core.Stage)) {
        action.Item2();
      }
      if(Core.Stage == ConfigurationStage.Composition) {
        // create directories if needed
        if(!Directory.Exists(Core.ConfigurationDirectory)) {
          Directory.CreateDirectory(Core.ConfigurationDirectory);
        } else {
          // perform requested deletions
          foreach(var deleteFilePath in Directory.GetFiles(Core.ConfigurationDirectory, "*.delete", SearchOption.AllDirectories)) {
            var filePath = deleteFilePath.Substring(0, deleteFilePath.Length - 7);
            if(File.Exists(filePath)) {
              File.Delete(filePath);
            }
            File.Delete(deleteFilePath);
          }
          // perform requested replacements
          foreach(var replaceFilePath in Directory.GetFiles(Core.ConfigurationDirectory, "*.replace", SearchOption.AllDirectories)) {
            var filePath = replaceFilePath.Substring(0, replaceFilePath.Length - 8);
            if(File.Exists(filePath)) {
              File.Delete(filePath);
            }
            File.Move(replaceFilePath, filePath);
          }
        }
        if(!Directory.Exists(Core.ExtensionsDirectory)) {
          Directory.CreateDirectory(Core.ExtensionsDirectory);
        }
      } else if(Core.Stage == ConfigurationStage.Configuration) {
        // assign links for all linked objects
        LinkedObservableObjectManager.AssignLinks();
        // load service configurations
        var filePath = Path.Combine(Core.ConfigurationDirectory, "settings.xml");
        var configuration = File.Exists(filePath) ? ServiceConfiguration.FromFile(filePath) : new ServiceConfiguration("settings");
        foreach(var serviceRegistration in Core.serviceRegistrations.OrderBy(r => r.Priority)) {
          serviceRegistration.Service.LoadConfiguration(configuration);
        }
      } else if(Core.Stage == ConfigurationStage.WindowInitialization) {
        var logService = Core.Container.GetExportedValue<ILogService>();
        foreach(var containerError in Core.ContainerErrors) {
          logService.Error(containerError, "application");
        }
      } else if(Core.Stage == ConfigurationStage.Exiting) {
        // save service configurations
        var configuration = new ServiceConfiguration("settings");
        foreach(var serviceRegistration in Core.serviceRegistrations.OrderByDescending(r => r.Priority)) {
          serviceRegistration.Service.SaveConfiguration(configuration);
        }
        configuration.SaveToFile(Path.Combine(Core.ConfigurationDirectory, "settings.xml"));
      }
    }
  }
}
