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
  public class BooleanToVisibilityConverter: ValueConverter<bool, Visibility> {
    public bool Inverted { get; set; } = false;
    public Visibility InvisibleState { get; set; } = Visibility.Collapsed;

    protected override Visibility convert(bool value) {
      return this.Inverted ? !value ? Visibility.Visible : this.InvisibleState : value ? Visibility.Visible : this.InvisibleState;
    }

    protected override bool convertBack(Visibility value) {
      return value == Visibility.Visible ? !this.Inverted : this.Inverted;
    }
  }
}
