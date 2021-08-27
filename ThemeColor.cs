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
  public class ThemeColor: MarkupExtension {

    public ThemeKey Key { get; set; }

    internal static IThemesService ThemesService;

    internal static Color GetDefaultColor(ThemeKey key) {
      switch (key) {
        case ThemeKey.Accent: return (Color)ColorConverter.ConvertFromString("#FF0B70BB");
        case ThemeKey.Background: return (Color)ColorConverter.ConvertFromString("#FFFFFFFF");
        case ThemeKey.Light: return (Color)ColorConverter.ConvertFromString("#FFDFDFDF");
        case ThemeKey.Strong: return (Color)ColorConverter.ConvertFromString("#FF7E7E7E");
        case ThemeKey.Info: return (Color)ColorConverter.ConvertFromString("#FFD3EBFC");
        case ThemeKey.Warning: return (Color)ColorConverter.ConvertFromString("#FFFFFAC3");
        case ThemeKey.Error: return (Color)ColorConverter.ConvertFromString("#FFFFD2D2");
        default: return (Color)ColorConverter.ConvertFromString("#FF000000");
      }
    }

    public ThemeColor() {
      if (ThemeColor.ThemesService == null) {
        try {
          ThemeColor.ThemesService = Configuration.Container.GetExportedValue<IThemesService>();
        } catch (Exception e) {
          if (!ObservableObject.IsInDesignModeStatic) {
            throw e;
          }
        }
      }
    }

    public override object ProvideValue(IServiceProvider serviceProvider) {
      return ThemeColor.ThemesService?.GetColor(this.Key) ?? ThemeColor.GetDefaultColor(this.Key);
    }
  }
}
