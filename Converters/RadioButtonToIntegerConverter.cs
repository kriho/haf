using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF.Converters {
  public class RadioButtonToIntegerConverter: UnsafeValueConverter<int, bool> {
    public int Value { get; set; } = 0;

    protected override bool convert(int value) {
      return value == this.Value;
    }

    protected override int convertBack(bool value) {
      return this.Value;
    }
  }
}
