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
using System.Windows.Media;
using System.Xml;

namespace HAF {

  [Export(typeof(IThemesService)), PartCreationPolicy(CreationPolicy.Shared)]
  public class ThemesService : Service, IThemesService {

    public LinkedDependency MayChangeTheme { get; private set; } = new LinkedDependency();

    public LinkedEvent OnActiveThemeChanged { get; private set; } = new LinkedEvent();

    /// <summary>
    /// the list of themes that can be selected and applied
    /// </summary>
    /// <remarks>
    /// the first theme is the default theme
    /// </remarks>
    public ObservableCollection<Theme> AvailableThemes { get; private set; } = new ObservableCollection<Theme>();

    private Theme activeTheme;
    public Theme ActiveTheme {
      get { return this.activeTheme; }
      set {
        if (this.SetValue(ref this.activeTheme, value)) {
          Telerik.Windows.Controls.Windows8Palette.Palette.AccentColor = value.AccentColor;
          Telerik.Windows.Controls.Windows8Palette.Palette.MainColor = value.MainColor;
          Telerik.Windows.Controls.Windows8Palette.Palette.BasicColor = value.BasicColor;
          Telerik.Windows.Controls.Windows8Palette.Palette.StrongColor = value.StrongColor;
          Telerik.Windows.Controls.Windows8Palette.Palette.MarkerColor = value.MarkerColor;
          this.NotifyPropertyChanged(() => this.AccentColor);
          this.NotifyPropertyChanged(() => this.MainColor);
          this.NotifyPropertyChanged(() => this.BasicColor);
          this.NotifyPropertyChanged(() => this.StrongColor);
          this.NotifyPropertyChanged(() => this.MarkerColor);
          this.OnActiveThemeChanged.Fire();
        }
      }
    }

    public Color AccentColor {
      get { return this.activeTheme.AccentColor; }
    }

    public Color MainColor {
      get { return this.activeTheme.MainColor; }
    }

    public Color BasicColor {
      get { return this.activeTheme.BasicColor; }
    }

    public Color StrongColor {
      get { return this.activeTheme.StrongColor; }
    }

    public Color MarkerColor {
      get { return this.activeTheme.MarkerColor; }
    }

    public RelayCommand<Theme> _SetTheme { get; private set; }

    protected override void Initialize() {
      this._SetTheme = new RelayCommand<Theme>(theme => {
        this.ActiveTheme = theme;
      }, (theme) => {
        return theme != null && this.MayChangeTheme;
      });
      this.MayChangeTheme.RegisterUpdate(() => {
        this._SetTheme.RaiseCanExecuteChanged();
      });
      // disable touch manager for increased performance 
      Telerik.Windows.Input.Touch.TouchManager.IsTouchEnabled = false;
      // set fixed font size
      Telerik.Windows.Controls.Windows8Palette.Palette.FontSizeXS = 15;
      Telerik.Windows.Controls.Windows8Palette.Palette.FontSizeS = 15;
      Telerik.Windows.Controls.Windows8Palette.Palette.FontSize = 15;
      Telerik.Windows.Controls.Windows8Palette.Palette.FontSizeL = 15;
      Telerik.Windows.Controls.Windows8Palette.Palette.FontSizeXL = 15;
      Telerik.Windows.Controls.Windows8Palette.Palette.FontSizeXXL = 15;
      Telerik.Windows.Controls.Windows8Palette.Palette.FontSizeXXXL = 15;
      // set fonts
      Telerik.Windows.Controls.Windows8Palette.Palette.FontFamily = new FontFamily("Segoe UI");
      Telerik.Windows.Controls.Windows8Palette.Palette.FontFamilyLight = new FontFamily("Segoe UI Light");
      Telerik.Windows.Controls.Windows8Palette.Palette.FontFamilyStrong = new FontFamily("Segoe UI Semibold");
    }

    public override void LoadConfiguration(ServiceConfiguration configuration) {
      Theme theme = null;
      if (configuration.ReadStringValue("theme", out var themeName)) {
        theme = this.AvailableThemes.FirstOrDefault(t => t.Name == themeName);
      } else {
        theme = this.AvailableThemes.FirstOrDefault();
      }
      if(theme != null) {
        this.ActiveTheme = theme;
      }
    }

    public override void SaveConfiguration(ServiceConfiguration configuration) {
      configuration.WriteValue("theme", this.activeTheme.Name);
    }
  }
}
