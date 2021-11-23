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

    private ILogEntry selectedEntry;
    public ILogEntry SelectedEntry {
      get => this.selectedEntry;
      set => this.SetValue(ref this.selectedEntry, value);
    }

    public Event<ILogEntry> OnEntryAdded { get; private set; } = new Event<ILogEntry>(nameof(OnEntryAdded));

    public LogService() {
      this.DoClearLogEntries = new RelayCommand(this.ClearLogEntries);
      if(this.IsInDesignMode) {
        for(var index = 0; index < Faker.RandomNumber.Next(3, 50); index++) {
          this.logEntries.Add(new LogEntry() {
            Timestamp = DateTime.Now.Subtract(TimeSpan.FromSeconds(Faker.RandomNumber.Next(0, 600))),
            Message = Faker.Lorem.Sentence(10),
            Severity = Enumeration.Get<LogSeverity>(Faker.RandomNumber.Next(1, 3)),
            Source = Faker.Lorem.Sentence(10),
          });
        }
      }
    }

    public void Info(string message, string source = null) {
      var entry = new LogEntry() {
        Timestamp = DateTime.Now,
        Message = message,
        Source = source,
        Severity = LogSeverity.Info,
      };
      if(Application.Current.Dispatcher.CheckAccess()) {
        this.logEntries.Add(entry);
        this.OnEntryAdded.Fire(entry);
      } else {
        Application.Current.Dispatcher.Invoke(() => {
          this.logEntries.Add(entry);
          this.OnEntryAdded.Fire(entry);
        });
      }
    }

    public void Warning(string message, string source = null) {
      var entry = new LogEntry() {
        Timestamp = DateTime.Now,
        Message = message,
        Source = source,
        Severity = LogSeverity.Warning,
      };
      if(Application.Current.Dispatcher.CheckAccess()) {
        this.logEntries.Add(entry);
        this.OnEntryAdded.Fire(entry);
      } else {
        Application.Current.Dispatcher.Invoke(() => {
          this.logEntries.Add(entry);
          this.OnEntryAdded.Fire(entry);
        });
      }
    }

    public void Error(string message, string source = null) {
      var entry = new LogEntry() {
        Timestamp = DateTime.Now,
        Message = message,
        Source = source,
        Severity = LogSeverity.Error,
      };
      if(Application.Current.Dispatcher.CheckAccess()) {
        this.logEntries.Add(entry);
        this.OnEntryAdded.Fire(entry);
      } else {
        Application.Current.Dispatcher.Invoke(() => {
          this.logEntries.Add(entry);
          this.OnEntryAdded.Fire(entry);
        });
      }
    }

    public void ClearLogEntries() {
      this.logEntries.Clear();
    }
  }
}