using System.Collections.Generic;
using System.Threading.Tasks;

namespace HAF {
  public interface IPluginsService: IService {
    /// <summary>
    /// The installed plugins.
    /// </summary>
    IReadOnlyList<IPluginMetadata> InstalledPlugins { get; }

    /// <summary>
    /// The list of all installed and available plugins in their current state.
    /// </summary>
    IReadOnlyObservableCollection<IPluginEntry> Plugins { get; }

    /// <summary>
    /// Install selected plugin.
    /// </summary>
    IRelayCommand DoInstallSelectedPlugin { get; }

    /// <summary>
    /// Uninstall selected plugin.
    /// </summary>
    IRelayCommand DoUninstallSelectedPlugin { get; }

    /// <summary>
    /// Publish selected plugin.
    /// </summary>
    IRelayCommand DoPublishSelectedPlugin { get; }

    /// <summary>
    /// Refresh list of all installed and available plugins and their current states.
    /// </summary>
    IRelayCommand DoRefreshPlugins { get; }

    /// <summary>
    /// Restart application to apply pending plugin changes.
    /// </summary>
    IRelayCommand DoRestart { get; }

    /// <summary>
    /// Are plugins changes pending.
    /// </summary>
    IReadOnlyState IsDirty { get; }

    /// <summary>
    /// The selected plugin.
    /// </summary>
    IPluginEntry SelectedPlugin { get; set; }

    /// <summary>
    /// Request plugin installation.
    /// </summary>
    Task InstallPlugin(IPluginEntry plugin);

    /// <summary>
    /// Request plugin unisntallation.
    /// </summary>
    void UninstallPlugin(IPluginEntry plugin);

    /// <summary>
    /// Refresh list of all installed and available plugins and their current states.
    /// </summary>
    Task RefreshPlugins(bool fetch);
  }
}