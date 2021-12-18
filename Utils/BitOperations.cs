using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public static partial class Utils {
    public static void RotateLeft(byte[] bytes) {
      if(Utils.ShiftLeft(bytes)) {
        bytes[bytes.Length - 1] = (byte)(bytes[bytes.Length - 1] | 0x01);
      }
    }

    public static void RotateRight(byte[] bytes) {
      if(Utils.ShiftRight(bytes)) {
        bytes[0] = (byte)(bytes[0] | 0x80);
      }
    }

    public static bool ShiftLeft(byte[] bytes) {
      var leftMostCarryFlag = false;
      for(var index = 0; index < bytes.Length; index++) {
        var carryFlag = (bytes[index] & 0x80) > 0;
        if(index > 0) {
          if(carryFlag) {
            bytes[index - 1] = (byte)(bytes[index - 1] | 0x01);
          }
        } else {
          leftMostCarryFlag = carryFlag;
        }
        bytes[index] = (byte)(bytes[index] << 1);
      }
      return leftMostCarryFlag;
    }

    public static bool ShiftLeft(byte[] bytes, int count) {
      var leftMostCarryFlag = false;
      for(var index = 0; index < count; index++) {
        leftMostCarryFlag = Utils.ShiftLeft(bytes);
      }
      return leftMostCarryFlag;
    }

    public static bool ShiftRight(byte[] bytes) {
      var rightMostCarryFlag = false;
      var rightEnd = bytes.Length - 1;
      for(var index = rightEnd; index >= 0; index--) {
        var carryFlag = (bytes[index] & 0x01) > 0;
        if(index < rightEnd) {
          if(carryFlag) {
            bytes[index + 1] = (byte)(bytes[index + 1] | 0x80);
          }
        } else {
          rightMostCarryFlag = carryFlag;
        }
        bytes[index] = (byte)(bytes[index] >> 1);
      }
      return rightMostCarryFlag;
    }


    public static bool ShiftRight(byte[] bytes, int count) {
      var rightMostCarryFlag = false;
      for(var index = 0; index < count; index++) {
        rightMostCarryFlag = Utils.ShiftRight(bytes);
      }
      return rightMostCarryFlag;
    }
  }
}
