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
  public static class Configuration {

    public static string ConfigurationDirectory;

    public static string ExtensionsDirectory;

    public static CompositionContainer Container { get; private set; }

    public static List<IService> ConfiguratedServices { get; private set; } = new List<IService>();

    public static void Initialize() {
      // create directories if needed
      if (!Directory.Exists(Configuration.ConfigurationDirectory)) {
        Directory.CreateDirectory(Configuration.ConfigurationDirectory);
      }
      if (!Directory.Exists(Configuration.ExtensionsDirectory)) {
        Directory.CreateDirectory(Configuration.ExtensionsDirectory);
      }
    }

    public static void ConfigureContainer(string[] designTimeAssemblyNames, Assembly applicationAssembly) {
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
      catalog.Catalogs.Add(new AssemblyCatalog(applicationAssembly));
      catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
      // filter out all duplicate service exports, only the first export of a service export type identity remains
      // note that design time services have highest priority, then extension services and lastly application services
      var serviceAwareCatalog = new ServiceAwareCatalog(catalog);
      Configuration.Container = new CompositionContainer(serviceAwareCatalog);
    }

    public static void Load() {
      var filePath = Path.Combine(Configuration.ConfigurationDirectory, "settings.xml");
      if (!File.Exists(filePath)) {
        Configuration.Save();
      }
      var configuration = ServiceConfiguration.FromFile(filePath);
      foreach (var service in Configuration.ConfiguratedServices) {
        service.LoadConfiguration(configuration);
      }
    }

    public static void Save() {
      var configuration = new ServiceConfiguration("settings");
      foreach (var service in Configuration.ConfiguratedServices) {
        service.SaveConfiguration(configuration);
      }
      configuration.SaveToFile(Path.Combine(Configuration.ConfigurationDirectory, "settings.xml"));
    }

  }
}
