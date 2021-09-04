using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HAF.Converters {
  public class OffsetPointConverter: UnsafeValueConverter<Point, Point> {
    public Vector Offset { get; set; } = new Vector();

    protected override Point convert(Point value) {
      return value + this.Offset;
    }
  }
}
