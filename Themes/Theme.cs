using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HAF {
  public class Theme: ITheme {
    public LocalizedText Name { get; set; }
    public Color BackgroundColor { get; set; }
    public Color ControlColor { get; set; }
    public Color TextColor { get; set; }
    public Color AccentColor { get; set; }
    public Color ActionColor { get; set; }
    public Color LightColor { get; set; }
    public Color StrongColor { get; set; }
    public Color InfoColor { get; set; }
    public Color WarningColor { get; set; }
    public Color ErrorColor { get; set; }
  }
}
