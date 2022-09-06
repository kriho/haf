using System;

namespace HAF {
  public interface ISettingsRegion {
    /// <summary>
    /// Internal name of the region. Used to reference the region.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Display name of the region.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Description of the region.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Display order of the region.
    /// </summary>
    int? DisplayOrder { get; }

    /// <summary>
    /// Regonfigure region with new properties.
    /// </summary>
    void Reconfigure(string displayName = null, string description = null, int? displayOrder = 0);

    /// <summary>
    /// All registered settings.
    /// </summary>
    IReadOnlyObservableCollection<ISettingsRegistration> Registrations { get; }

    /// <summary>
    /// Register setting.
    /// </summary>
    /// <typeparam name="T">Type of the setting value.</typeparam>
    /// <param name="name">Internal name of the setting. Used to reference the setting.</param>
    /// <param name="value">Setting that is registered.</param>
    /// <param name="drawer">Drawer for visualizing the setting. A matching drawer is selected automatically when no drawer is provided.</param>
    /// <param name="displayOrder">The display order within the region.</param>
    /// <param name="owner">The setting owner. When no owner is provided, the <see cref="SettingsService"/> owns the setting.</param>
    /// <returns>The registered setting.</returns>
    ISetting<T> RegisterValue<T>(string name, ISetting<T> value, ISettingsDrawer drawer = null, int displayOrder = 0, ISettingsOwner owner = null);

    /// <summary>
    /// Register drawer for a manualy maintained setting.
    /// </summary>
    /// <param name="name">Internal name of the setting. Used to reference the setting.</param>
    /// <param name="drawer">Drawer for visualizing the setting.</param>
    /// <param name="displayOrder">The display order within the region.</param>
    void RegisterDrawer(string name, ISettingsDrawer drawer, int displayOrder = 0);

    /// <summary>
    /// Register setting without drawer.
    /// </summary>
    /// <typeparam name="T">Type of the setting value.</typeparam>
    /// <param name="name">Internal name of the setting. Used to reference the setting.</param>
    /// <param name="value">Setting that is registered.</param>
    /// <param name="owner">The setting owner. When no owner is provided, the <see cref="SettingsService"/> owns the setting.</param>
    /// <returns>The registered setting.</returns>
    ISetting<T> RegisterHiddenValue<T>(string name, ISetting<T> value, ISettingsOwner owner = null);
  }
}