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

    /// <summary>
    /// Progress description of an operation that is blocking the entire window.
    /// </summary>
    IObservableTaskProgress BusyProgress { get; }

    /// <summary>
    /// Is the window blocked by an ongoing operation. Set to true when <see cref="BusyProgress.Description"/> is not null and not empty.
    /// </summary>
    IReadOnlyState IsBusy { get; }
  }
}