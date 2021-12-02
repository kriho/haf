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
  public class ThemesService: Service, IThemesService {
    public ICompoundState CanChangeTheme { get; private set; } = new CompoundState();

    public IEvent OnActiveThemeChanged { get; private set; } = new Event(nameof(OnActiveThemeChanged));

    public IObservableCollection<ITheme> AvailableThemes { get; private set; } = new ObservableCollection<ITheme>();

    public IRelayCommand DoDuplicateTheme { get; private set; }

    public IRelayCommand<ITheme> DoDeleteTheme { get; private set; }

    private ITheme activeTheme;
    public ITheme ActiveTheme {
      get { return this.activeTheme; }
      set {
        if(!this.CanChangeTheme.Value) {
          // theme changing is not allowed
          return;
        }
        if(this.activeTheme != null) {
          // unregister change callback
          this.activeTheme.PropertyChanged -= this.ActiveTheme_PropertyChanged;
        }
        if(this.SetValue(ref this.activeTheme, value)) {
          // apply theme
          if(value != null) {
            this.ApplyTheme(value);
            // register change callback
            value.PropertyChanged += this.ActiveTheme_PropertyChanged;
          }
          // update active state
          foreach(var theme in this.AvailableThemes) {
            theme.IsActive = theme == value;
          }
          this.OnActiveThemeChanged.Fire();
          this.DoDeleteTheme.RaiseCanExecuteChanged();
        }
      }
    }

    private void ActiveTheme_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
      if(e.PropertyName.EndsWith("Color") && !e.PropertyName.StartsWith("Last")) {
        this.ApplyTheme(this.activeTheme, e.PropertyName);
      }
    }

    private string editName = null;
    public string EditName {
      get { return this.editName; }
      set {
        if(this.SetValue(ref this.editName, value)) {
          this.DoDuplicateTheme.RaiseCanExecuteChanged();
        }
      }
    }

    protected virtual void ApplyTheme(ITheme theme, string property = null) {
      // update resources
      if(property == null || property == "BackgroundColor") {
        Application.Current.Resources["ThemeBackgroundColor"] = theme.BackgroundColor;
        Application.Current.Resources["ThemeBackgroundBrush"] = new SolidColorBrush(theme.BackgroundColor);
      }
      if(property == null || property == "ControlColor") {
        Application.Current.Resources["ThemeControlColor"] = theme.ControlColor;
        Application.Current.Resources["ThemeControlBrush"] = new SolidColorBrush(theme.ControlColor);
      }

      if(property == null || property == "LightColor") {
        Application.Current.Resources["ThemeLightColor"] = theme.LightColor;
        Application.Current.Resources["ThemeLightBrush"] = new SolidColorBrush(theme.LightColor);
      }
      if(property == null || property == "StrongColor") {
        Application.Current.Resources["ThemeStrongColor"] = theme.StrongColor;
        Application.Current.Resources["ThemeStrongBrush"] = new SolidColorBrush(theme.StrongColor);
      }
      if(property == null || property == "SecondaryColor") {
        Application.Current.Resources["ThemeSecondaryColor"] = theme.SecondaryColor;
        Application.Current.Resources["ThemeSecondaryBrush"] = new SolidColorBrush(theme.SecondaryColor);
      }
      if(property == null || property == "TextColor") {
        Application.Current.Resources["ThemeTextColor"] = theme.TextColor;
        Application.Current.Resources["ThemeTextBrush"] = new SolidColorBrush(theme.TextColor);
      }
      if(property == null || property == "ActionColor") {
        Application.Current.Resources["ThemeActionColor"] = theme.ActionColor;
        Application.Current.Resources["ThemeActionBrush"] = new SolidColorBrush(theme.ActionColor);
      }
      if(property == null || property == "AccentColor") {
        Application.Current.Resources["ThemeAccentColor"] = theme.AccentColor;
        Application.Current.Resources["ThemeAccentBrush"] = new SolidColorBrush(theme.AccentColor);
      }
      if(property == null || property == "InfoColor") {
        Application.Current.Resources["ThemeInfoColor"] = theme.InfoColor;
        Application.Current.Resources["ThemeInfoBrush"] = new SolidColorBrush(theme.InfoColor);
      }
      if(property == null || property == "WarningColor") {
        Application.Current.Resources["ThemeWarningColor"] = theme.WarningColor;
        Application.Current.Resources["ThemeWarningBrush"] = new SolidColorBrush(theme.WarningColor);
      }
      if(property == null || property == "ErrorColor") {
        Application.Current.Resources["ThemeErrorColor"] = theme.ErrorColor;
        Application.Current.Resources["ThemeErrorBrush"] = new SolidColorBrush(theme.ErrorColor);
      }
    }

    public Color GetColor(ThemeKey key) {
      switch(key) {
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

    public void UpdateTheme(ITheme theme) {
      if(theme == this.activeTheme) {
        this.ApplyTheme(this.activeTheme);
      }
    }

    public IRelayCommand<ITheme> DoSetTheme { get; private set; }

    public ITheme DefaultLightTheme { get; private set; }

    public ITheme DefaultDarkTheme { get; private set; }

    public ThemesService() {
      var defaultTheme = new Theme(this, false) {
        Name = new LocalizedText("Light"),
        BackgroundColor = (Color)ColorConverter.ConvertFromString("#FFF1F1F1"),
        ControlColor = (Color)ColorConverter.ConvertFromString("#FFFFFFFF"),
        LightColor = (Color)ColorConverter.ConvertFromString("#FFE6E6E6"),
        StrongColor = (Color)ColorConverter.ConvertFromString("#FFABABAB"),
        SecondaryColor = (Color)ColorConverter.ConvertFromString("#FF444444"),
        TextColor = (Color)ColorConverter.ConvertFromString("#FF444444"),
        AccentColor = (Color)ColorConverter.ConvertFromString("#FF2C61AC"),
        ActionColor = (Color)ColorConverter.ConvertFromString("#FF527FC0"),
        InfoColor = (Color)ColorConverter.ConvertFromString("#FFEDF7FF"),
        WarningColor = (Color)ColorConverter.ConvertFromString("#FFFFFDE2"),
        ErrorColor = (Color)ColorConverter.ConvertFromString("#FFFFE9E9"),
      };
      defaultTheme.ApplyColorChanges();
      this.DefaultLightTheme = defaultTheme;
      defaultTheme = new Theme(this, false) {
        Name = new LocalizedText("Dark"),
        BackgroundColor = (Color)ColorConverter.ConvertFromString("#FF232323"),
        ControlColor = (Color)ColorConverter.ConvertFromString("#FF303030"),
        LightColor = (Color)ColorConverter.ConvertFromString("#FF414141"),
        StrongColor = (Color)ColorConverter.ConvertFromString("#FF5B5B5B"),
        SecondaryColor = (Color)ColorConverter.ConvertFromString("#FF989898"),
        TextColor = (Color)ColorConverter.ConvertFromString("#FFFFFFFF"),
        AccentColor = (Color)ColorConverter.ConvertFromString("#FF8D9FC3"),
        ActionColor = (Color)ColorConverter.ConvertFromString("#FFA5B9DE"),
        InfoColor = (Color)ColorConverter.ConvertFromString("#FF5A666F"),
        WarningColor = (Color)ColorConverter.ConvertFromString("#FF747044"),
        ErrorColor = (Color)ColorConverter.ConvertFromString("#FF643737"),
      };
      defaultTheme.ApplyColorChanges();
      this.DefaultDarkTheme = defaultTheme;
      this.DoSetTheme = new RelayCommand<ITheme>(theme => {
        this.ActiveTheme = theme;
      }, this.CanChangeTheme);
      this.DoDuplicateTheme = new RelayCommand(() => {
        var theme = new Theme(this, true) {
          Name = new LocalizedText(this.editName),
          BackgroundColor = this.activeTheme.BackgroundColor,
          ControlColor = this.activeTheme.ControlColor,
          LightColor = this.activeTheme.LightColor,
          StrongColor = this.activeTheme.StrongColor,
          SecondaryColor = this.activeTheme.SecondaryColor,
          TextColor = this.activeTheme.TextColor,
          AccentColor = this.activeTheme.AccentColor,
          ActionColor = this.activeTheme.ActionColor,
          InfoColor = this.activeTheme.InfoColor,
          WarningColor = this.activeTheme.WarningColor,
          ErrorColor = this.activeTheme.ErrorColor,
        };
        theme.ApplyColorChanges();
        this.AvailableThemes.Add(theme);
        this.ActiveTheme = theme;
      }, () => !string.IsNullOrWhiteSpace(this.editName) && !this.AvailableThemes.Any(t => t.Name.Id == this.editName));
      this.DoDeleteTheme = new RelayCommand<ITheme>(theme => {
        if(this.AvailableThemes.Contains(theme)) {
          this.AvailableThemes.Remove(theme);
        }
      }, theme => theme != null && theme.IsEditable && theme != this.activeTheme);
#if DEBUG
      if(this.IsInDesignMode) {
        this.AvailableThemes.Add(this.DefaultLightTheme);
        this.AvailableThemes.Add(this.DefaultDarkTheme);
        this.ActiveTheme = this.DefaultLightTheme;
      }
#endif
    }

    public override Task LoadConfiguration(ServiceConfiguration configuration) {
      ITheme activeTheme = null;
      if(configuration.TryReadEntry("themes", out var entry)) {
        foreach(var themeEntry in entry.ReadEntries("theme")) {
          var theme = new Theme(this, true) {
            Name = new LocalizedText(themeEntry.ReadAttribute("name", "unnamed theme")),
            BackgroundColor = (Color)ColorConverter.ConvertFromString(themeEntry.ReadAttribute("background", "#FF232323")),
            ControlColor = (Color)ColorConverter.ConvertFromString(themeEntry.ReadAttribute("control", "#FF303030")),
            LightColor = (Color)ColorConverter.ConvertFromString(themeEntry.ReadAttribute("light", "#FF414141")),
            StrongColor = (Color)ColorConverter.ConvertFromString(themeEntry.ReadAttribute("strong", "#FF5B5B5B")),
            SecondaryColor = (Color)ColorConverter.ConvertFromString(themeEntry.ReadAttribute("secondary", "#FFFFFFFF")),
            TextColor = (Color)ColorConverter.ConvertFromString(themeEntry.ReadAttribute("text", "#FFFFFFFF")),
            AccentColor = (Color)ColorConverter.ConvertFromString(themeEntry.ReadAttribute("accent", "#FF8D9FC3")),
            ActionColor = (Color)ColorConverter.ConvertFromString(themeEntry.ReadAttribute("action", "#FFA5B9DE")),
            InfoColor = (Color)ColorConverter.ConvertFromString(themeEntry.ReadAttribute("info", "#FF49555E")),
            WarningColor = (Color)ColorConverter.ConvertFromString(themeEntry.ReadAttribute("warning", "#FF7D7840")),
            ErrorColor = (Color)ColorConverter.ConvertFromString(themeEntry.ReadAttribute("error", "#FF901818")),
          };
          theme.ApplyColorChanges();
          this.AvailableThemes.Add(theme);
        }
        if(entry.TryReadAttribute("default", out string themeName)) {
          activeTheme = this.AvailableThemes.FirstOrDefault(t => t.Name.Id == themeName);
        }
      }
      if(activeTheme == null) {
        activeTheme = this.AvailableThemes.FirstOrDefault();
      }
      if(activeTheme != null) {
        this.ActiveTheme = activeTheme;
      }
      return Task.CompletedTask;
    }

    public override Task SaveConfiguration(ServiceConfiguration configuration) {
      var entry = configuration.WriteEntry("themes", true)
        .WriteAttribute("default", this.activeTheme.Name.Id);
      foreach(var theme in this.AvailableThemes.Where(t => t.IsEditable)) {
        entry.WriteEntry("theme", false)
          .WriteAttribute("name", theme.Name.Id)
          .WriteAttribute("background", theme.BackgroundColor.ToString())
          .WriteAttribute("control", theme.ControlColor.ToString())
          .WriteAttribute("light", theme.LightColor.ToString())
          .WriteAttribute("strong", theme.StrongColor.ToString())
          .WriteAttribute("secondary", theme.SecondaryColor.ToString())
          .WriteAttribute("text", theme.TextColor.ToString())
          .WriteAttribute("accent", theme.AccentColor.ToString())
          .WriteAttribute("action", theme.ActionColor.ToString())
          .WriteAttribute("info", theme.InfoColor.ToString())
          .WriteAttribute("warning", theme.WarningColor.ToString())
          .WriteAttribute("error", theme.ErrorColor.ToString());
      }
      return Task.CompletedTask;
    }
  }
}
