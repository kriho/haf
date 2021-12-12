using HAF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace HAF {
  public interface ISettingsService : IService {
    Task LoadConfiguration(ISettingsOwner owner, ServiceConfiguration configuration);
    Task SaveConfiguration(ISettingsOwner owner, ServiceConfiguration configuration);
    ISettingsRegion RegisterRegion(string name, string displayName = null, string description = null, int? displayOrder = 0);
    bool TryFindSetting<T>(string regionName, string name, out T value);
    IReadOnlyObservableCollection<ISettingsRegion> Regions { get; }
    ISettingsDrawer GetDrawer(ISettingsValueBase settingsValue);
    IReadOnlyList<ExportFactory<ISettingsDrawer, ISettingsDrawerMeta>> Drawers { get; }
  }
}