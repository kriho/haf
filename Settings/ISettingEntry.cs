namespace HAF {
  public interface ISettingEntry {
    string Name { get; }
    ISettingRegion Region { get; }
    int Priority { get; }
    ISettingMeta Meta { get; }
  }
}