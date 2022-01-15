using System.Windows;

namespace HAF {
  public interface ISettingsRegistration {
    ISettingsRegion Region { get; }
    string Name { get; }
    int DisplayOrder { get; }
    ISettingsDrawer Drawer { get; }
    ISettingsValueBase Setting { get; }
    ISettingsOwner Owner { get; }
  }
}