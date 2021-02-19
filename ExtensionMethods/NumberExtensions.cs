using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public static class NumberExtensions {
    public static string ToString(this int? value, string defaultvalue, string format = null) {
      if (value.HasValue) {
        return defaultvalue;
      }
      return format == null ? value.Value.ToString() : value.Value.ToString(format);
    }

    public static bool IsBetweenInclusive<T>(this T actual, T lower, T upper) where T : IComparable<T> {
      return actual.CompareTo(lower) >= 0 && actual.CompareTo(upper) <= 0;
    }

    public static bool IsBetweenExclusive<T>(this T actual, T lower, T upper) where T : IComparable<T> {
      return actual.CompareTo(lower) > 0 && actual.CompareTo(upper) < 0;
    }

  }
}
