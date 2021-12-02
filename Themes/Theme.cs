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

    private Color lastInfoColor;
    public Color LastInfoColor {
      get => this.lastInfoColor;
      set => this.SetValue(ref this.lastInfoColor, value);
    }

    private Color infoColor;
    public Color InfoColor {
      get => this.infoColor;
      set {
        if(this.SetValue(ref this.infoColor, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private Color lastWarningColor;
    public Color LastWarningColor {
      get => this.lastWarningColor;
      set => this.SetValue(ref this.lastWarningColor, value);
    }

    private Color warningColor;
    public Color WarningColor {
      get => this.warningColor;
      set {
        if(this.SetValue(ref this.warningColor, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private Color lastErrorColor;
    public Color LastErrorColor {
      get => this.lastErrorColor;
      set => this.SetValue(ref this.lastErrorColor, value);
    }

    private Color errorColor;
    public Color ErrorColor {
      get => this.errorColor;
      set {
        if(this.SetValue(ref this.errorColor, value)) {
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
      this.LastInfoColor = this.InfoColor;
      this.LastWarningColor = this.WarningColor;
      this.LastErrorColor = this.ErrorColor;
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
