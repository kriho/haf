using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public static partial class Utils {
    public static ushort SwapEndianness(ushort value) {
      unchecked {
        return (ushort)(((value & 0xFF00U) >> 8) | ((value & 0x00FFU) << 8));
      }
    }
    public static short SwapEndianness(short value) {
      unchecked {
        return (short)SwapEndianness((ushort)value);
      }
    }

    public static uint SwapEndianness(uint value) {
      value = (value >> 16) | (value << 16);
      value = ((value & 0xFF00FF00U) >> 8) | ((value & 0x00FF00FFU) << 8);
      return value;
    }

    public static int SwapEndianness(int value) {
      unchecked {
        return (int)SwapEndianness((uint)value);
      }
    }

    public static ulong SwapEndianness(ulong value) {
      value = (value >> 32) | (value << 32);
      value = ((value & 0xFFFF0000FFFF0000U) >> 16) | ((value & 0x0000FFFF0000FFFFU) << 16);
      value = ((value & 0xFF00FF00FF00FF00U) >> 8) | ((value & 0x00FF00FF00FF00FFU) << 8);
      return value;
    }

    public static long SwapEndianness(long value) {
      unchecked {
        return (long)SwapEndianness((ulong)value);
      }
    }
  }
}
