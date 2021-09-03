using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HAF {
  public class PointBlender: MultiValueConverter<Point, Point> {

    public double BlendX { get; set; } = 0.5;
    public double BlendY { get; set; } = 0.5;

    protected override Point convert(IEnumerable<Point> values) {
      if (values.Count() != 2) {
        return new Point();
      }
      return new Point(values.ElementAt(0).X + (values.ElementAt(1).X - values.ElementAt(0).X) * this.BlendX, values.ElementAt(0).Y + (values.ElementAt(1).Y - values.ElementAt(0).Y) * this.BlendY);
    }
  }
}
