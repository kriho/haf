using HAF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace HAF {

  [Export(typeof(IThemesService)), PartCreationPolicy(CreationPolicy.Shared)]
  public class ThemesService : Service, IThemesService {

    public LinkedDependency MayChangeTheme { get; private set; } = new LinkedDependency();

    public LinkedEvent OnActiveThemeChanged { get; private set; } = new LinkedEvent(nameof(OnActiveThemeChanged));

    /// <summary>
    /// the list of themes that can be selected and applied
    /// </summary>
    /// <remarks>
    /// the first theme is the default theme
    /// </remarks>
    public IObservableCollection<ITheme> AvailableThemes { get; private set; } = new ObservableCollection<ITheme>();

    private ITheme activeTheme;
    public ITheme ActiveTheme {
      get { return this.activeTheme; }
      set {
        if(!this.MayChangeTheme) {
          // theme changing is not allowed
          return;
        }
        if (this.SetValue(ref this.activeTheme, value)) {
          if(value != null) {
            this.ApplyTheme(value);
          }
          this.OnActiveThemeChanged.Fire();
        }
      }
    }

    protected virtual void ApplyTheme(ITheme theme) {
      // update resources
      Application.Current.Resources["ThemeActionColor"] = theme.ActionColor;
      Application.Current.Resources["ThemeControlColor"] = theme.ControlColor;
      Application.Current.Resources["ThemeBackgroundColor"] = theme.BackgroundColor;
      Application.Current.Resources["ThemeBackgroundColor"] = theme.BackgroundColor;
      Application.Current.Resources["ThemeTextColor"] = theme.TextColor;
      Application.Current.Resources["ThemeAccentColor"] = theme.AccentColor;
      Application.Current.Resources["ThemeLightColor"] = theme.LightColor;
      Application.Current.Resources["ThemeStrongColor"] = theme.StrongColor;
      Application.Current.Resources["ThemeInfoColor"] = theme.InfoColor;
      Application.Current.Resources["ThemeWarningColor"] = theme.WarningColor;
      Application.Current.Resources["ThemeErrorColor"] = theme.ErrorColor;
      Application.Current.Resources["ThemeActionBrush"] = new SolidColorBrush(theme.ActionColor);
      Application.Current.Resources["ThemeControlBrush"] = new SolidColorBrush(theme.ControlColor);
      Application.Current.Resources["ThemeBackgroundBrush"] = new SolidColorBrush(theme.BackgroundColor);
      Application.Current.Resources["ThemeTextBrush"] = new SolidColorBrush(theme.TextColor);
      Application.Current.Resources["ThemeAccentBrush"] = new SolidColorBrush(theme.AccentColor);
      Application.Current.Resources["ThemeLightBrush"] = new SolidColorBrush(theme.LightColor);
      Application.Current.Resources["ThemeStrongBrush"] = new SolidColorBrush(theme.StrongColor);
      Application.Current.Resources["ThemeInfoBrush"] = new SolidColorBrush(theme.InfoColor);
      Application.Current.Resources["ThemeWarningBrush"] = new SolidColorBrush(theme.WarningColor);
      Application.Current.Resources["ThemeErrorBrush"] = new SolidColorBrush(theme.ErrorColor);
    }

    public Color GetColor(ThemeKey key) {
      switch (key) {
        case ThemeKey.Control: return this.activeTheme.ControlColor;
        case ThemeKey.Accent: return this.activeTheme.AccentColor;
        case ThemeKey.Background: return this.activeTheme.BackgroundColor;
        case ThemeKey.Action: return this.activeTheme.ActionColor;
        case ThemeKey.Light: return this.activeTheme.LightColor;
        case ThemeKey.Strong: return this.activeTheme.StrongColor;
        case ThemeKey.Warning: return this.activeTheme.WarningColor;
        case ThemeKey.Info: return this.activeTheme.InfoColor;
        case ThemeKey.Error: return this.activeTheme.ErrorColor;
        default: return this.activeTheme.TextColor;
      }
    }

    public Brush GetBrush(ThemeKey key) {
      return new SolidColorBrush(this.GetColor(key));
    }

    public RelayCommand<ITheme> DoSetTheme { get; private set; }

    public ITheme DefaultLightTheme { get; private set; } = new Theme() {
      Name = new LocalizedText("Light"),
      AccentColor = (Color)ColorConverter.ConvertFromString("#FF2C61AC"),
      BackgroundColor = (Color)ColorConverter.ConvertFromString("#FFF1F1F1"),
      ControlColor = (Color)ColorConverter.ConvertFromString("#FFFFFFFF"),
      LightColor = (Color)ColorConverter.ConvertFromString("#FFE6E6E6"),
      StrongColor = (Color)ColorConverter.ConvertFromString("#FFABABAB"),
      TextColor = (Color)ColorConverter.ConvertFromString("#FF444444"),
      ActionColor = (Color)ColorConverter.ConvertFromString("#FF527FC0"),
      InfoColor = (Color)ColorConverter.ConvertFromString("#FFD3EBFC"),
      WarningColor = (Color)ColorConverter.ConvertFromString("#FFFFFAC3"),
      ErrorColor = (Color)ColorConverter.ConvertFromString("#FFFFD2D2"),
    };

    public ITheme DefaultDarkTheme { get; private set; } = new Theme() {
      Name = new LocalizedText("Dark"),
      AccentColor = (Color)ColorConverter.ConvertFromString("#FF8D9FC3"),
      BackgroundColor = (Color)ColorConverter.ConvertFromString("#FF232323"),
      ControlColor = (Color)ColorConverter.ConvertFromString("#FF303030"),
      LightColor = (Color)ColorConverter.ConvertFromString("#FF414141"),
      StrongColor = (Color)ColorConverter.ConvertFromString("#FF5B5B5B"),
      TextColor = (Color)ColorConverter.ConvertFromString("#FFFFFFFF"),
      ActionColor = (Color)ColorConverter.ConvertFromString("#FFA5B9DE"),
      InfoColor = (Color)ColorConverter.ConvertFromString("#FF49555E"),
      WarningColor = (Color)ColorConverter.ConvertFromString("#FF7D7840"),
      ErrorColor = (Color)ColorConverter.ConvertFromString("#FF901818"),
    };

    public ThemesService() {
      this.DoSetTheme = new RelayCommand<ITheme>(theme => {
        this.ActiveTheme = theme;
      }, (theme) => {
        return theme != null && this.MayChangeTheme;
      });
      this.MayChangeTheme.RegisterUpdate(() => {
        this.DoSetTheme.RaiseCanExecuteChanged();
      });
    }

    public override void LoadConfiguration(ServiceConfiguration configuration) {
      ITheme theme = null;
      if (configuration.TryReadValue("theme", out string themeName)) {
        theme = this.AvailableThemes.FirstOrDefault(t => t.Name.Id == themeName);
      }
      if(theme == null) {
        theme = this.AvailableThemes.FirstOrDefault();
      }
      if(theme != null) {
        this.ActiveTheme = theme;
      }
    }

    public override void SaveConfiguration(ServiceConfiguration configuration) {
      configuration.WriteValue("theme", this.activeTheme.Name.Id);
    }
  }
}
