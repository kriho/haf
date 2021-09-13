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

    public void Log(string message) {
      this.logEntries.Add(new LogEntry() {
        Timestamp = DateTime.Now,
        Message = message,
      });
    }

    public void ClearLogEntries() {
      this.logEntries.Clear();
    }
  }
}