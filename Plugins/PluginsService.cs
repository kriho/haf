using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HAF {
  [InheritedExport(typeof(IPluginsService)), PartCreationPolicy(CreationPolicy.Shared)]
  public abstract class PluginsService: Service, IPluginsService {
    private RangeObservableCollection<IPluginEntry> plugins = new RangeObservableCollection<IPluginEntry>();
    public IReadOnlyObservableCollection<IPluginEntry> Plugins => this.plugins;

    public IReadOnlyList<IPluginMetadata> InstalledPlugins { get; private set; }

    private State isDirty = new State(false);
    public IReadOnlyState IsDirty => this.isDirty;

    public IRelayCommand DoRefreshPlugins { get; private set; }

    public IRelayCommand DoInstallSelectedPlugin { get; private set; }

    public IRelayCommand DoUninstallSelectedPlugin { get; private set; }

    public IRelayCommand DoPublishSelectedPlugin { get; private set; }

    public IRelayCommand DoRestart { get; private set; }

    private IPluginEntry selectedPlugin = null;
    public IPluginEntry SelectedPlugin {
      get => this.selectedPlugin;
      set {
        if(this.SetValue(ref this.selectedPlugin, value)) {
          this.DoInstallSelectedPlugin.RaiseCanExecuteChanged();
          this.DoUninstallSelectedPlugin.RaiseCanExecuteChanged();
          this.DoPublishSelectedPlugin.RaiseCanExecuteChanged();
        }
      }
    }

    [ImportingConstructor]
    public PluginsService([ImportMany] IEnumerable<Lazy<IPlugin, IPluginMetadata>> plugins) {
      this.InstalledPlugins = plugins.Select(p => p.Metadata).ToList();
      this.DoRefreshPlugins = new RelayCommand(() => {
        _ = this.RefreshPlugins(true);
      });
      this.DoInstallSelectedPlugin = new RelayCommand(async () => {
        await this.InstallPlugin(this.selectedPlugin);
      }, () => this.selectedPlugin != null && (!this.selectedPlugin.Installed || this.selectedPlugin.UpdateAvailable));
      this.DoUninstallSelectedPlugin = new RelayCommand(() => {
        this.UninstallPlugin(this.selectedPlugin);
      }, () => this.selectedPlugin != null && this.selectedPlugin.Installed);
      this.DoPublishSelectedPlugin = new RelayCommand(async () => {
        await this.PushPlugin(this.selectedPlugin, this.GetPluginPath(this.selectedPlugin));
      }, () => this.selectedPlugin != null && this.selectedPlugin.Installed && !this.selectedPlugin.UpdateAvailable);
      this.DoRestart = new RelayCommand(() => {
        System.Windows.Forms.Application.Restart();
        System.Windows.Application.Current.Shutdown();
      }, this.isDirty);
      _ = this.RefreshPlugins(false);
      _ = this.InitializePlugins(plugins);
    }

    private async Task InitializePlugins(IEnumerable<Lazy<IPlugin, IPluginMetadata>> plugins) {
      foreach(var plugin in plugins) {
        if(plugin.Metadata.PluginInitializationRequred) {
          await plugin.Value.Initialize();
        }
      }
    }
    
    protected abstract Task<IEnumerable<IPluginMetadata>> FetchAvailablePlugins();

    protected abstract Task PullPlugin(IPluginMetadata plugin, string filePath);

    protected abstract Task PushPlugin(IPluginMetadata plugin, string filePath);

    public async Task RefreshPlugins(bool fetch) {
      var plugins = new List<IPluginEntry>();
      // list installed plugins
      foreach(var installedPlugin in this.InstalledPlugins) {
        plugins.Add(new PluginEntry() {
          PluginName = installedPlugin.PluginName,
          PluginDescription = installedPlugin.PluginDescription,
          PluginCategory = installedPlugin.PluginCategory,
          PluginIcon = installedPlugin.PluginIcon,
          PluginVersion = installedPlugin.PluginVersion,
          Installed = true,
        });
      }
      // search for local plugins
      foreach(var filePath in Directory.GetFiles(Path.Combine(HAF.Core.ConfigurationDirectory, "extensions"), "*.dll", SearchOption.TopDirectoryOnly)) {
        var name = Path.GetFileNameWithoutExtension(filePath);
        if(!plugins.Any(p => p.PluginName == name)) {
          plugins.Add(new PluginEntry() {
            PluginName = Path.GetFileNameWithoutExtension(filePath),
            PendingInstall = true,  // indicate that plugin is installed after restart
          });
          this.isDirty.Value = true;
        }
      }
      // fetch remote plugins
      if(fetch) {
        foreach(var availablePlugin in await this.FetchAvailablePlugins()) {
          var localPlugin = plugins.FirstOrDefault(p => p.PluginName == availablePlugin.PluginName);
          if(localPlugin != null) {
            if(availablePlugin.PluginVersion > localPlugin.PluginVersion) {
              localPlugin.UpdateAvailable = true;
            }
          } else {
            plugins.Add(new PluginEntry() {
              PluginName = availablePlugin.PluginName,
              PluginVersion = availablePlugin.PluginVersion, 
              PluginCategory = availablePlugin.PluginCategory,
              PluginIcon = availablePlugin.PluginIcon,
              PluginDescription = availablePlugin.PluginDescription,
            });
          }
        }
      }
      // update
      this.plugins.Clear();
      this.plugins.AddRange(plugins);
    }

    public async Task InstallPlugin(IPluginEntry plugin) {
      var filePath = this.GetPluginPath(plugin);
      if(File.Exists(filePath)) {
        filePath += ".replace";
      }
      await this.PullPlugin(plugin, filePath);
      plugin.PendingInstall = true;
      this.isDirty.Value = true;
    }

    public void UninstallPlugin(IPluginEntry plugin) {
      var filePath = this.GetPluginPath(plugin);
      File.Create(filePath + ".delete");
      plugin.PendingUninstall = true;
      this.isDirty.Value = true;
    }

    private string GetPluginPath(IPluginMetadata plugin) {
      return Path.Combine(HAF.Core.ConfigurationDirectory, "extensions", plugin.PluginName + ".dll");
    }
  }
}
