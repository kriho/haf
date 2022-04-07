using System.Collections.ObjectModel;
using System.Windows.Media;

namespace HAF {
  public interface IThemesService: IService {
    IRelayCommand<ITheme> DoSetTheme { get; }
    IRelayCommand DoDuplicateTheme { get; }
    IRelayCommand<ITheme> DoDeleteTheme { get; }
    IRelayCommand<ITheme> DoSetActiveTheme { get; }
    ITheme ActiveTheme { get; set; }
    IObservableCollection<ITheme> AvailableThemes { get; }
    ICompoundState MayChangeActiveTheme { get; }
    IEvent OnActiveThemeChanged { get; }
    ITheme DefaultLightTheme { get; }
    ITheme DefaultDarkTheme { get; }
    void UpdateTheme(ITheme theme);
  }
}