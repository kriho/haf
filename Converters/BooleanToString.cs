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
  public class BooleanToString : ValueConverter<bool, string> {
    public bool Inverted { get; set; }
    public string TrueValue { get; set; }
    public string FalseValue { get; set; }

    public BooleanToString() {
      this.Inverted = false;
      this.TrueValue = "";
      this.FalseValue = "";
    }

    protected override string convert(bool value) {
      if (this.Inverted) {
        value = !value;
      }
      return value ? this.TrueValue : this.FalseValue;
    }

    protected override bool convertBack(string value) {
      var result = (value == TrueValue);
      if (this.Inverted) {
        result = !result;
      }
      return result;
    }
  }
}
