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
  public class BooleanToDoubleConverter: ValueConverter<bool, double> {
    public bool Inverted { get; set; } = false;
    public double TrueValue { get; set; } = 1;
    public double FalseValue { get; set; } = 0;

    protected override double convert(bool value) {
      if (this.Inverted) {
        value = !value;
      }
      return value ? this.TrueValue : this.FalseValue;
    }

    protected override bool convertBack(double value) {
      var result = value == TrueValue;
      return this.Inverted ? !result : result;
    }
  }
}
