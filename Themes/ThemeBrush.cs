using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace HAF {
  [MarkupExtensionReturnType(typeof(Brush))]
  public class ThemeBrush: DynamicResourceExtension {

    [ConstructorArgument("key")]
    public ThemeKey Key {
      get {
        switch (this.ResourceKey) {
          case "ThemeTextBrush": return ThemeKey.Text;
          case "ThemeAccentBrush": return ThemeKey.Accent;
          case "ThemeErrorBrush": return ThemeKey.Error;
          case "ThemeWarningBrush": return ThemeKey.Warning;
          case "ThemeInfoBrush": return ThemeKey.Info;
          case "ThemeLightBrush": return ThemeKey.Light;
          case "ThemeStrongBrush": return ThemeKey.Strong;
          default: return ThemeKey.Background;
        }
      }
      set {
        this.ResourceKey = "Theme" + value.ToString() + "Brush";
      }
    }

    public ThemeBrush(ThemeKey key) {
      this.Key = key;
    }
  }
}
