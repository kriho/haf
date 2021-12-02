using HAF.Models;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace HAF {

  public enum ThemeKey {
    Control, // Office2016: main; Windows8: unused
    Background, // Office2016: alternative, mouse over, marker inverted; Windows8: main
    Text, // Office2016: marker; Windows8: marker
    Accent, // Office2016: accent, accent pressed, complementary; Windows8: accent
    Action, // Office2016: accent focused, accent mouse over; Windows8: unused
    Light, // Office2016: primary, selected; Windows8: basic
    Strong, // Office2016: icon, basic, pressed; Windows8: strong
    Info,
    Warning,
    Error // validation
  }

  public interface IThemesService: IService {
    IRelayCommand<ITheme> DoSetTheme { get; }
    IRelayCommand DoDuplicateTheme { get; }
    IRelayCommand<ITheme> DoDeleteTheme { get; }
    ITheme ActiveTheme { get; set; }
    IObservableCollection<ITheme> AvailableThemes { get; }
    ICompoundState CanChangeTheme { get; }
    IEvent OnActiveThemeChanged { get; }
    ITheme DefaultLightTheme { get; }
    ITheme DefaultDarkTheme { get; }
    Color GetColor(ThemeKey key);
    Brush GetBrush(ThemeKey key);
    void UpdateTheme(ITheme theme);
  }
}