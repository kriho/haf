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
  public class DoubleToFormattedUnitConverter: ValueConverter<double, string> {
    public string Unit { get; set; } = "m";
    public double Factor { get; set; } = 1;
    public string Format { get; set; } = "{0:0.00}";

    protected override string convert(double value) {
      return string.Format(this.Format, value * this.Factor) + (this.Unit != "" ? " " + this.Unit : "");
    }
  }
}
