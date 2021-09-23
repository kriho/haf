using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;

namespace HAF {
  [Export(typeof(ILogService)), PartCreationPolicy(CreationPolicy.Shared)]
  public class LogService: Service, ILogService {

    private ObservableCollection<ILogEntry> logEntries = new ObservableCollection<ILogEntry>();
    public IReadOnlyObservableCollection<ILogEntry> LogEntries => this.logEntries;

    public IRelayCommand DoClearLogEntries { get; private set; }

    public IRelayCommand<ILogEntry> DoRemoveEntry { get; private set; }

    public LinkedEvent<ILogEntry> OnEntryAdded { get; private set; } = new LinkedEvent<ILogEntry>(nameof(OnEntryAdded));

    public LogService() {
      this.DoClearLogEntries = new RelayCommand(this.ClearLogEntries);
      this.DoRemoveEntry = new RelayCommand<ILogEntry>(entry => {
        if(this.logEntries.Contains(entry)) {
          this.logEntries.Remove(entry);
        }
      });
    }

    public void Info(string message) {
      Application.Current.Dispatcher.Invoke(() => {
        var entry = new LogEntry() {
          Timestamp = DateTime.Now,
          Message = message,
          Type = LogType.Info,
        };
        this.logEntries.Add(entry);
        this.OnEntryAdded.Fire(entry);
      });
    }

    public void Warning(string message) {
      Application.Current.Dispatcher.Invoke(() => {
        var entry = new LogEntry() {
          Timestamp = DateTime.Now,
          Message = message,
          Type = LogType.Warning,
        };
        this.logEntries.Add(entry);
        this.OnEntryAdded.Fire(entry);
      });
    }

    public void Error(string message) {
      Application.Current.Dispatcher.Invoke(() => {
        var entry = new LogEntry() {
          Timestamp = DateTime.Now,
          Message = message,
          Type = LogType.Error,
        };
        this.logEntries.Add(entry);
        this.OnEntryAdded.Fire(entry);
      });
    }

    public void ClearLogEntries() {
      this.logEntries.Clear();
    }
  }
}