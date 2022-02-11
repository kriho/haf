using System.ComponentModel;

namespace HAF {
  public interface IPluginEntry: IPluginMetadata, INotifyPropertyChanged {
    /// <summary>
    /// Is the plugin installed.
    /// </summary>
    bool Installed { get; set; }

    /// <summary>
    /// Is an update for the plugin available.
    /// </summary>
    bool UpdateAvailable { get; set; }

    /// <summary>
    /// Is the plugin installation pending.
    /// </summary>
    bool PendingInstall { get; set; }

    /// <summary>
    /// Is the plugin uninstallation pending.
    /// </summary>
    bool PendingUninstall { get; set; }
  }
}