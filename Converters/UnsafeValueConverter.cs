using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace HAF.Converters {
  /// <summary>
  /// Abstract base class for all value converters that do not perform strict null checks.
  /// </summary>
  /// <typeparam name="Tin">Type of the converter input.</typeparam>
  /// <typeparam name="Tout">Type of the converter output.</typeparam>
  [MarkupExtensionReturnType(typeof(IValueConverter))]
  public abstract class UnsafeValueConverter<Tin, Tout>: MarkupExtension, IValueConverter {
    /// <summary>
    /// Handler used by derived classes to implement the conversion logic.
    /// </summary>
    /// <param name="value">Input value.</param>
    /// <returns>Output value.</returns>
    protected abstract Tout convert(Tin value);

    /// <summary>
    /// Handler used by derived classes to implement the reversed conversion logic.
    /// </summary>
    /// <param name="value">Output value.</param>
    /// <returns>Input value.</returns>
    /// <exception cref="NotImplementedException">When the reverse conversion is not implemented.</exception>
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
