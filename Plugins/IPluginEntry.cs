using System.ComponentModel;

namespace HAF {
  public interface IPluginEntry: IPluginMetadata, INotifyPropertyChanged {
    bool Installed { get; set; }
    bool UpdateAvailable { get; set; }
    bool PendingInstall { get; set; }
    bool PendingUninstall { get; set; }
  }
}