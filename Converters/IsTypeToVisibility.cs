using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HAF {
  public class IsTypeToVisibilityConverter: UnsafeValueConverter<object, Visibility> {
    public bool Inverted { get; set; } = false;
    public Type Type { get; set; }

    protected override Visibility convert(object value) {
      bool result = value.GetType() == this.Type;
      if (this.Inverted) {
        result = !result;
      }
      return result ? Visibility.Visible : Visibility.Collapsed;
    }
  }
}
