using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public class PluginEntry: ObservableObject, IPluginEntry {
    public string PluginName { get; set; }
    
    public string PluginDescription { get; set; }
    
    public string PluginCategory { get; set; }

    public string PluginIcon { get; set; }
    
    public int PluginVersion { get; set; }

    public bool Installed { get; set; } = false;
    
    public bool UpdateAvailable { get; set; } = false;

    private bool pendingInstall = false;
    public bool PendingInstall {
      get => this.pendingInstall;
      set => this.SetValue(ref this.pendingInstall, value);
    }

    private bool pendingUninstall = false;
    public bool PendingUninstall {
      get => this.pendingUninstall;
      set => this.SetValue(ref this.pendingUninstall, value);
    }
  }
}
