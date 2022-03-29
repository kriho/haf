using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Xml;

namespace HAF {

  [Export(typeof(ISettingsService)), PartCreationPolicy(CreationPolicy.Shared)]
  public class SettingsService: Service, ISettingsService {
    private ObservableCollection<ISettingsRegion> regions = new ObservableCollection<ISettingsRegion>();
    public IReadOnlyObservableCollection<ISettingsRegion> Regions => (IReadOnlyObservableCollection<ISettingsRegion>)this.regions;

    public IReadOnlyList<ExportFactory<ISettingsDrawer, ISettingsDrawerMeta>> Drawers { get; private set; }

    public CollectionViewSource FilteredRegistrations { get; private set; }

    private string filter;
    public string Filter {
      get { return this.filter; }
      set {
        if(this.SetValue(ref this.filter, value?.ToLower())) {
          this.FilteredRegistrations.View.Refresh();
        }
      }
    }

    public IRelayCommand DoClearFilter { get; private set; }

    private bool FilterRegion(ISettingsRegistration registration) {
      if(string.IsNullOrWhiteSpace(this.filter)) {
        return true;
      }
      if(registration.Setting == null) {
        // filter by drawer
        if(registration.Drawer.DisplayName != null && registration.Drawer.DisplayName.ToLower().Contains(this.filter)) {
          return true;
        }
        if(registration.Drawer.Description != null && registration.Drawer.Description.ToLower().Contains(this.filter)) {
          return true;
        }
      } else {
        // filter by setting
        if(registration.Setting.DisplayName != null && registration.Setting.DisplayName.ToLower().Contains(this.filter)) {
          return true;
        }
        if(registration.Setting.Description != null && registration.Setting.Description.ToLower().Contains(this.filter)) {
          return true;
        }
      }
      return false;
    }

    [ImportingConstructor]
    public SettingsService([ImportMany] IEnumerable<ExportFactory<ISettingsDrawer, ISettingsDrawerMeta>> drawers) {
      this.Drawers = drawers.ToList();
      this.FilteredRegistrations = new CollectionViewSource();
      this.FilteredRegistrations.GroupDescriptions.Add(new PropertyGroupDescription("Region"));
      this.FilteredRegistrations.Filter += (s2, e2) => {
        e2.Accepted = e2.Item is ISettingsRegistration registration && this.FilterRegion(registration);
      };
      Core.StageAction(ConfigurationStage.Running, () => {
        this.FilteredRegistrations.Source = this.regions.OrderBy(r => r.DisplayOrder).SelectMany(r => r.Registrations.OrderBy(e => e.DisplayOrder));
        this.FilteredRegistrations.View.Refresh();
        this.regions.CollectionChanged += (s1, e1) => {
          this.FilteredRegistrations.View.Refresh();
          this.FilteredRegistrations.Source = this.regions.OrderBy(r => r.DisplayOrder).SelectMany(r => r.Registrations.OrderBy(e => e.DisplayOrder));
        };
      });
      this.DoClearFilter = new RelayCommand(() => {
        this.Filter = "";
      });
    }

    public ISettingsRegion RegisterRegion(string name, string displayName = null, string description = null, int? displayOrder = null) {
      var region = this.regions.FirstOrDefault(r => r.Name == name);
      if(region == null) {
        region = new SettingsRegion(this) {
          Name = name,
          DisplayName = displayName,
          Description = description,
          DisplayOrder = displayOrder,
        };
        if(displayOrder != null) {
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
      } else {
        if(displayOrder != null) {
          int insertIndex;
          for(insertIndex = 0; insertIndex < this.regions.Count; insertIndex++) {
            if(this.regions[insertIndex].DisplayOrder >= displayOrder) {
              break;
            }
          }
          if(insertIndex == this.regions.Count) {
            insertIndex--;
          }
          regions.Move(regions.IndexOf(region), insertIndex);
        }
        region.Reconfigure(displayName, description, displayOrder);
      }
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
      var registration = region.Registrations.FirstOrDefault(r => r.Name == name);
      if(registration == null) {
        value = default(T);
        return false;
      }
      value = (T)registration.Setting;
      return true;
    }

    public Task LoadConfiguration(ISettingsOwner owner, ServiceConfiguration configuration) {
      foreach(var region in this.regions) {
        configuration.TryReadEntry(region.Name, out var regionEntry);
        foreach(var registration in region.Registrations) {
          if(registration.Setting == null || registration.Owner != owner) {
            // just a drawer or owned by other service
            continue;
          }
          if(registration.Setting is SettingInteger intSetting) {
            intSetting.Value = regionEntry?.ReadValue(registration.Name, intSetting.DefaultValue) ?? intSetting.DefaultValue;
          } else if(registration.Setting is SettingString stringSetting) {
            stringSetting.Value = regionEntry?.ReadValue(registration.Name, stringSetting.DefaultValue) ?? stringSetting.DefaultValue;
          } else if(registration.Setting is SettingBoolean boolSetting) {
            boolSetting.Value = regionEntry?.ReadValue(registration.Name, boolSetting.DefaultValue) ?? boolSetting.DefaultValue;
          } else {
            throw new Exception($"failes to load setting \"{registration.Name}\" from region \"{region.Name}\" of type {registration.Setting.GetType().Name}");
          }
        }
      }
      return Task.CompletedTask;
    }

    public override async Task LoadConfiguration(ServiceConfiguration configuration) {
      await this.LoadConfiguration(null, configuration);
    }

    public Task SaveConfiguration(ISettingsOwner owner, ServiceConfiguration configuration) {
      foreach(var region in this.regions) {
        var regionEntry = configuration.WriteEntry(region.Name, true);
        foreach(var registration in region.Registrations) {
          if(registration.Setting == null || registration.Owner != owner) {
            // just a drawer or owned by other service
            continue;
          }
          if(registration.Setting is SettingInteger intSetting) {
            regionEntry.WriteValue(registration.Name, intSetting.Value);
          } else if(registration.Setting is SettingString stringSetting) {
            regionEntry.WriteValue(registration.Name, stringSetting.Value);
          } else if(registration.Setting is SettingBoolean boolSetting) {
            regionEntry.WriteValue(registration.Name, boolSetting.Value);
          } else {
            throw new Exception($"failes to save setting \"{registration.Name}\" from region \"{region.Name}\" of type {registration.Setting.GetType().Name}");
          }
        }
      }
      return Task.CompletedTask;
    }

    public override async Task SaveConfiguration(ServiceConfiguration configuration) {
      await this.SaveConfiguration(null, configuration);
    }

    public ISettingsDrawer GetDrawer(ISettingsValueBase settingsValue) {
      var factories = this.Drawers.Where(a => a.Metadata.AssociatedType.IsAssignableFrom(settingsValue.GetType()));
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
