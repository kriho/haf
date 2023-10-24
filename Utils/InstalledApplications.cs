using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

#if NET48

namespace HAF {
  public static partial class Utils {

    public class InstalledApplication {
      public string Name { get; set; }
      public string Directory { get; set; }
      public string Version { get; set; }
    }

    public static IEnumerable<InstalledApplication> GetInstalledApplications(bool searchRegistry32 = true, bool searchRegistry64 = true) {
      if(searchRegistry32) {
        using(var rootKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)) {
          using(var uninstallKey = rootKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", false)) {
            foreach(var subKey in uninstallKey.GetSubKeyNames().Select(n => uninstallKey.OpenSubKey(n, false))) {
              var name = subKey.GetValue("DisplayName")?.ToString();
              var directory = subKey.GetValue("InstallLocation")?.ToString();
              if(!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(directory)) {
                yield return new InstalledApplication() {
                  Name = name,
                  Version = subKey.GetValue("DisplayVersion")?.ToString(),
                  Directory = directory,
                };
              }
            }
          }
        }
      }
      if(searchRegistry64) {
        using(var rootKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)) {
          using(var uninstallKey = rootKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", false)) {
            foreach(var subKey in uninstallKey.GetSubKeyNames().Select(n => uninstallKey.OpenSubKey(n, false))) {
              var name = subKey.GetValue("DisplayName")?.ToString();
              var directory = subKey.GetValue("InstallLocation")?.ToString();
              if(!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(directory)) {
                yield return new InstalledApplication() {
                  Name = name,
                  Version = subKey.GetValue("DisplayVersion")?.ToString(),
                  Directory = directory,
                };
              }
            }
          }
        }
      }
    }
  }
}

#endif