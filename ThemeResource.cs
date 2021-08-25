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
  public class ThemeResource: MarkupExtension {

    public ThemeKey Key { get; set; }

    private static IThemesService themesService;

    public ThemeResource() {
#if DEBUG
      if (ObservableObject.IsInDesignModeStatic) {
        return;
      }
#endif
      if (ThemeResource.themesService == null) {
        ThemeResource.themesService = Configuration.Container.GetExportedValue<IThemesService>();
      }
    }

    public override object ProvideValue(IServiceProvider serviceProvider) {
      Type targetType = null;
      if (serviceProvider != null) {
        if (serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget target) {
          if (target.TargetProperty is PropertyInfo clrProp)
            targetType = clrProp.PropertyType;
          if (targetType == null) {
            if (target.TargetProperty is DependencyProperty dp)
              targetType = dp.PropertyType;
          }
        }
      }
      if(targetType == typeof(string)) {
        return this.Key;
      }
      if (targetType == typeof(Brush)) {
        return ThemeResource.themesService?.GetBrush(this.Key) ?? new SolidColorBrush();
      }
      if (targetType == typeof(Color)) {
        return ThemeResource.themesService?.GetColor(this.Key) ?? new Color();
      }
      throw new Exception($"theme resource \"{this.Key}\" can not be converted to {targetType.Name}");
    }
  }
}
