using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace HAF {
  public static partial class Utils {

    // see https://stackoverflow.com/questions/2961656/generic-tryparse/25191788#25191788 for main inspiration

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

    public static bool TryParseUnsignedInt(string value, out uint result) {
      if(value.ToLower().StartsWith("0x")) {
        return uint.TryParse(value.Substring(2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out result);
      }
      return uint.TryParse(value, out result);
    }

    public static uint ParseUnsignedInt(string value) {
     if(TryParseUnsignedInt(value, out var result)) {
        return result;
      }
      throw new InvalidOperationException($"the value \"{value}\" can not be parsed as integer");
    }
  }
}
