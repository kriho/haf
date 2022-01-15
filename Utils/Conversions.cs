using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public static partial class Utils {
    public static byte[] HexadecimalStringToByteArray(string hex) {
      return Enumerable.Range(0, hex.Length)
                       .Where(x => x % 2 == 0)
                       .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                       .ToArray();
    }


    public static string ByteArrayToHexadecimalString(byte[] buffer, bool addPrefix = true, int offset = 0, int? length = 0) {
      var usedLength = length.HasValue ? length.Value : buffer.Length;
      var hex = new StringBuilder(usedLength * 2 + (addPrefix ? 2 : 0));
      if(addPrefix) {
        hex.Append("0x");
      }
      for(var index = 0; index < usedLength; index++) {
        hex.AppendFormat("{0:X2}", buffer[offset + index]);
      }
      return hex.ToString();
    }
  }
}
