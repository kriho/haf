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
  }
}
