using HAF.Models;
using System.Windows.Media;
using Telerik.Windows.Data;

namespace HAF {
  public interface IThemesService: IService {
    RelayCommand<Models.Theme> _SetTheme { get; }
    Color AccentColor { get; }
    Models.Theme ActiveTheme { get; set; }
    RadObservableCollection<Models.Theme> AvailableThemes { get; }
    Color BasicColor { get; }
    Color MainColor { get; }
    Color MarkerColor { get; }
    Color StrongColor { get; }
  }
}