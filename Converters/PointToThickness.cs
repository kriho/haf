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
  public class PointToThickness : ValueConverter<Point, Thickness> {

    protected override Thickness convert(Point value) {
      return new Thickness(value.X, value.Y, 0, 0);
    }

    protected override Point convertBack(Thickness value) {
      return new Point(value.Left, value.Top);
    }
  }
}
