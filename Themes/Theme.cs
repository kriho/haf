using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HAF {
  public class Theme: ObservableObject, ITheme {
    public LocalizedText Name { get; private set; }

    public IReadOnlyState IsEditable { get; private set; }

    private State isDirty = new State(false);
    public IReadOnlyState IsDirty => this.isDirty;

    public IState IsActive { get; private set; } = new State(false);

    public IRelayCommand DoApplyChanges { get; private set; }
    
    private double disabledOpacity;
    public double DisabledOpacity {
      get => this.disabledOpacity;
      set => this.SetValue(ref this.disabledOpacity, value);
    }

    private Color lastBackground;
    public Color LastBackground {
      get => this.lastBackground;
      set => this.SetValue(ref this.lastBackground, value);
    }

    private Color background;
    public Color Background {
      get => this.background;
      set {
        if(this.SetValue(ref this.background, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private Color lastBackgroundInfo;
    public Color LastBackgroundInfo {
      get => this.lastBackground;
      set => this.SetValue(ref this.lastBackgroundInfo, value);
    }

    private Color backgroundInfo;
    public Color BackgroundInfo {
      get => this.backgroundInfo;
      set {
        if(this.SetValue(ref this.backgroundInfo, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private Color lastBackgroundWarning;
    public Color LastBackgroundWarning {
      get => this.lastBackgroundWarning;
      set => this.SetValue(ref this.lastBackgroundWarning, value);
    }

    private Color backgroundWarning;
    public Color BackgroundWarning {
      get => this.backgroundWarning;
      set {
        if(this.SetValue(ref this.backgroundWarning, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private Color lastBackgroundError;
    public Color LastBackgroundError {
      get => this.lastBackgroundError;
      set => this.SetValue(ref this.lastBackgroundError, value);
    }

    private Color backgroundError;
    public Color BackgroundError {
      get => this.backgroundError;
      set {
        if(this.SetValue(ref this.backgroundError, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private Color lastControl;
    public Color LastControl {
      get => this.lastControl;
      set => this.SetValue(ref this.lastControl, value);
    }

    private Color control;
    public Color Control {
      get => this.control;
      set {
        if(this.SetValue(ref this.control, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private Color lastLight;
    public Color LastLight {
      get => this.lastLight;
      set => this.SetValue(ref this.lastLight, value);
    }

    private Color light;
    public Color Light {
      get => this.light;
      set {
        if(this.SetValue(ref this.light, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private Color lastStrong;
    public Color LastStrong {
      get => this.lastStrong;
      set => this.SetValue(ref this.lastStrong, value);
    }

    private Color strong;
    public Color Strong {
      get => this.strong;
      set {
        if(this.SetValue(ref this.strong, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private Color lastSecondary;
    public Color LastSecondary {
      get => this.lastSecondary;
      set => this.SetValue(ref this.lastSecondary, value);
    }

    private Color secondary;
    public Color Secondary {
      get => this.secondary;
      set {
        if(this.SetValue(ref this.secondary, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private Color lastInvertedSecondary;
    public Color LastInvertedSecondary {
      get => this.lastInvertedSecondary;
      set => this.SetValue(ref this.lastInvertedSecondary, value);
    }

    private Color invertedSecondary;
    public Color InvertedSecondary {
      get => this.invertedSecondary;
      set {
        if(this.SetValue(ref this.invertedSecondary, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private Color lastText;
    public Color LastText {
      get => this.lastText;
      set => this.SetValue(ref this.lastText, value);
    }

    private Color text;
    public Color Text {
      get => this.text;
      set {
        if(this.SetValue(ref this.text, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private Color lastInvertedText;
    public Color LastInvertedText {
      get => this.lastInvertedText;
      set => this.SetValue(ref this.lastInvertedText, value);
    }

    private Color invertedText;
    public Color InvertedText {
      get => this.invertedText;
      set {
        if(this.SetValue(ref this.invertedText, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private Color lastTextInfo;
    public Color LastTextInfo {
      get => this.lastTextInfo;
      set => this.SetValue(ref this.lastTextInfo, value);
    }

    private Color textInfo;
    public Color TextInfo {
      get => this.textInfo;
      set {
        if(this.SetValue(ref this.textInfo, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private Color lastTextWarning;
    public Color LastTextWarning {
      get => this.lastTextWarning;
      set => this.SetValue(ref this.lastTextWarning, value);
    }

    private Color textWarning;
    public Color TextWarning {
      get => this.textWarning;
      set {
        if(this.SetValue(ref this.textWarning, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private Color lastTextError;
    public Color LastTextError {
      get => this.lastTextError;
      set => this.SetValue(ref this.lastTextError, value);
    }

    private Color textError;
    public Color TextError {
      get => this.textError;
      set {
        if(this.SetValue(ref this.textError, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private Color lastAccent;
    public Color LastAccent {
      get => this.lastAccent;
      set => this.SetValue(ref this.lastAccent, value);
    }

    private Color accent;
    public Color Accent {
      get => this.accent;
      set {
        if(this.SetValue(ref this.accent, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    private Color lastAction;
    public Color LastAction {
      get => this.lastAction;
      set => this.SetValue(ref this.lastAction, value);
    }

    private Color action;
    public Color Action {
      get => this.action;
      set {
        if(this.SetValue(ref this.action, value)) {
          this.isDirty.Value = true;
        }
      }
    }

    public void ApplyChanges() {
      this.LastBackground = this.Background;
      this.LastBackgroundInfo = this.BackgroundInfo;
      this.LastBackgroundWarning = this.BackgroundWarning;
      this.LastBackgroundError = this.BackgroundError;
      this.LastControl = this.Control;
      this.LastLight = this.Light;
      this.LastStrong = this.Strong;
      this.LastSecondary = this.Secondary;
      this.LastInvertedSecondary = this.InvertedSecondary;
      this.LastText = this.Text;
      this.LastInvertedText = this.InvertedText;
      this.LastTextInfo = this.TextInfo;
      this.LastTextWarning = this.TextWarning;
      this.LastTextError = this.TextError;
      this.LastAccent = this.Accent;
      this.LastAction = this.Action;
      this.isDirty.Value = false;
    }

    public Theme(LocalizedText name, bool isEditable) {
      this.Name = name;
      this.IsEditable = new FixedState(isEditable);
      this.DoApplyChanges = new RelayCommand(() => {
        this.ApplyChanges();
      }, this.isDirty);
    }
  }
}
