using System;
using System.Globalization;

namespace HAF {
  public interface ILogEntry {
    DateTime Timestamp { get; }
    string Message { get; }
    LogType Type { get; }
  }
}