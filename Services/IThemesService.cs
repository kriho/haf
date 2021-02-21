using HAF.Models;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace HAF {
  public interface IThemesService : IService {
    RelayCommand<Theme> _SetTheme { get; }
    Color AccentColor { get; }
    Theme ActiveTheme { get; set; }
    ObservableCollection<Theme> AvailableThemes { get; }
    Color BasicColor { get; }
    ServiceDependency CanChangeTheme { get; }
    Color MainColor { get; }
    Color MarkerColor { get; }
    ServiceEvent OnActiveThemeChanged { get; }
    Color StrongColor { get; }
  }
}