using HAF.Models;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace HAF {

  public enum ThemeKey {
    Background,
    Text,
    Accent,
    Light,
    Strong,
    Info,
    Warning,
    Error
  }

  public interface IThemesService: IService {
    Color BackgroundColor { get; }
    Brush BackgroundBrush { get; }
    Color TextColor { get; }
    Brush TextBrush { get; }
    Color AccentColor { get; }
    Brush AccentBrush { get; }
    Color LightColor { get; }
    Brush LightBrush { get; }
    Color StrongColor { get; }
    Brush StrongBrush { get; }
    Color InfoColor { get; }
    Brush InfoBrush { get; }
    Color WarningColor { get; }
    Brush WarningBrush { get; }
    Color ErrorColor { get; }
    Brush ErrorBrush { get; }
    RelayCommand<Theme> DoSetTheme { get; }
    Theme ActiveTheme { get; set; }
    IObservableCollection<Theme> AvailableThemes { get; }
    LinkedDependency MayChangeTheme { get; }
    LinkedEvent OnActiveThemeChanged { get; }
    Theme DefaultLightTheme { get; }
    Theme DefaultDarkTheme { get; }
    Color GetColor(ThemeKey key);
    Brush GetBrush(ThemeKey key);
  }
}