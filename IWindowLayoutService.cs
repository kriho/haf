using HAF.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HAF {
  public interface IWindowLayoutService : IService {
    WindowLayout ActiveWindowLayout { get; set; }
    IReadOnlyObservableCollection<PaneMeta> AvailablePanes { get; }
    ICompoundState CanChangeWindowLayout { get; }
    WindowLayout DefaultWindowLayout { get; set; }
    IReadOnlyCollection<WindowLayout> DefaultWindowLayouts { get; }
    RelayCommand<WindowLayout> DoDelete { get; }
    RelayCommand<WindowLayout> DoLoad { get; }
    Event OnActiveWindowLayoutChanged { get; }
    RelayCommand<WindowLayout> DoSave { get; }
    RelayCommand<WindowLayout> DoSetDefault { get; }
    RelayCommand<PaneMeta> DoShowPane { get; }
    IReadOnlyObservableCollection<WindowLayout> WindowLayouts { get; }

    void AddWindowLayout(string name);
  }
}