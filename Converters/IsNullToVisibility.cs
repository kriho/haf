using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HAF {
  public class IsNullToVisibilityConverter: UnsafeValueConverter<object, Visibility> {
    public bool Inverted { get; set; } = false;
    public Visibility InvisibleState { get; set; } = Visibility.Collapsed;

    protected override Visibility convert(object value) {
      var result = (value == null);
      if (this.Inverted) {
        result = !result;
      }
      return result ? Visibility.Visible : this.InvisibleState;
    }
  }
}
