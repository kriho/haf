using System.Globalization;

namespace HAF {
  public interface ILogService: IService {
    IReadOnlyObservableCollection<ILogEntry> LogEntries { get; }
    void Info(string message);
    void Error(string message);
    void Warning(string message);
    void ClearLogEntries();
    ILogEntry SelectedEntry { get; set; }
    LinkedEvent<ILogEntry> OnEntryAdded { get; }
    IRelayCommand DoClearLogEntries { get; }
    IRelayCommand<ILogEntry> DoRemoveEntry { get; }
    IRelayCommand DoRemoveSelectedEntry { get; }
  }
}