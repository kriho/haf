using System.Windows;

namespace HAF {
  public class SettingsRegistration: ISettingsRegistration {
    public string Name { get; set; }
    public int DisplayOrder { get; set; }
    public ISettingsDrawer Drawer { get; set; }
    public ISettingsValueBase SettingsValue { get; set; }
  }
}