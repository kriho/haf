using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HAF {
  public class ColorToBrushConverter: ValueConverter<Color, Brush> {
    public Color FallbackColor { get; set; } = Colors.Black;
    public byte? Alpha { get; set; } = null;

    protected override Brush convert(Color value) {
      if (value == null) {
        return new SolidColorBrush(this.FallbackColor);
      } else {
        if (this.Alpha.HasValue) {
          return new SolidColorBrush(Color.FromArgb(this.Alpha.Value, value.R, value.G, value.B));
        } else {
          return new SolidColorBrush(value);
        }
      }
    }
  }
}
