using System.Globalization;

namespace HAF {
  public interface ILogService: IService {
    IReadOnlyObservableCollection<ILogEntry> LogEntries { get; }
    void Log(string message);
    void ClearLogEntries();
    IRelayCommand DoClearLogEntries { get; }
  }
}