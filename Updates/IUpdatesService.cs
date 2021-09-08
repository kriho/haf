namespace HAF {
  public interface IUpdatesService : IService {
    RelayCommand DoApply { get; }
    RelayCommand DoCancel { get; }
    RelayCommand DoFetch { get; }
    RelayCommand DoInstall { get; }
    string AvaliableVersion { get; }
    string CurrentVersion { get; }
    bool IsBusy { get; }
    bool IsRestartRequired { get; }
    bool IsUpdateAvaliable { get; }
    int Progress { get; }
    LinkedState CanUpdate { get; }
    LinkedEvent OnAvailableVersionChanged { get; }
  }
}