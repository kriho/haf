using System;
using System.ComponentModel.Composition;
using System.Globalization;

namespace HAF {
  [Export(typeof(ILogService)), PartCreationPolicy(CreationPolicy.Shared)]
  public class LogService: Service, ILogService {

    private ObservableCollection<ILogEntry> logEntries = new ObservableCollection<ILogEntry>();
    public IReadOnlyObservableCollection<ILogEntry> LogEntries => this.logEntries;

    public IRelayCommand DoClearLogEntries { get; private set; }

    public LogService() {
      this.DoClearLogEntries = new RelayCommand(this.ClearLogEntries);
    }

    public void Info(string message) {
      this.logEntries.Add(new LogEntry() {
        Timestamp = DateTime.Now,
        Message = message,
        Type = LogType.Info,
      });
    }

    public void Warning(string message) {
      this.logEntries.Add(new LogEntry() {
        Timestamp = DateTime.Now,
        Message = message,
        Type = LogType.Warning,
      });
    }

    public void Error(string message) {
      this.logEntries.Add(new LogEntry() {
        Timestamp = DateTime.Now,
        Message = message,
        Type = LogType.Error,
      });
    }

    public void ClearLogEntries() {
      this.logEntries.Clear();
    }
  }
}