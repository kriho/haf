using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace HAF {
  public static partial class Utils {

    public class InstalledApplication {
      public string Name { get; set; }
      public string Directory { get; set; }
      public string Version { get; set; }
    }

    public static List<InstalledApplication> GetInstalledApplications(bool searchRegistry32 = true, bool searchRegistry64 = true) {
      var applications = new List<InstalledApplication>();
      void SearchInRootKey(RegistryKey rootKey) {
        using(var uninstallKey = rootKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", false)) {
          applications.AddRange(uninstallKey
               .GetSubKeyNames()
               .Select(n => uninstallKey.OpenSubKey(n, false))
               .Select(k => new InstalledApplication() {
                 Name = k.GetValue("DisplayName")?.ToString(),
                 Version = k.GetValue("DisplayVersion")?.ToString(),
                 Directory = k.GetValue("InstallLocation")?.ToString(),
               }));
        }
      }
      if(searchRegistry32) {
        using(var rootKey = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry32)) {
          SearchInRootKey(rootKey);
        }
      }
      if(searchRegistry64) {
        using(var rootKey = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64)) {
          SearchInRootKey(rootKey);
        }
      }
      return applications;
    }
  }
}
