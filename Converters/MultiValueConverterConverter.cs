using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace HAF.Converters {
  [MarkupExtensionReturnType(typeof(IMultiValueConverter))]
  public abstract class MultiValueConverter<Tin, Tout>: MarkupExtension, IMultiValueConverter {
    protected CultureInfo culture;

    protected abstract Tout convert(IEnumerable<Tin> values);

    protected virtual Tin[] convertBack(Tout values) {
      throw new NotImplementedException("ConvertBack has not been implemented for this converter.");
    }

    public override object ProvideValue(IServiceProvider serviceProvider) {
      return this;
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
      if (targetType != typeof(Tout)) {
        throw new NotImplementedException("This converter can not be used for the given type.");
      }
      this.culture = culture;
      IEnumerable<Tin> input = new Tin[0];
      if (values != null) {
        input = values.Where(o => o is Tin).Select(o => (Tin)o);
      }
      return this.convert(input);
    }

    public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture) {
      if (targetType.Any(t => t != typeof(Tout))) {
        throw new NotImplementedException("This converter can not be used for the given type.");
      }
      this.culture = culture;
      var input = default(Tout);
      if (value != null) {
        input = (Tout)value;
      }
      return this.convertBack(input).Select(o => (object)o).ToArray();
    }
  }
}
