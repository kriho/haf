using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HAF.Converters {
  public class IntegerToPluralizedStringConverter: ValueConverter<int, string> {

    public string ZeroFormat { get; set; }
    public string SingularFormat { get; set; }
    public string PluralFormat { get; set; }

    protected override string convert(int value) {
      if (value == 1) {
        return String.Format(this.SingularFormat, value);
      } else if (value == 0 && this.ZeroFormat != null) {
        return String.Format(this.ZeroFormat, value);
      } else {
        return String.Format(this.PluralFormat, value);
      }
    }
  }
}
