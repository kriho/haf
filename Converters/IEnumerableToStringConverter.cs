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
  public class IEnumerableToStringConverter: ValueConverter<IEnumerable<object>, string> {
    public string Seperator { get; set; } = ", ";

    protected override string convert(IEnumerable<object> value) {
      return String.Join(this.Seperator, value.Select(v => v.ToString()));
    }

    protected override IEnumerable<object> convertBack(string value) {
      throw new NotImplementedException();
    }
  }
}
