using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public static partial class Utils {

    public static bool HasCommandLineArgument(string name) {
      foreach(var arg in Environment.GetCommandLineArgs()) {
        if(!String.IsNullOrWhiteSpace(arg) && arg.Contains('=')) {
          if(arg.Substring(0, arg.IndexOf('=')) == name) {
            return true;
          }
        }
      }
      return false;
    }

    public static T GetCommandLineArgument<T>(string name) {
      foreach(var arg in Environment.GetCommandLineArgs()) {
        var tokens = arg.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
        if(tokens.Length == 2 && tokens[0] == name) {
          return (T)Convert.ChangeType(tokens[1], typeof(T));
        }
      }
      return default;
    }
  }
}
