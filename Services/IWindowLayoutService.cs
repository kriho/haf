using HAF.Models;
using System.Collections.ObjectModel;

namespace HAF {
  public interface IWindowLayoutService : IService {
    WindowLayout ActiveWindowLayout { get; set; }
    NotifyCollection<PaneMeta> AvailablePanes { get; }
    LinkedDependency MayChangeWindowLayout { get; }
    WindowLayout DefaultWindowLayout { get; set; }
    RangeNotifyCollection<WindowLayout> DefaultWindowLayouts { get; }
    RelayCommand<WindowLayout> DoDelete { get; }
    RelayCommand<WindowLayout> DoLoad { get; }
    LinkedEvent OnActiveWindowLayoutChanged { get; }
    RelayCommand<WindowLayout> DoSave { get; }
    RelayCommand<WindowLayout> DoSetDefault { get; }
    RelayCommand<PaneMeta> DoShowPane { get; }
    IReadOnlyNotifyCollection<WindowLayout> WindowLayouts { get; }

    void AddWindowLayout(string name);
  }
}