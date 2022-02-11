using System;
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

  public static class Configuration {
    private class ServiceRegistration {
      public IService Service { get; private set; }
      public int Priority { get; private set; }

      public ServiceRegistration(IService service, int priority) {
        this.Service = service;
        this.Priority = priority;
      }
    }

    public static string ConfigurationDirectory { get; set; }

    public static string ExtensionsDirectory { get; set; }

    public static string ApplicationDirectory => AppDomain.CurrentDomain.BaseDirectory;

    public static CompositionContainer Container { get; private set; }

    private static readonly List<ServiceRegistration> serviceRegistrations = new List<ServiceRegistration>();

    private static readonly List<string> containerErrors = new List<string>();
    public static IReadOnlyList<string> ContainerErrors => Configuration.containerErrors;

    private static List<Tuple<ConfigurationStage, Action>> compositionActions = new List<Tuple<ConfigurationStage, Action>>();

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

    public static void ConfigureContainer(params string[] assemblyNames) {
      // aggregate all catalogs
      var catalog = new AggregateCatalog();
      foreach(var filePath in Directory.GetFiles(Configuration.ExtensionsDirectory, "*.dll", SearchOption.TopDirectoryOnly)) {
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
      var duplicateServicePartDefinitions = Configuration.GetDuplicateServicePartDefinitions(catalog);
      var filteredCatalog = new FilteredCatalog(catalog, definition => !duplicateServicePartDefinitions.Contains(definition));
      Configuration.Container = new CompositionContainer(filteredCatalog);
    }

    public static void RegisterService(IService service, int priority = 0) {
      if(Configuration.serviceRegistrations.Any(r => r.Service == service)) {
        throw new Exception($"the service {service.GetType().Name} is already registered in the HAF configuration");
      }
      Configuration.serviceRegistrations.Add(new ServiceRegistration(service, priority));
    }

    public static void StageAction(ConfigurationStage stage, Action action) {
      if(stage <= Configuration.Stage) {
        // execute directly as stage is current or passed
        action();
      }
      // defer action
      compositionActions.Add(new Tuple<ConfigurationStage, Action>(stage, action));
    }

    public static void EnterStage(ConfigurationStage stage) {
      if(stage <= Configuration.Stage) {
        throw new InvalidOperationException("the new configuration stage must be further along then the current stage");
      }
      Configuration.Stage = stage;
      foreach(var action in compositionActions.Where(a => a.Item1 == Configuration.Stage)) {
        action.Item2();
      }
      if(Configuration.Stage == ConfigurationStage.Composition) {
        // create directories if needed
        if(!Directory.Exists(Configuration.ConfigurationDirectory)) {
          Directory.CreateDirectory(Configuration.ConfigurationDirectory);
        } else {
          // perform requested deletions
          foreach(var deleteFilePath in Directory.GetFiles(Configuration.ConfigurationDirectory, "*.delete", SearchOption.AllDirectories)) {
            var filePath = deleteFilePath.Substring(0, deleteFilePath.Length - 7);
            if(File.Exists(filePath)) {
              File.Delete(filePath);
            }
            File.Delete(deleteFilePath);
          }
          // perform requested replacements
          foreach(var replaceFilePath in Directory.GetFiles(Configuration.ConfigurationDirectory, "*.replace", SearchOption.AllDirectories)) {
            var filePath = replaceFilePath.Substring(0, replaceFilePath.Length - 8);
            if(File.Exists(filePath)) {
              File.Delete(filePath);
            }
            File.Move(replaceFilePath, filePath);
          }
        }
        if(!Directory.Exists(Configuration.ExtensionsDirectory)) {
          Directory.CreateDirectory(Configuration.ExtensionsDirectory);
        }
      } else if(Configuration.Stage == ConfigurationStage.Configuration) {
        // assign links for all linked objects
        LinkedObservableObjectManager.AssignLinks();
        // load service configurations
        var filePath = Path.Combine(Configuration.ConfigurationDirectory, "settings.xml");
        var configuration = File.Exists(filePath) ? ServiceConfiguration.FromFile(filePath) : new ServiceConfiguration("settings");
        foreach(var serviceRegistration in Configuration.serviceRegistrations.OrderBy(r => r.Priority)) {
          serviceRegistration.Service.LoadConfiguration(configuration);
        }
      } else if(Configuration.Stage == ConfigurationStage.WindowInitialization) {
        var logService = Configuration.Container.GetExportedValue<ILogService>();
        foreach(var containerError in Configuration.ContainerErrors) {
          logService.Error(containerError, "application");
        }
      } else if(Configuration.Stage == ConfigurationStage.Exiting) {
        // save service configurations
        var configuration = new ServiceConfiguration("settings");
        foreach(var serviceRegistration in Configuration.serviceRegistrations.OrderByDescending(r => r.Priority)) {
          serviceRegistration.Service.SaveConfiguration(configuration);
        }
        configuration.SaveToFile(Path.Combine(Configuration.ConfigurationDirectory, "settings.xml"));
      }
    }
  }
}
