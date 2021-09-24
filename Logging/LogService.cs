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

    public IRelayCommand DoRemoveSelectedEntry { get; private set; }

    public IRelayCommand<ILogEntry> DoRemoveEntry { get; private set; }

    private ILogEntry selectedEntry;
    public ILogEntry SelectedEntry {
      get => this.selectedEntry;
      set {
        if(this.SetValue(ref this.selectedEntry, value)) {
          this.DoRemoveSelectedEntry.RaiseCanExecuteChanged();
        }
      }
    }

    public LinkedEvent<ILogEntry> OnEntryAdded { get; private set; } = new LinkedEvent<ILogEntry>(nameof(OnEntryAdded));

    public LogService() {
      this.DoClearLogEntries = new RelayCommand(this.ClearLogEntries);
      this.DoRemoveEntry = new RelayCommand<ILogEntry>(entry => {
        if(this.logEntries.Contains(entry)) {
          this.logEntries.Remove(entry);
        }
      });
      this.DoRemoveSelectedEntry = new RelayCommand(() => {
        if(this.logEntries.Contains(this.selectedEntry)) {
          this.logEntries.Remove(this.selectedEntry);
        }
      }, () => this.selectedEntry != null);
      if(this.IsInDesignMode) {
        for(var index = 0; index < Faker.RandomNumber.Next(3, 50); index++) {
          this.logEntries.Add(new LogEntry() {
            Timestamp = DateTime.Now.Subtract(TimeSpan.FromSeconds(Faker.RandomNumber.Next(0, 600))),
            Message = Faker.Lorem.Sentence(10),
            Type = Enumeration.Get<LogType>(Faker.RandomNumber.Next(1, 3)),
          });
        }
      }
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