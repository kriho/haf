using System.Windows;

namespace HAF {
  public interface ISettingsRegistration {
    /// <summary>
    /// The parent region of the setting.
    /// </summary>
    ISettingsRegion Region { get; }

    /// <summary>
    /// Internal name of the setting. Used to reference the setting.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Display order of the setting.
    /// </summary>
    int DisplayOrder { get; }

    /// <summary>
    /// Drawer used to visualize the setting.
    /// </summary>
    ISettingsDrawer Drawer { get; }

    /// <summary>
    /// Typed setting value.
    /// </summary>
    ISettingsValueBase Setting { get; }

    /// <summary>
    /// Owner of the setting. The <c>SettingsService</c> owns the setting when NULL.
    /// </summary>
    ISettingsOwner Owner { get; }
  }
}