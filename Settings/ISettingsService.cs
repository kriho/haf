using HAF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HAF {
  public interface ISettingsService : IService {
    string Filter { get; set; }
    IRelayCommand DoClearFilter { get; }
    CollectionViewSource FilteredRegistrations { get; }
    Task LoadConfiguration(ISettingsOwner owner, ServiceConfiguration configuration);
    Task SaveConfiguration(ISettingsOwner owner, ServiceConfiguration configuration);
    ISettingsRegion RegisterRegion(string name, string displayName = null, string description = null, int? displayOrder = null);
    bool TryFindSetting<T>(string regionName, string name, out T value);
    IReadOnlyObservableCollection<ISettingsRegion> Regions { get; }
    ISettingsDrawer GetDrawer(ISettingsValueBase settingsValue);
    IReadOnlyList<ExportFactory<ISettingsDrawer, ISettingsDrawerMeta>> Drawers { get; }
  }
}