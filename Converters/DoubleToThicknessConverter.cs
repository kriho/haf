using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace HAF.Converters {
  public class DoubleToThicknessConverter: ValueConverter<double, Thickness> {
    public bool Left { get; set; } = false;
    public bool Top { get; set; } = false;
    public bool Right { get; set; } = false;
    public bool Bottom { get; set; } = false;

    protected override Thickness convert(double value) {
      return new Thickness() {
        Left = this.Left ? value : 0,
        Top = this.Top ? value : 0,
        Right = this.Right ? value : 0,
        Bottom = this.Bottom ? value : 0,
      };
    }
  }
}
