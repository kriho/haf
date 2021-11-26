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

    private static List<Tuple<ConfigurationStage,Action>> compositionActions = new List<Tuple<ConfigurationStage, Action>>();

    public static ConfigurationStage Stage { get; private set; } = ConfigurationStage.Startup;

    static Configuration() {
      if(ObservableObject.IsInDesignModeStatic) {
        var catalog = new AggregateCatalog();
        catalog.Catalogs.Add(new AssemblyCatalog(Assembly.Load("HAF.DesignTime, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")));
        catalog.Catalogs.Add(new AssemblyCatalog(Assembly.Load("HAF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")));
        var serviceAwareCatalog = new ServiceAwareCatalog(catalog);
        Configuration.Container = new CompositionContainer(serviceAwareCatalog);

      }
    }

    public static void ConfigureContainer(params string[] assemblyNames) {
      // aggregate all catalogs
      var catalog = new AggregateCatalog();
      catalog.Catalogs.Add(new DirectoryCatalog(Configuration.ExtensionsDirectory));
      foreach(var assemblyName in assemblyNames) {
        catalog.Catalogs.Add(new AssemblyCatalog(Assembly.Load(assemblyName)));
      }
      catalog.Catalogs.Add(new AssemblyCatalog(Assembly.Load("HAF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")));
      // filter out all duplicate service exports, only the first export of a service export type identity remains
      // note that design time services have highest priority, then extension services and lastly application services
      var serviceAwareCatalog = new ServiceAwareCatalog(catalog);
      Configuration.Container = new CompositionContainer(serviceAwareCatalog);
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
