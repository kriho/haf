﻿using System;

namespace HAF {
  public interface ISettingsRegion {
    string Name { get; }
    string DisplayName { get; }
    string Description { get; }
    int? DisplayOrder { get; }
    void Reconfigure(string displayName = null, string description = null, int? displayOrder = 0);
    IReadOnlyObservableCollection<ISettingsRegistration> Settings { get; }
    ISetting<T> RegisterValue<T>(string name, ISetting<T> value, ISettingsDrawer drawer = null, int displayOrder = 0);
    void RegisterDrawer(ISettingsDrawer drawer, int displayOrder = 0);
  }
}