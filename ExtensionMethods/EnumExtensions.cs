using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public static class Enum<T> {
    public static T[] GetValues() {
      return (T[])Enum.GetValues(typeof(T));
    }

    public static string[] GetNames() {
      return Enum.GetNames(typeof(T));
    }

    public static string GetDescription() {
      var attributes = (DescriptionAttribute[])typeof(T).GetCustomAttributes(typeof(DescriptionAttribute), false);
      if (attributes.Length == 0) {
        return null;
      } else {
        return attributes.First().Description;
      }
    }

    public static string GetDescription(T value) {
      if (value == null) {
        throw new ArgumentNullException("value");
      }
      var description = value.ToString();
      var fieldInfo = value.GetType().GetField(description);
      var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
      if (attributes != null && attributes.Length > 0) {
        description = attributes.First().Description;
      }
      return description;
    }

    public static IEnumerable<KeyValuePair<string, T>> GetDescriptionValuePairs() {
      return GetValues().Select(v => new KeyValuePair<string, T>(GetDescription(v), v));
    }
  }
}
