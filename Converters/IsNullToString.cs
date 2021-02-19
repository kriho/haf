using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HAF.Converters {
  public class IsNullToString : UnsafeValueConverter<object, string> {
    public bool Inverted { get; set; } = false;
    public string TrueValue { get; set; } = "";
    public string FalseValue { get; set; } = "";

    protected override string convert(object value) {
      var result = value == null;
      if (this.Inverted) {
        result = !result;
      }
      return result ? this.TrueValue : this.FalseValue;
    }
  }
}
