using HAF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HAF {
  public interface IWindowLayoutService : IService {
    /// <summary>
    /// The window layout that is currently used by the application.
    /// </summary>
    IWindowLayout ActiveWindowLayout { get; set; }

    /// <summary>
    /// Available panes.
    /// </summary>
    IReadOnlyObservableCollection<IPaneMeta> AvailablePanes { get; }

    /// <summary>
    /// Is changing the window layout allowed.
    /// </summary>
    ICompoundState CanChangeWindowLayout { get; }

    /// <summary>
    /// The window layout that is loaded on application start and saved on application exit.
    /// </summary>
    IWindowLayout DefaultWindowLayout { get; set; }

    /// <summary>
    /// Window layouts that are added on application start when not already available.
    /// </summary>
    IReadOnlyCollection<IWindowLayout> DefaultWindowLayouts { get; }

    /// <summary>
    /// Event that fires when the window layout that is currently used by the application changes.
    /// </summary>
    IReadOnlyEvent OnActiveWindowLayoutChanged { get; }

    /// <summary>
    /// Delete the provided window layout.
    /// </summary>
    IRelayCommand<IWindowLayout> DoDeleteWindowLayout { get; }

    /// <summary>
    /// Load the provided window layout.
    /// </summary>
    IRelayCommand<IWindowLayout> DoLoadWindowLayout { get; }

    /// <summary>
    /// Save provided window layout.
    /// </summary>
    IRelayCommand<IWindowLayout> DoSaveWindowLayout { get; }

    /// <summary>
    /// Set provided window layout as default.
    /// </summary>
    IRelayCommand<IWindowLayout> DoSetDefaultWindowLayout { get; }

    /// <summary>
    /// Show the provided pane.
    /// </summary>
    IRelayCommand<IPaneMeta> DoShowPane { get; }

    /// <summary>
    /// Add new window layout with name provided by <c>EditName</c>.
    /// </summary>
    IRelayCommand DoAddWindowLayout { get; }

    /// <summary>
    /// Available window layout.
    /// </summary>
    IReadOnlyObservableCollection<IWindowLayout> WindowLayouts { get; }

    /// <summary>
    /// Name used for creating new window layouts.
    /// </summary>
    string EditName { get; set; }

    /// <summary>
    /// Add new window layout with provided name.
    /// </summary>
    void AddWindowLayout(string name);

    /// <summary>
    /// Add default window layout. Default window layouts are added on application start when not already available.
    /// </summary>
    /// <param name="name">Name of the window layout.</param>
    /// <param name="layout">Serialized layout value of the window layout.</param>
    void AddDefaultWindowLayout(string name, string layout);

    /// <summary>
    /// Register pane.
    /// </summary>
    /// <param name="name">Name of the pane.</param>
    /// <param name="type">Type of the view hosted by the pane.</param>
    /// <param name="canUserClose">Can the user close the pane.</param>
    void RegisterAvailablePane(string name, Type type, bool canUserClose = true);
  }
}