using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HAF.Converters {
  public class IsEqualToBoolean: MultiValueConverter<object, bool> {

    protected override bool convert(IEnumerable<object> values) {
      var first = values.FirstOrDefault();
      return values.All(v => v.Equals(first));
    }
  }
}
