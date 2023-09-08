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
  public class BooleanToIntegerConverter: ValueConverter<bool, int> {
    public bool Inverted { get; set; } = false;
    public int TrueValue { get; set; } = 1;
    public int FalseValue { get; set; } = 0;

    protected override int convert(bool value) {
      if (this.Inverted) {
        value = !value;
      }
      return value ? this.TrueValue : this.FalseValue;
    }

    protected override bool convertBack(int value) {
      var result = value == TrueValue;
      return this.Inverted ? !result : result;
    }
  }
}