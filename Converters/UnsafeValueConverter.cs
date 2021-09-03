using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace HAF {
  [MarkupExtensionReturnType(typeof(IValueConverter))]
  public abstract class UnsafeValueConverter<Tin, Tout>: MarkupExtension, IValueConverter {
    protected abstract Tout convert(Tin value);

    protected virtual Tin convertBack(Tout value) {
      throw new NotImplementedException("ConvertBack has not been implemented for this converter.");
    }

    protected CultureInfo culture;

    public override object ProvideValue(IServiceProvider serviceProvider) {
      return this;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      this.culture = culture;
      var input = default(Tin);
      if (value != null) {
        input = (Tin)value;
      }
      return this.convert(input);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      this.culture = culture;
      var input = default(Tout);
      if (value != null) {
        input = (Tout)value;
      }
      return this.convertBack(input);
    }
  }
}
