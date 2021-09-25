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

    public static void Info(string message) {
      Log.LogService.Info(message);
    }

    public static void Warning(string message) {
      Log.LogService.Warning(message);
    }

    public static void Error(string message) {
      Log.LogService.Error(message);
    }
  }
}
