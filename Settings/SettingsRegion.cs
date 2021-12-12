using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace HAF {
  public class SettingsRegion: ISettingsRegion {
    private ISettingsService parent;

    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }

    public int? DisplayOrder { get; set; }

    private ObservableCollection<ISettingsRegistration> settings = new ObservableCollection<ISettingsRegistration>();
    public IReadOnlyObservableCollection<ISettingsRegistration> Registrations => this.settings;

    public SettingsRegion(ISettingsService settingsService) {
      this.parent = settingsService;
    }

    public void Reconfigure(string displayName = null, string description = null, int? displayOrder = null) {
      if(displayName != null) {
        this.DisplayName = displayName;
      }
      if(description != null) {
        this.Description = description;
      }
      if(displayOrder != null) {
        this.DisplayOrder = displayOrder;
      }
    }

    public void RegisterDrawer(ISettingsDrawer drawer, int displayOrder = 0) {
      int insertIndex;
      for(insertIndex = 0; insertIndex < this.settings.Count; insertIndex++) {
        if(this.settings[insertIndex].DisplayOrder > displayOrder) {
          break;
        }
      }
      this.settings.Insert(insertIndex, new SettingsRegistration() {
        Name = null,
        Setting = null,
        DisplayOrder = displayOrder,
        Drawer = drawer,
      });
    }

    public ISetting<T> RegisterValue<T>(string name, ISetting<T> value, ISettingsDrawer drawer = null, int displayOrder = 0, ISettingsOwner owner = null) {
      if(this.settings.Any(r => r.Name == name)) {
        throw new InvalidOperationException($"the setting \"{name}\" was already registered in region \"{this.Name}\"");
      }
      int insertIndex;
      for(insertIndex = 0; insertIndex < this.settings.Count; insertIndex++) {
        if(this.settings[insertIndex].DisplayOrder > displayOrder) {
          break;
        }
      }
      if(drawer == null) {
        drawer = this.parent.GetDrawer(value);
      }
      var registration = new SettingsRegistration() {
        Name = name,
        Setting = value,
        DisplayOrder = displayOrder,
        Drawer = drawer,
        Owner = owner,
      };
      drawer.DataContext = registration;
      this.settings.Insert(insertIndex, registration);
      return value;
    }

    public override string ToString() {
      return $"{this.Name}:{this.DisplayOrder ?? '-'} ({this.Registrations.Count})";
    }
  }
}