using HAF.Models;
using System.Collections.ObjectModel;

namespace HAF {
  public interface IWindowLayoutService : IService {
    WindowLayout ActiveWindowLayout { get; set; }
    NotifyCollection<PaneMeta> AvailablePanes { get; }
    LinkedDependency MayChangeWindowLayout { get; }
    WindowLayout DefaultWindowLayout { get; set; }
    RangeNotifyCollection<WindowLayout> DefaultWindowLayouts { get; }
    RelayCommand<WindowLayout> DeleteCommand { get; }
    RelayCommand<WindowLayout> LoadCommand { get; }
    LinkedEvent OnActiveWindowLayoutChanged { get; }
    RelayCommand<WindowLayout> DoSave { get; }
    RelayCommand<WindowLayout> SetDefaultCommand { get; }
    RelayCommand<PaneMeta> ShowPaneCommand { get; }
    IReadOnlyNotifyCollection<WindowLayout> WindowLayouts { get; }

    void AddWindowLayout(string name);
  }
}