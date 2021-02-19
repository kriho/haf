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
  public class BooleanToDouble : ValueConverter<bool, double> {
    public bool Inverted { get; set; }
    public double TrueValue { get; set; }
    public double FalseValue { get; set; }

    public BooleanToDouble() {
      this.Inverted = false;
      this.TrueValue = 1;
      this.FalseValue = 0;
    }

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
