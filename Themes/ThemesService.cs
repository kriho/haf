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
    public ICompoundState MayChangeActiveTheme { get; private set; } = new CompoundState();

    public IEvent OnActiveThemeChanged { get; private set; } = new Event(nameof(OnActiveThemeChanged));

    public IObservableCollection<ITheme> AvailableThemes { get; private set; } = new ObservableCollection<ITheme>();

    public IRelayCommand DoDuplicateTheme { get; private set; }

    public IRelayCommand<ITheme> DoSetActiveTheme { get; private set; }

    public IRelayCommand<ITheme> DoDeleteTheme { get; private set; }

    private string editName = null;
    public string EditName {
      get { return this.editName; }
      set {
        if(this.SetValue(ref this.editName, value)) {
          this.DoDuplicateTheme.RaiseCanExecuteChanged();
        }
      }
    }

    private ITheme activeTheme;
    public ITheme ActiveTheme {
      get { return this.activeTheme; }
      set {
        if(!this.MayChangeActiveTheme.Value) {
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
            theme.IsActive.Value = theme == value;
          }
          this.DoDeleteTheme.RaiseCanExecuteChanged();
          this.DoSetActiveTheme.RaiseCanExecuteChanged();
          this.OnActiveThemeChanged.Fire();
        }
      }
    }

    private void ActiveTheme_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
      if(!e.PropertyName.StartsWith("Last")) {
        this.ApplyTheme(this.activeTheme, e.PropertyName);
      }
    }

    protected virtual void ApplyTheme(ITheme theme, string property = null) {
      // update resources
      if(property == null || property == "Background") {
        Application.Current.Resources["ThemeBackgroundColor"] = theme.Background;
        Application.Current.Resources["ThemeBackgroundBrush"] = new SolidColorBrush(theme.Background);
      }
      if(property == null || property == "BackgroundInfo") {
        Application.Current.Resources["ThemeBackgroundInfoColor"] = theme.BackgroundInfo;
        Application.Current.Resources["ThemeBackgroundInfoBrush"] = new SolidColorBrush(theme.BackgroundInfo);
      }
      if(property == null || property == "BackgroundWarning") {
        Application.Current.Resources["ThemeBackgroundWarningColor"] = theme.BackgroundWarning;
        Application.Current.Resources["ThemeBackgroundWarningBrush"] = new SolidColorBrush(theme.BackgroundWarning);
      }
      if(property == null || property == "BackgroundError") {
        Application.Current.Resources["ThemeBackgroundErrorColor"] = theme.BackgroundError;
        Application.Current.Resources["ThemeBackgroundErrorBrush"] = new SolidColorBrush(theme.BackgroundError);
      }
      if(property == null || property == "Control") {
        Application.Current.Resources["ThemeControlColor"] = theme.Control;
        Application.Current.Resources["ThemeControlBrush"] = new SolidColorBrush(theme.Control);
      }
      if(property == null || property == "Light") {
        Application.Current.Resources["ThemeLightColor"] = theme.Light;
        Application.Current.Resources["ThemeLightBrush"] = new SolidColorBrush(theme.Light);
      }
      if(property == null || property == "Strong") {
        Application.Current.Resources["ThemeStrongColor"] = theme.Strong;
        Application.Current.Resources["ThemeStrongBrush"] = new SolidColorBrush(theme.Strong);
      }
      if(property == null || property == "Secondary") {
        Application.Current.Resources["ThemeSecondaryColor"] = theme.Secondary;
        Application.Current.Resources["ThemeSecondaryBrush"] = new SolidColorBrush(theme.Secondary);
      }
      if(property == null || property == "InvertedSecondary") {
        Application.Current.Resources["ThemeInvertedSecondaryColor"] = theme.InvertedSecondary;
        Application.Current.Resources["ThemeInvertedSecondaryBrush"] = new SolidColorBrush(theme.InvertedSecondary);
      }
      if(property == null || property == "Text") {
        Application.Current.Resources["ThemeTextColor"] = theme.Text;
        Application.Current.Resources["ThemeTextBrush"] = new SolidColorBrush(theme.Text);
      }
      if(property == null || property == "InvertedText") {
        Application.Current.Resources["ThemeInvertedTextColor"] = theme.InvertedText;
        Application.Current.Resources["ThemeInvertedTextBrush"] = new SolidColorBrush(theme.InvertedText);
      }
      if(property == null || property == "TextInfo") {
        Application.Current.Resources["ThemeTextInfoColor"] = theme.TextInfo;
        Application.Current.Resources["ThemeTextInfoBrush"] = new SolidColorBrush(theme.TextInfo);
      }
      if(property == null || property == "TextWarning") {
        Application.Current.Resources["ThemeTextWarningColor"] = theme.TextWarning;
        Application.Current.Resources["ThemeTextWarningBrush"] = new SolidColorBrush(theme.TextWarning);
      }
      if(property == null || property == "TextError") {
        Application.Current.Resources["ThemeTextErrorColor"] = theme.TextError;
        Application.Current.Resources["ThemeTextErrorBrush"] = new SolidColorBrush(theme.TextError);
      }
      if(property == null || property == "Accent") {
        Application.Current.Resources["ThemeAccentColor"] = theme.Accent;
        Application.Current.Resources["ThemeAccentBrush"] = new SolidColorBrush(theme.Accent);
      }
      if(property == null || property == "Action") {
        Application.Current.Resources["ThemeActionColor"] = theme.Action;
        Application.Current.Resources["ThemeActionBrush"] = new SolidColorBrush(theme.Action);
      }
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
      var defaultTheme = new Theme(new LocalizedText("Light"), false) {
        Background = (Color)ColorConverter.ConvertFromString("#FFF1F1F1"),
        BackgroundInfo = (Color)ColorConverter.ConvertFromString("#FFEDF7FF"),
        BackgroundWarning = (Color)ColorConverter.ConvertFromString("#FFFFFDE2"),
        BackgroundError = (Color)ColorConverter.ConvertFromString("#FFFFE9E9"),
        Control = (Color)ColorConverter.ConvertFromString("#FFFFFFFF"),
        Light = (Color)ColorConverter.ConvertFromString("#FFE6E6E6"),
        Strong = (Color)ColorConverter.ConvertFromString("#FFABABAB"),
        Secondary = (Color)ColorConverter.ConvertFromString("#FF444444"),
        InvertedSecondary = (Color)ColorConverter.ConvertFromString("#FFE6E6E6"),
        Text = (Color)ColorConverter.ConvertFromString("#FF444444"),
        InvertedText = (Color)ColorConverter.ConvertFromString("#FFF1F1F1"),
        TextInfo = (Color)ColorConverter.ConvertFromString("#FFBDD9EE"),
        TextWarning = (Color)ColorConverter.ConvertFromString("#FFE1D555"),
        TextError = (Color)ColorConverter.ConvertFromString("#FFFF3232"),
        Accent = (Color)ColorConverter.ConvertFromString("#FF2C61AC"),
        Action = (Color)ColorConverter.ConvertFromString("#FF527FC0"),
      };
      defaultTheme.ApplyChanges();
      this.DefaultLightTheme = defaultTheme;
      defaultTheme = new Theme(new LocalizedText("Dark"), false) {
        Background = (Color)ColorConverter.ConvertFromString("#FF232323"),
        BackgroundInfo = (Color)ColorConverter.ConvertFromString("#FF5A666F"),
        BackgroundWarning = (Color)ColorConverter.ConvertFromString("#FF747044"),
        BackgroundError = (Color)ColorConverter.ConvertFromString("#FF643737"),
        Control = (Color)ColorConverter.ConvertFromString("#FF303030"),
        Light = (Color)ColorConverter.ConvertFromString("#FF414141"),
        Strong = (Color)ColorConverter.ConvertFromString("#FF5B5B5B"),
        Secondary = (Color)ColorConverter.ConvertFromString("#FF989898"),
        InvertedSecondary = (Color)ColorConverter.ConvertFromString("#FF414141"),
        Text = (Color)ColorConverter.ConvertFromString("#FFFFFFFF"),
        InvertedText = (Color)ColorConverter.ConvertFromString("#FF000000"),
        TextInfo = (Color)ColorConverter.ConvertFromString("#FF5A666F"),
        TextWarning = (Color)ColorConverter.ConvertFromString("#FF747044"),
        TextError = (Color)ColorConverter.ConvertFromString("#FF643737"),
        Accent = (Color)ColorConverter.ConvertFromString("#FF8D9FC3"),
        Action = (Color)ColorConverter.ConvertFromString("#FFA5B9DE"),
      };
      defaultTheme.ApplyChanges();
      this.DefaultDarkTheme = defaultTheme;
      this.DoSetTheme = new RelayCommand<ITheme>(theme => {
        this.ActiveTheme = theme;
      }, this.MayChangeActiveTheme);
      this.DoDuplicateTheme = new RelayCommand(() => {
        var theme = new Theme(new LocalizedText(this.editName), true) {
          Background = this.activeTheme.Background,
          BackgroundInfo = this.activeTheme.BackgroundInfo,
          BackgroundWarning = this.activeTheme.BackgroundWarning,
          BackgroundError = this.activeTheme.BackgroundError,
          Control = this.activeTheme.Control,
          Light = this.activeTheme.Light,
          Strong = this.activeTheme.Strong,
          Secondary = this.activeTheme.Secondary,
          InvertedSecondary = this.activeTheme.InvertedSecondary,
          Text = this.activeTheme.Text,
          InvertedText = this.activeTheme.InvertedText,
          TextInfo = this.activeTheme.TextInfo,
          TextWarning = this.activeTheme.TextWarning,
          TextError = this.activeTheme.TextError,
          Accent = this.activeTheme.Accent,
          Action = this.activeTheme.Action,
        };
        theme.ApplyChanges();
        this.AvailableThemes.Add(theme);
        if(this.MayChangeActiveTheme.Value) {
          this.ActiveTheme = theme;
        }
      }, () => !string.IsNullOrWhiteSpace(this.editName) && !this.AvailableThemes.Any(t => t.Name.Id == this.editName));
      this.DoSetActiveTheme = new RelayCommand<ITheme>(theme => {
        this.ActiveTheme = theme;
      }, theme => this.MayChangeActiveTheme.Value && theme != this.activeTheme);
      this.MayChangeActiveTheme.RegisterUpdate(this.DoSetActiveTheme.RaiseCanExecuteChanged);
      this.DoDeleteTheme = new RelayCommand<ITheme>(theme => {
        if(this.AvailableThemes.Contains(theme)) {
          this.AvailableThemes.Remove(theme);
        }
      }, theme => theme != null && theme.IsEditable.Value && theme != this.activeTheme);
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
          var theme = new Theme(new LocalizedText(themeEntry.ReadAttribute("name", "unnamed theme")), true) {
            Background = (Color)ColorConverter.ConvertFromString(themeEntry.ReadAttribute("background", "#FF232323")),
            BackgroundInfo = (Color)ColorConverter.ConvertFromString(themeEntry.ReadAttribute("backgroundInfo", "#FF49555E")),
            BackgroundWarning = (Color)ColorConverter.ConvertFromString(themeEntry.ReadAttribute("backgroundWarning", "#FF7D7840")),
            BackgroundError = (Color)ColorConverter.ConvertFromString(themeEntry.ReadAttribute("backgroundError", "#FF901818")),
            Control = (Color)ColorConverter.ConvertFromString(themeEntry.ReadAttribute("control", "#FF303030")),
            Light = (Color)ColorConverter.ConvertFromString(themeEntry.ReadAttribute("light", "#FF414141")),
            Strong = (Color)ColorConverter.ConvertFromString(themeEntry.ReadAttribute("strong", "#FF5B5B5B")),
            Secondary = (Color)ColorConverter.ConvertFromString(themeEntry.ReadAttribute("secondary", "#FFFFFFFF")),
            InvertedSecondary = (Color)ColorConverter.ConvertFromString(themeEntry.ReadAttribute("invertedSecondary", "#FF414141")),
            Text = (Color)ColorConverter.ConvertFromString(themeEntry.ReadAttribute("text", "#FFFFFFFF")),
            InvertedText = (Color)ColorConverter.ConvertFromString(themeEntry.ReadAttribute("invertedText", "#FF414141")),
            TextInfo = (Color)ColorConverter.ConvertFromString(themeEntry.ReadAttribute("textInfo", "#FF5A666F")),
            TextWarning = (Color)ColorConverter.ConvertFromString(themeEntry.ReadAttribute("textWarning", "#FF747044")),
            TextError = (Color)ColorConverter.ConvertFromString(themeEntry.ReadAttribute("textError", "#FF643737")),
            Accent = (Color)ColorConverter.ConvertFromString(themeEntry.ReadAttribute("accent", "#FF8D9FC3")),
            Action = (Color)ColorConverter.ConvertFromString(themeEntry.ReadAttribute("action", "#FFA5B9DE")),
          };
          theme.ApplyChanges();
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
      foreach(var theme in this.AvailableThemes.Where(t => t.IsEditable.Value)) {
        entry.WriteEntry("theme", false)
          .WriteAttribute("name", theme.Name.Id)
          .WriteAttribute("background", theme.Background.ToString())
          .WriteAttribute("backgroundInfo", theme.BackgroundInfo.ToString())
          .WriteAttribute("backgroundWarning", theme.BackgroundWarning.ToString())
          .WriteAttribute("backgroundError", theme.BackgroundError.ToString())
          .WriteAttribute("control", theme.Control.ToString())
          .WriteAttribute("light", theme.Light.ToString())
          .WriteAttribute("strong", theme.Strong.ToString())
          .WriteAttribute("secondary", theme.Secondary.ToString())
          .WriteAttribute("invertedSecondary", theme.InvertedSecondary.ToString())
          .WriteAttribute("text", theme.Text.ToString())
          .WriteAttribute("invertedText", theme.InvertedText.ToString())
          .WriteAttribute("TextInfo", theme.TextInfo.ToString())
          .WriteAttribute("TextWarning", theme.TextWarning.ToString())
          .WriteAttribute("TextError", theme.TextError.ToString())
          .WriteAttribute("accent", theme.Accent.ToString())
          .WriteAttribute("action", theme.Action.ToString());
      }
      return Task.CompletedTask;
    }
  }
}
