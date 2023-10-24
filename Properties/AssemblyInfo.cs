using HAF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public class LogSeverity: Enumeration {

    public static LogSeverity Diagnostic = new LogSeverity(0, "Diagnostic");
    public static LogSeverity Info = new LogSeverity(1, "Info");
    public static LogSeverity Warning = new LogSeverity(2, "Warning");
    public static LogSeverity Error = new LogSeverity(3, "Error");

    public LogSeverity(int id, string name): base(id, name) {
    }

    public static LogSeverity Parse(string value) {
      switch (value.ToLower()) {
        case "diagnostic": return LogSeverity.Diagnostic;
        case "info": return LogSeverity.Info;
        case "warning": return LogSeverity.Warning;
        default: return LogSeverity.Error;
      }
    }
  }
}
