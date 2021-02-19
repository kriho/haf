using HAF.Models;
using System.Collections.ObjectModel;
using Telerik.Windows.Data;

namespace HAF {
  public interface IWindowLayoutService : IService {
    RelayCommand<Models.WindowLayout> _Delete { get; }
    RelayCommand<Models.WindowLayout> _Load { get; }
    RelayCommand<Models.WindowLayout> _Save { get; }
    RelayCommand<Models.WindowLayout> _SetDefault { get; }
    RelayCommand<PaneMeta> _ShowPane { get; }
    ObservableCollection<PaneMeta> AvailablePanes { get; }
    Models.WindowLayout ActiveWindowLayout { get; set; }
    Models.WindowLayout DefaultWindowLayout { get; set; }
    RadObservableCollection<Models.WindowLayout> DefaultWindowLayouts { get; }
    RadObservableCollection<Models.WindowLayout> WindowLayouts { get; }

    void AddWindowLayout(string name);
  }
}