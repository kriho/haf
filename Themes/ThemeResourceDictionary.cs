using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HAF {
  public class ThemeResourceDictionary: ResourceDictionary {

    private static ThemeResourceDictionary Instance;

    public ThemeResourceDictionary() {
      Instance = this;
    }

    public static void ApplyTheme(Models.ITheme theme) {
      Instance["ThemeBackgroundColor"] = theme.BackgroundColor;
      Instance["ThemeTextColor"] = theme.TextColor;
      Instance["ThemeAccentColor"] = theme.AccentColor;
      Instance["ThemeLightColor"] = theme.LightColor;
      Instance["ThemeStrongColor"] = theme.StrongColor;
      Instance["ThemeInfoColor"] = theme.InfoColor;
      Instance["ThemeWarningColor"] = theme.WarningColor;
      Instance["ThemeErrorColor"] = theme.ErrorColor;
    }
  }
}
