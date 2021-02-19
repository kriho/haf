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
  public class BooleanToVisibility : ValueConverter<bool, Visibility> {
    public bool Inverted { get; set; }
    public Visibility InvisibleState { get; set; }

    public BooleanToVisibility() {
      this.Inverted = false;
      this.InvisibleState = Visibility.Collapsed;
    }

    protected override Visibility convert(bool value) {
      if (this.Inverted)
        return !value ? Visibility.Visible : this.InvisibleState;
      else
        return value ? Visibility.Visible : this.InvisibleState;
    }

    protected override bool convertBack(Visibility value) {
      if (value == Visibility.Visible)
        return this.Inverted ? false : true;
      else
        return this.Inverted ? true : false;
    }
  }
}
