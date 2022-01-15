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
  [MarkupExtensionReturnType(typeof(Color))]
  public class ThemeColor: DynamicResourceExtension {

    [ConstructorArgument("key")]
    public ThemeKey Key {
      get {
        switch (this.ResourceKey) {
          case "ThemeControlColor": return ThemeKey.Control;
          case "ThemeTextColor": return ThemeKey.Text;
          case "ThemeAccentColor": return ThemeKey.Accent;
          case "ThemeActionColor": return ThemeKey.Action;
          case "ThemeLightColor": return ThemeKey.Light;
          case "ThemeStrongColor": return ThemeKey.Strong;
          case "ThemeInfoForegroundColor": return ThemeKey.InfoForeground;
          case "ThemeInfoBackgroundColor": return ThemeKey.InfoBackground;
          case "ThemeWarningForegroundColor": return ThemeKey.WarningForeground;
          case "ThemeWarningBackgroundColor": return ThemeKey.WarningBackground;
          case "ThemeErrorForegroundColor": return ThemeKey.ErrorForeground;
          case "ThemeErrorBackgroundColor": return ThemeKey.ErrorBackground;
          default: return ThemeKey.Background;
        }
      }
      set {
        this.ResourceKey = "Theme" + value.ToString() + "Color";
      }
    }

    public ThemeColor(ThemeKey key) {
      this.Key = key;
    }
  }
}
