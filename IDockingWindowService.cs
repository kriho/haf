using System;

namespace HAF {
  public interface IDockingWindowService : IWindowService {
    /// <summary>
    /// Docking control of the window.
    /// </summary>
    object Docking { get; set; }

    /// <summary>
    /// Serialize the current window layout.
    /// </summary>
    /// <returns>Serialized window layout.</returns>
    string GetWindowLayout();

    /// <summary>
    /// Load window layout.
    /// </summary>
    /// <param name="layout">Serialized window layout.</param>
    void SetWindowLayout(string layout);

    /// <summary>
    /// Show pane if it exists.
    /// </summary>
    void ShowPane(string name);

    /// <summary>
    /// Move pane to the active pane group if it exists.
    /// </summary>
    /// <param name="name"></param>
    void MovePane(string name);

    /// <summary>
    /// Show pane and create it if it does not exist.
    /// </summary>
    /// <param name="viewType">Type of the view hosted by the pane.</param>
    /// <param name="canUserClose">Can the user close the pane.</param>
    void ShowPane(string name, Type viewType, bool canUserClose);
  }
}