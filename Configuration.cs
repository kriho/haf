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
  public static class Configuration {

    public class ServiceRegistration {
      public IService Service { get; private set;}
      public int Priority{ get; private set;}

      public ServiceRegistration(IService service, int priority) {
        this.Service = service;
        this.Priority = priority;
      }
    }

    public static string ConfigurationDirectory;

    public static string ExtensionsDirectory;

    public static CompositionContainer Container { get; private set; }

    private static readonly List<ServiceRegistration> serviceRegistrations = new List<ServiceRegistration>();

    public static void Initialize() {
      // create directories if needed
      if (!Directory.Exists(Configuration.ConfigurationDirectory)) {
        Directory.CreateDirectory(Configuration.ConfigurationDirectory);
      }
      if (!Directory.Exists(Configuration.ExtensionsDirectory)) {
        Directory.CreateDirectory(Configuration.ExtensionsDirectory);
      }
    }

    public static void ConfigureContainer(string userInterfaceLibrary, string applicationAssemblyName, params string[] designTimeAssemblyNames) {
      // aggregate all catalogs
      var catalog = new AggregateCatalog();
#if DEBUG
      if (ObservableObject.IsInDesignModeStatic) {
        foreach(var designTimeAssemblyName in designTimeAssemblyNames) {
          catalog.Catalogs.Add(new AssemblyCatalog(Assembly.Load(designTimeAssemblyName)));
        }
      }
#endif
      catalog.Catalogs.Add(new DirectoryCatalog(Configuration.ExtensionsDirectory));
      catalog.Catalogs.Add(new AssemblyCatalog(Assembly.Load(applicationAssemblyName)));
      catalog.Catalogs.Add(new AssemblyCatalog(Assembly.Load($"HAF.{userInterfaceLibrary}, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")));
      catalog.Catalogs.Add(new AssemblyCatalog(Assembly.Load("HAF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")));
      // filter out all duplicate service exports, only the first export of a service export type identity remains
      // note that design time services have highest priority, then extension services and lastly application services
      var serviceAwareCatalog = new ServiceAwareCatalog(catalog);
      Configuration.Container = new CompositionContainer(serviceAwareCatalog);
    }

    public static void RegisterService(IService service, int priority = 0) {
      if(Configuration.serviceRegistrations.Any(s => s.Service == service)) {
        throw new Exception($"the service {service.GetType().Name} is already registered in the HAF configuration");
      }
      Configuration.serviceRegistrations.Add(new ServiceRegistration(service, priority));
    }

    public static void LoadServiceConfigurations(int priority = 0) {
      // assign links for all linked objects
      LinkedObjectManager.AssignLinks();
      // load configuration
      var filePath = Path.Combine(Configuration.ConfigurationDirectory, "settings.xml");
      var configuration = File.Exists(filePath) ? ServiceConfiguration.FromFile(filePath) : new ServiceConfiguration("settings");
      foreach (var serviceRegistration in Configuration.serviceRegistrations.Where(s => s.Priority == priority)) {
        serviceRegistration.Service.LoadConfiguration(configuration);
      }
    }

    public static void SaveServiceConfigurations() {
      var configuration = new ServiceConfiguration("settings");
      foreach (var serviceRegistration in Configuration.serviceRegistrations) {
        serviceRegistration.Service.SaveConfiguration(configuration);
      }
      configuration.SaveToFile(Path.Combine(Configuration.ConfigurationDirectory, "settings.xml"));
    }

  }
}
