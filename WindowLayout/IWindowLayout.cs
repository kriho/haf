namespace HAF {
  public interface IWindowLayout {
    /// <summary>
    /// Is the window layout currenty used by the application.
    /// </summary>
    bool IsCurrent { get; set; }

    /// <summary>
    /// Is the window layout loaded on application start and saved on application exit.
    /// </summary>
    bool IsDefault { get; set; }

    /// <summary>
    /// Serialized value of the window layout.
    /// </summary>
    string Layout { get; set; }

    /// <summary>
    /// Name of the window layout.
    /// </summary>
    string Name { get; }
  }
}