namespace HAF {
  public interface IWindowService : IService {
    /// <summary>
    /// Main window of the application.
    /// </summary>
    Window Window { get; set; }

    /// <summary>
    /// Register control of window.
    /// </summary>
    /// <typeparam name="T">Type of the control.</typeparam>
    /// <param name="name">Name of the control.</param>
    void RegisterControl<T>(string name, T control);

    /// <summary>
    /// Try to get a registered control.
    /// </summary>
    /// <typeparam name="T">Type of the control.</typeparam>
    /// <param name="name">Name of the control.</param>
    /// <returns>True if the control was found.</returns>
    bool TryGetControl<T>(string name, out T control);

    /// <summary>
    /// Get a registered control.
    /// </summary>
    /// <typeparam name="T">Type of the control.</typeparam>
    /// <param name="name">Name of the control.</param>
    /// <returns>Registered control or NULL if no matching control was found.</returns>
    T GetControl<T>(string name);
  }
}