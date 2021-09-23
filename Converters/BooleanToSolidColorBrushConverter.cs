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
  public class BooleanToSolidColorBrushConverter: ValueConverter<bool, SolidColorBrush> {
    public Color TrueValue { get; set; }
    public Color FalseValue { get; set; }

    protected override SolidColorBrush convert(bool value) {
      return new SolidColorBrush(value ? this.TrueValue : this.FalseValue);
    }
  }
}
