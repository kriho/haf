namespace HAF {
  public class SettingEntry: ISettingEntry {
    public string Name { get; set; }
    public ISettingRegion Region { get; set; }
    public int Priority { get; set; }
    public ISettingMeta Meta { get; set; }
  }
}