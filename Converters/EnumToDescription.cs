using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace HAF.Converters {
  public class EnumToDescription : ValueConverter<object, string> {
    public string Fallback { get; set; }

    protected override string convert(object value) {
      if (value == null) {
        return this.Fallback;
      }
      var fieldInfo = value.GetType().GetField(value.ToString());
      var attribArray = fieldInfo.GetCustomAttributes(false);
      if (attribArray.Length == 0) {
        return this.Fallback;
      } else {
        var attrib = attribArray[0] as DescriptionAttribute;
        return attrib.Description;
      }
    }
  }
}
