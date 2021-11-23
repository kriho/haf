using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public static class Log {
    internal static readonly ILogService LogService;

    static Log() {
      Log.LogService = Configuration.Container.GetExportedValue<ILogService>();
    }

    public static void Info(string message, string source = null) {
      Log.LogService.Info(message, source);
    }

    public static void Warning(string message, string source = null) {
      Log.LogService.Warning(message, source);
    }

    public static void Error(string message, string source = null) {
      Log.LogService.Error(message, source);
    }
  }
}
