using HAF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public class LogType: Enumeration {

    public static LogType Info = new LogType(1, "Info");
    public static LogType Warning = new LogType(2, "Warning");
    public static LogType Error = new LogType(3, "Error");
    
    public LogType(int id, string name): base(id, name) {
    }
  }
}
