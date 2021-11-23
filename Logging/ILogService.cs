using System.Globalization;

namespace HAF {
  public interface ILogService: IService {
    IReadOnlyObservableCollection<ILogEntry> LogEntries { get; }
    void Info(string message, string source = null);
    void Error(string message, string source = null);
    void Warning(string message, string source = null);
    Event<ILogEntry> OnEntryAdded { get; }
    IRelayCommand DoClearLogEntries { get; }
    void ClearLogEntries();
    ILogEntry SelectedEntry { get; set; }
  }
}