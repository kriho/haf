using System.Collections.Generic;
using System.Threading.Tasks;

namespace HAF {
  public interface IPluginsService: IService {
    IReadOnlyList<IPluginMetadata> InstalledPlugins { get; }
    IReadOnlyObservableCollection<IPluginEntry> Plugins { get; }
    IRelayCommand DoInstallPlugin { get; }
    IRelayCommand DoUninstallPlugin { get; }
    IRelayCommand DoPublishPlugin { get; }
    IRelayCommand DoRefreshPlugins { get; }
    IRelayCommand DoRestart { get; }

    IReadOnlyState IsDirty { get; }
    IPluginEntry SelectedPlugin { get; set; }

    Task InstallPlugin(IPluginEntry plugin);
    void UninstallPlugin(IPluginEntry plugin);
    Task RefreshPlugins(bool fetch);
  }
}