namespace HAF {
  public interface IWindowLayout {
    bool IsCurrent { get; set; }
    bool IsDefault { get; set; }
    string Layout { get; set; }
    string Name { get; }
  }
}