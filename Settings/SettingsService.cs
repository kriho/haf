using HAF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HAF {

  [Export(typeof(ISettingsService)), PartCreationPolicy(CreationPolicy.Shared)]
  public class SettingsService: Service, ISettingsService {
    private List<ISettingRegion> regions = new List<ISettingRegion>();

    private ObservableCollection<ISettingEntry> settings = new ObservableCollection<ISettingEntry>();
    public IReadOnlyObservableCollection<ISettingEntry> Settings => this.settings;

    public ICollectionSetting<T> RegisterCollectionSetting<T>(string name, string displayName, string description = null, string regionName = null, int priority = 0) {
      throw new NotImplementedException();
    }

    public void RegisterRegion(string name, string displayName, string description = null, int priority = 0) {
      if(this.regions.Any(r => r.Name == name)) {
        throw new InvalidOperationException($"the region \"{name}\" was already registered");
      }
      this.regions.Add(new SettingRegion() {
        Name = name,
        DisplayName = displayName,
        Priority = priority,
      });
    }

    private ISettingRegion GetRegion(string regionName) {
      if(regionName == null) {
        return null;
      }
      var region = this.regions.FirstOrDefault(r => r.Name == regionName);
      if(region == null) {
        throw new InvalidOperationException($"referenced region \"{regionName}\" does not exist");
      }
      return region;
    }

    public ISetting<T> RegisterSetting<T>(string name, string displayName, string description = null, string regionName = null, int priority = 0, Action<ValidationBatch> validation = null) {
      var region = this.GetRegion(regionName);
      if(this.settings.Any(r => r.Name == name && r.Region == region)) {
        throw new InvalidOperationException($"the setting \"{name}\" was already registered{(region != null ? $"in region \"{region.Name}\"" : "")}");
      }
      var setting = new Setting<T>() {
        DisplayName = displayName,
        Description = description,
        Validation = validation,
      };
      this.settings.Add(new SettingEntry() {
        Name = name,
        Meta = setting,
        Region = region,
        Priority = priority,
      });
      return setting;
    }

    public IStringSetting RegisterStringSetting(string name, string displayName, string description = null, string regionName = null, int priority = 0, Action<ValidationBatch> validation = null) {
      var region = this.GetRegion(regionName);
      if(this.settings.Any(r => r.Name == name && r.Region == region)) {
        throw new InvalidOperationException($"the setting \"{name}\" was already registered{(region != null ? $"in region \"{region.Name}\"" : "")}");
      }
      var setting = new StringSetting() {
        DisplayName = displayName,
        Description = description,
        Validation = validation,
      };
      this.settings.Add(new SettingEntry() {
        Name = name,
        Meta = setting,
        Region = region,
        Priority = priority,
      });
      return setting;
    }

    public ICollectionSetting<T> RegisterCollectionSetting<T>(string name, string displayName, string description = null, string regionName = null, int priority = 0, Action<ValidationBatch> validation = null) {
      var region = this.GetRegion(regionName);
      if(this.settings.Any(r => r.Name == name && r.Region == region)) {
        throw new InvalidOperationException($"the setting \"{name}\" was already registered{(region != null ? $"in region \"{region.Name}\"" : "")}");
      }
      var setting = new CollectionSetting<T>() {
        DisplayName = displayName,
        Description = description,
        Validation = validation,
      };
      this.settings.Add(new SettingEntry() {
        Name = name,
        Meta = setting,
        Region = region,
        Priority = priority,
      });
      return setting;
    }

    public bool TryFindSetting<T>(string name, string regionName, out T value) {
      var region = this.GetRegion(regionName);
      var setting = this.settings.FirstOrDefault(r => r.Name == name && r.Region == region);
      if(setting == null) {
        value = default(T);
        return false;
      }
      value = (T)setting.Meta;
      return true;
    }
  }
}
