using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF.Converters {
  public class IsNullToBooleanConverter: UnsafeValueConverter<object, bool> {
    public bool Inverted { get; set; } = false;

    protected override bool convert(object value) {
      return this.Inverted ? (value == null) : (value != null);
    }
  }
}
