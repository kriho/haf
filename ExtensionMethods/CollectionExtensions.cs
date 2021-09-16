using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public static class CollectionExtensions {
    public static bool IsNullOrEmpty(this ICollection collection) {
      return collection == null || collection.Count == 0;
    }

    public static void AddRangeIterative<T, S>(this ICollection<T> collection, IEnumerable<S> values) where S : T {
      foreach (var value in values) {
        collection.Add(value);
      }
    }

    public static void AddRangeIterative<T, S>(this ICollection<T> collection, params S[] values) where S : T {
      foreach(var value in values) {
        collection.Add(value);
      }
    }
  }
}
