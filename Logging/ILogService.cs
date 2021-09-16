using System.Globalization;

namespace HAF {
  public interface ILogService: IService {
    IReadOnlyObservableCollection<ILogEntry> LogEntries { get; }
    void Info(string message);
    void Error(string message);
    void Warning(string message);
    void ClearLogEntries();
    IRelayCommand DoClearLogEntries { get; }
  }
}