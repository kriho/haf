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
  public class ThemeBrush: MarkupExtension {

    public  ThemeKey Key { get; set; }

    public ThemeBrush() {
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
      return ThemeColor.ThemesService?.GetBrush(this.Key) ?? new SolidColorBrush(ThemeColor.GetDefaultColor(this.Key));
    }
  }
}
