using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HAF.Models {
  public class Theme {
    public string Name { get; set; }
    public Color BackgroundColor {get; set;}
    public Color TextColor {get; set;}
    public Color AccentColor { get; set; }
    public Color LightColor {get; set;}
    public Color StrongColor {get; set;}
    public Color InfoColor {get; set;}
    public Color WarningColor {get; set;}
    public Color ErrorColor {get; set;}
  }
}
