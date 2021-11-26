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
    private ObservableCollection<ISettingsRegion> regions = new ObservableCollection<ISettingsRegion>();
    public IReadOnlyObservableCollection<ISettingsRegion> Regions => (IReadOnlyObservableCollection<ISettingsRegion>)this.regions;

    public IEnumerable<ExportFactory<ISettingsDrawer, ISettingsDrawerMeta>> Adapters { get; private set; }

    [ImportingConstructor]
    public SettingsService([ImportMany] IEnumerable<ExportFactory<ISettingsDrawer, ISettingsDrawerMeta>> adapters) {
      this.Adapters = adapters;
    }

    public ISettingsRegion RegisterRegion(string name, string displayName = null, string description = null, int? displayOrder = 0) {
      var region = this.regions.FirstOrDefault(r => r.Name == name);
      if (region == null) {
        region = new SettingsRegion(this) {
          Name = name,
          DisplayName = displayName,
          Description = description,
          DisplayOrder = displayOrder,
        };
        if(displayOrder.HasValue) {
          int insertIndex;
          for(insertIndex = 0; insertIndex < this.regions.Count; insertIndex++) {
            if(this.regions[insertIndex].DisplayOrder >= displayOrder) {
              break;
            }
          }
          this.regions.Insert(insertIndex, region);
        } else {
          this.regions.Add(region);
        }
      }
      if(displayOrder != null) {
        int insertIndex;
        for(insertIndex = 0; insertIndex < this.regions.Count; insertIndex++) {
          if(this.regions[insertIndex].DisplayOrder >= displayOrder) {
            break;
          }
        }
        regions.Move(regions.IndexOf(region), insertIndex);
      }
      region.Reconfigure(displayName, description, displayOrder);
      return region;
    }

    private ISettingsRegion GetRegion(string regionName) {
      if(regionName == null) {
        return null;
      }
      var region = this.regions.FirstOrDefault(r => r.Name == regionName);
      if(region == null) {
        throw new InvalidOperationException($"referenced settings region \"{regionName}\" does not exist");
      }
      return region;
    }

    public bool TryFindSetting<T>(string regionName, string name, out T value) {
      var region = this.GetRegion(regionName);
      var setting = region.Settings.FirstOrDefault(r => r.Name == name);
      if(setting == null) {
        value = default(T);
        return false;
      }
      value = (T)setting.SettingsValue;
      return true;
    }

    public override Task LoadConfiguration(ServiceConfiguration configuration) {
      foreach(var region in this.regions) {
        configuration.TryReadEntry(region.Name, out var regionEntry);
        foreach(var setting in region.Settings) {
          if(setting.SettingsValue == null) {
            // just a drawer
            continue;
          }
          if(setting.SettingsValue is SettingInteger intSetting) {
            intSetting.Value = regionEntry?.ReadValue(setting.Name, intSetting.DefaultValue) ?? intSetting.DefaultValue;
          } else if(setting.SettingsValue is SettingString stringSetting) {
            stringSetting.Value = regionEntry?.ReadValue(setting.Name, stringSetting.DefaultValue) ?? stringSetting.DefaultValue;
          } else if(setting.SettingsValue is SettingBoolean boolSetting) {
            boolSetting.Value = regionEntry?.ReadValue(setting.Name, boolSetting.DefaultValue) ?? boolSetting.DefaultValue;
          } else {
            throw new Exception($"failes to load setting \"{setting.Name}\" from region \"{region.Name}\" of type {setting.SettingsValue.GetType().Name}");
          }
        }
      }
      return Task.CompletedTask;
    }

    public override Task SaveConfiguration(ServiceConfiguration configuration) {
      foreach(var region in this.regions) {
        var regionEntry = configuration.WriteEntry(region.Name, true);
        foreach(var setting in region.Settings) {
          if(setting.SettingsValue == null) {
            // just a drawer
            continue;
          }
          if(setting.SettingsValue.SaveSetting == null) {
            if(setting.SettingsValue is SettingInteger intSetting) {
              if(intSetting.SaveSetting == null) {
                regionEntry.WriteValue(setting.Name, intSetting.Value);
              } else {
                intSetting.SaveSetting(regionEntry);
              }
            } else if(setting.SettingsValue is SettingString stringSetting) {
              regionEntry.WriteValue(setting.Name, stringSetting.Value);
            } else if(setting.SettingsValue is SettingBoolean boolSetting) {
              regionEntry.WriteValue(setting.Name, boolSetting.Value);
            } else {
              throw new Exception($"failes to save setting \"{setting.Name}\" from region \"{region.Name}\" of type {setting.SettingsValue.GetType().Name}");
            }
          } else {
            setting.SettingsValue.SaveSetting(regionEntry);
          }
        }
      }
      return Task.CompletedTask;
    }

    public ISettingsDrawer GetDrawer(ISettingsValueBase settingsValue) {
      var factories = this.Adapters.Where(a => a.Metadata.AssociatedType.IsAssignableFrom(settingsValue.GetType()));
      if(factories.Count() == 0) {
        throw new Exception($"failed to get drawer for settings value of type {settingsValue.GetType()}");
      }
      if(settingsValue is SettingString settingsString) {
        if(settingsString.Mask == null) {
          factories = factories.Where(f => !f.Metadata.Features.Contains("mask"));
        } else {
          factories = factories.Where(f => f.Metadata.Features.Contains("mask"));
        }
      }
      if(factories.Count() == 0) {
        throw new Exception($"failed to get drawer with requested features for settings value of type {settingsValue.GetType()}");
      }
      return factories.First().CreateExport().Value;
    }
  }
}
