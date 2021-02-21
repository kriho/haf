using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public static class Backend {

    public static CompositionContainer Container;

    public static string ConfigurationDirectory;

    public static string ExtensionsDirectory;

    public static List<IService> ConfiguratedServices = new List<IService>();

    public static void Initialize() {
      // create directories if needed
      if (!System.IO.Directory.Exists(Backend.ConfigurationDirectory)) {
        System.IO.Directory.CreateDirectory(Backend.ConfigurationDirectory);
      }
      if (!System.IO.Directory.Exists(Backend.ExtensionsDirectory)) {
        System.IO.Directory.CreateDirectory(Backend.ExtensionsDirectory);
      }
      Backend.LoadConfiguration();
    }

    public static void SaveConfiguration() {
      var storage = new Configuration("settings");
      foreach (var service in Backend.ConfiguratedServices) {
        service.SaveConfiguration(storage);
      }
      storage.SaveToFile(Path.Combine(Backend.ConfigurationDirectory, "settings.xml"));
    }

    public static void LoadConfiguration() {
      var filePath = Path.Combine(Backend.ConfigurationDirectory, "settings.xml");
      if (!File.Exists(filePath)) {
        Backend.SaveConfiguration();
      }
      var configuration = Configuration.FromFile(filePath);
      foreach (var service in Backend.ConfiguratedServices) {
        service.LoadConfiguration(configuration);
      }
    }
  }
}
