using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF.Converters {
  public class StringToBooleanConverter: ValueConverter<string, bool> {
    public string TrueValue { get; set; }

    public string FalseValue { get; set; }

    protected override bool convert(string value) {
      return value == this.TrueValue;
    }

    protected override string convertBack(bool value) {
      return value ? this.TrueValue : this.FalseValue;
    }
  }
}
