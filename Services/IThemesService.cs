using HAF.Models;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace HAF {
  public interface IThemesService : IService {
    RelayCommand<Theme> DoSetTheme { get; }
    Color AccentColor { get; }
    Theme ActiveTheme { get; set; }
    NotifyCollection<Theme> AvailableThemes { get; }
    Color BasicColor { get; }
    LinkedDependency MayChangeTheme { get; }
    Color MainColor { get; }
    Color MarkerColor { get; }
    LinkedEvent OnActiveThemeChanged { get; }
    Color StrongColor { get; }
  }
}