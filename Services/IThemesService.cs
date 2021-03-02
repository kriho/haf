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
    LinkedDependency MayChangeTheme { get; }
    Color MainColor { get; }
    Color MarkerColor { get; }
    LinkedEvent OnActiveThemeChanged { get; }
    Color StrongColor { get; }
  }
}