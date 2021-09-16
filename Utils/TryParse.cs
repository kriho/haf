using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace HAF {
  public static partial class Utils {

    public static T? TryParse<T>(string value) where T : struct {
      return TryParseHelper<T>.TryParse(value, out T result) ? result : default(T);
    }

    public static T TryParse<T>(string value, T fallback) {
      return TryParseHelper<T>.TryParse(value, out T result) ? result : fallback;
    }

    public static bool TryParse<T>(string value, out T result) {
      return TryParseHelper<T>.TryParse(value, out result);
    }

    private static class TryParseHelper<T> {
      public delegate bool TryParseFunc(string str, out T result);

      private static TryParseFunc tryParse;
      public static TryParseFunc TryParse {
        get {
          if(tryParse == null) {
            tryParse = Delegate.CreateDelegate(typeof(TryParseFunc), typeof(T), "TryParse") as TryParseFunc;
          }
          return tryParse;
        }
      }
    }
  }
}
