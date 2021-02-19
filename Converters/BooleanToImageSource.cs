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
  public class BooleanToImageSource : ValueConverter<bool, ImageSource> {
    public bool Inverted { get; set; }
    public ImageSource TrueValue { get; set; }
    public ImageSource FalseValue { get; set; }

    public BooleanToImageSource() {
      this.Inverted = false;
    }

    protected override ImageSource convert(bool value) {
      var result = value;
      if (this.Inverted) {
        result = !result;
      }
      if (result) {
        return this.TrueValue;
      } else {
        return this.FalseValue;
      }
    }
  }
}
