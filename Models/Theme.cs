using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HAF.Models {
  public class Theme {
    public string Name { get; set; }
    public Color AccentColor { get; set; }
    public Color MainColor {get; set;}
    public Color BasicColor {get; set;}
    public Color StrongColor {get; set;}
    public Color MarkerColor {get; set;}
  }
}
