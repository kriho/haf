using System;
using System.Globalization;

namespace HAF {
  public interface ILogEntry {
    DateTime Timestamp { get; }

    string Message { get; }

    string Source { get; }

    LogSeverity Severity { get; }
  }
}