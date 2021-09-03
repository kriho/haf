using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HAF {
  public class ObjectToIntegerConverter: UnsafeValueConverter<object, int> {
    public int FallbackValue { get; set; } = 0;

    protected override int convert(object value) {
      if (int.TryParse(value.ToString(), out var parsedValue)) {
        return parsedValue;
      } else {
        return this.FallbackValue;
      }
    }
  }
}
