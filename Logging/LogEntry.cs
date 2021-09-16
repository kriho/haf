using System;
using System.Globalization;

namespace HAF {
  public class LogEntry: ILogEntry {
    public DateTime Timestamp { get; set; }
    public string Message { get; set; }
    public LogType Type { get; set; }
  }
}