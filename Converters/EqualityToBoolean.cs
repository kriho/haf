using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public class EqualityToBooleanConverter: UnsafeValueConverter<object, bool> {
    public bool Inverted { get; set; } = false;
    public object Other { get; set; } = null;

    protected override bool convert(object value) {
      return this.Inverted ? value.Equals(this.Other) : !value.Equals(this.Other);
    }
  }
}
