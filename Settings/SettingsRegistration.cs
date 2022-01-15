using System.Windows;

namespace HAF {
  public class SettingsRegistration: ISettingsRegistration {
    public ISettingsRegion Region { get; set; }
    public string Name { get; set; }
    public int DisplayOrder { get; set; }
    public ISettingsDrawer Drawer { get; set; }
    public ISettingsValueBase Setting { get; set; }
    public ISettingsOwner Owner { get; set; }
  }
}