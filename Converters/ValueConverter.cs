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
  /// Abstract base class for all value converters.
  /// </summary>
  /// <typeparam name="Tin">Type of the converter input.</typeparam>
  /// <typeparam name="Tout">Type of the converter output.</typeparam>
  [MarkupExtensionReturnType(typeof(IValueConverter))]
  public abstract class ValueConverter<Tin, Tout>: MarkupExtension, IValueConverter {
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

    /// <summary>
    /// Provide another value converter to prepare the value. As this property is supported by all value converters, it enables limitless piping.
    /// </summary>
    public IValueConverter Prepare { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      if (!targetType.IsAssignableFrom(typeof(Tout))) {
        throw new NotImplementedException("This converter can not be used for the given type.");
      }
      this.culture = culture;
      var input = default(Tin);
      if (value != null) {
        input = (Tin)value;
      }
      if (this.Prepare != null) {
        return this.Prepare.Convert(this.convert(input), typeof(Tin), null, this.culture);
      }
      return this.convert(input);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      if (targetType != typeof(Tin)) {
        throw new NotImplementedException("This converter can not be used for the given type.");
      }
      this.culture = culture;
      var input = default(Tout);
      if (value != null) {
        input = (Tout)value;
      }
      if (this.Prepare != null) {
        return this.Prepare.ConvertBack(this.convertBack(input), typeof(Tout), null, this.culture);
      }
      return this.convertBack(input);
    }
  }
}
