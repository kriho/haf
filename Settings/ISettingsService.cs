using HAF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace HAF {
  public interface ISettingsService : IService {
    void RegisterRegion(string name, string displayName, string description = null, int priority = 0);
    ISetting<T> RegisterSetting<T>(string name, string displayName, string description = null, string regionName = null, int priority = 0, Action<ValidationBatch> validation = null);
    IStringSetting RegisterStringSetting(string name, string displayName, string description = null, string regionName = null, int priority = 0, Action<ValidationBatch> validation = null);
    ICollectionSetting<T> RegisterCollectionSetting<T>(string name, string displayName, string description = null, string regionName = null, int priority = 0, Action<ValidationBatch> validation = null);
    bool TryFindSetting<T>(string name, string regionName, out T value);
    IReadOnlyObservableCollection<ISettingEntry> Settings { get; }
  }
}