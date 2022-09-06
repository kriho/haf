using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public static class LinqExtensions {
    public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> @this, Func<T, TKey> keySelector) {
      return @this.GroupBy(keySelector).Select(grps => grps).Select(e => e.First());
    }

    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> enumerable) {
      return enumerable ?? Enumerable.Empty<T>();
    }

    public static double TryAverage<T>(this IEnumerable<T> enumerable, Func<T, double> selector, double fallbackValue) {
      if(enumerable.Count() == 0) {
        return fallbackValue;
      }
      return enumerable.Select(selector).Average();
    }

    public static double TryAverage(this IEnumerable<int> enumerable, double fallbackValue) {
      if(enumerable.Count() == 0) {
        return fallbackValue;
      }
      return enumerable.Average();
    }

    public static double TryAverage(this IEnumerable<double> enumerable, double fallbackValue) {
      if(enumerable.Count() == 0) {
        return fallbackValue;
      }
      return enumerable.Average();
    }

    public static double StandardDeviation(this IEnumerable<double> enumerable) {
      if(enumerable.Count() == 0) {
        return 0;
      }
      var average = enumerable.Average();
      return Math.Sqrt(enumerable.Average(v => Math.Pow(v - average, 2)));
    }
  }
}
