using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HAF {
  public class Theme: ObservableObject, ITheme {
    public LocalizedText Name { get; set; }

    public bool IsEditable { get; private set; }

    private State isDirty = new State(false);
    public IReadOnlyState IsDirty => this.isDirty;

    public IRelayCommand DoApplyChanges { get; private set; }

    private Color lastBackgroundColor;
    public Color LastBackgroundColor {
      get => this.lastBackgroundColor;
      set => this.SetValue(ref this.lastBackgroundColor, value);
    }

    private Color backgroundColor;
    public Color BackgroundColor {
      get => this.backgroundColor;
      set {
        if(this.SetValue(ref this.backgroundColor, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private Color lastControlColor;
    public Color LastControlColor {
      get => this.lastControlColor;
      set => this.SetValue(ref this.lastControlColor, value);
    }

    private Color controlColor;
    public Color ControlColor {
      get => this.controlColor;
      set {
        if(this.SetValue(ref this.controlColor, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private Color lastLightColor;
    public Color LastLightColor {
      get => this.lastLightColor;
      set => this.SetValue(ref this.lastLightColor, value);
    }

    private Color lightColor;
    public Color LightColor {
      get => this.lightColor;
      set {
        if(this.SetValue(ref this.lightColor, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private Color lastStrongColor;
    public Color LastStrongColor {
      get => this.lastStrongColor;
      set => this.SetValue(ref this.lastStrongColor, value);
    }

    private Color strongColor;
    public Color StrongColor {
      get => this.strongColor;
      set {
        if(this.SetValue(ref this.strongColor, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private Color lastSecondaryColor;
    public Color LastSecondaryColor {
      get => this.lastSecondaryColor;
      set => this.SetValue(ref this.lastSecondaryColor, value);
    }

    private Color secondaryColor;
    public Color SecondaryColor {
      get => this.secondaryColor;
      set {
        if(this.SetValue(ref this.secondaryColor, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private Color lastTextColor;
    public Color LastTextColor {
      get => this.lastTextColor;
      set => this.SetValue(ref this.lastTextColor, value);
    }

    private Color textColor;
    public Color TextColor {
      get => this.textColor;
      set {
        if(this.SetValue(ref this.textColor, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private Color lastAccentColor;
    public Color LastAccentColor {
      get => this.lastAccentColor;
      set => this.SetValue(ref this.lastAccentColor, value);
    }

    private Color accentColor;
    public Color AccentColor {
      get => this.accentColor;
      set {
        if(this.SetValue(ref this.accentColor, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private Color lastActionColor;
    public Color LastActionColor {
      get => this.lastActionColor;
      set => this.SetValue(ref this.lastActionColor, value);
    }

    private Color actionColor;
    public Color ActionColor {
      get => this.actionColor;
      set {
        if(this.SetValue(ref this.actionColor, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private Color lastInfoForegroundColor;
    public Color LastInfoForegroundColor {
      get => this.lastInfoForegroundColor;
      set => this.SetValue(ref this.lastInfoForegroundColor, value);
    }

    private Color infoForegroundColor;
    public Color InfoForegroundColor {
      get => this.infoForegroundColor;
      set {
        if(this.SetValue(ref this.infoForegroundColor, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private Color lastInfoBackgroundColor;
    public Color LastInfoBackgroundColor {
      get => this.lastInfoBackgroundColor;
      set => this.SetValue(ref this.lastInfoBackgroundColor, value);
    }

    private Color infoBackgroundColor;
    public Color InfoBackgroundColor {
      get => this.infoBackgroundColor;
      set {
        if(this.SetValue(ref this.infoBackgroundColor, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private Color lastWarningForegroundColor;
    public Color LastWarningForegroundColor {
      get => this.lastWarningForegroundColor;
      set => this.SetValue(ref this.lastWarningForegroundColor, value);
    }

    private Color warningForegroundColor;
    public Color WarningForegroundColor {
      get => this.warningForegroundColor;
      set {
        if(this.SetValue(ref this.warningForegroundColor, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private Color lastWarningBackgroundColor;
    public Color LastWarningBackgroundColor {
      get => this.lastWarningBackgroundColor;
      set => this.SetValue(ref this.lastWarningBackgroundColor, value);
    }

    private Color warningBackgroundColor;
    public Color WarningBackgroundColor {
      get => this.warningBackgroundColor;
      set {
        if(this.SetValue(ref this.warningBackgroundColor, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private Color lastErrorForegroundColor;
    public Color LastErrorForegroundColor {
      get => this.lastErrorForegroundColor;
      set => this.SetValue(ref this.lastErrorForegroundColor, value);
    }

    private Color errorForegroundColor;
    public Color ErrorForegroundColor {
      get => this.errorForegroundColor;
      set {
        if(this.SetValue(ref this.errorForegroundColor, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private Color lastErrorBackgroundColor;
    public Color LastErrorBackgroundColor {
      get => this.lastErrorBackgroundColor;
      set => this.SetValue(ref this.lastErrorBackgroundColor, value);
    }

    private Color errorBackgroundColor;
    public Color ErrorBackgroundColor {
      get => this.errorBackgroundColor;
      set {
        if(this.SetValue(ref this.errorBackgroundColor, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private bool isActive;
    public bool IsActive {
      get => this.isActive;
      set => this.SetValue(ref this.isActive, value);
    }

    public void ApplyColorChanges() {
      this.LastBackgroundColor = this.BackgroundColor;
      this.LastControlColor = this.ControlColor;
      this.LastLightColor = this.LightColor;
      this.LastStrongColor = this.StrongColor;
      this.LastSecondaryColor = this.SecondaryColor;
      this.LastTextColor = this.TextColor;
      this.LastAccentColor = this.AccentColor;
      this.LastActionColor = this.ActionColor;
      this.LastInfoForegroundColor = this.InfoForegroundColor;
      this.LastInfoBackgroundColor = this.InfoBackgroundColor;
      this.LastWarningForegroundColor = this.WarningForegroundColor;
      this.LastWarningBackgroundColor = this.WarningBackgroundColor;
      this.LastErrorForegroundColor = this.ErrorForegroundColor;
      this.LastErrorBackgroundColor = this.ErrorBackgroundColor;
      this.isDirty.Value = false;
    }

    public Theme(IThemesService themesService, bool isEditable) {
      this.IsEditable = isEditable;
      this.DoApplyChanges = new RelayCommand(() => {
        this.ApplyColorChanges();
      }, this.isDirty);
    }
  }
}
