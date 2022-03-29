using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HAF.Converters {
  public abstract class RadioButtonConverter<T>: UnsafeValueConverter<object, bool>{
    public T Value { get; set; }

    protected override bool convert(object obj) {
      var success = obj is T value && value.Equals(this.Value);
      return success;
    }

    protected override object convertBack(bool value) {
      return value ? this.Value : Binding.DoNothing;
    }
  }
}
