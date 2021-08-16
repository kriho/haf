namespace HAF {
  public interface IUpdatesService : IService {
    RelayCommand DoApply { get; }
    RelayCommand DoCancel { get; }
    RelayCommand DoFetch { get; }
    RelayCommand DoInstall { get; }
    string AvaliableVersion { get; set; }
    string CurrentVersion { get; }
    bool IsBusy { get; set; }
    bool IsRestartRequired { get; set; }
    bool IsUpdateAvaliable { get; set; }
    int Progress { get; set; }
    bool SupportsUpdates { get; set; }
    LinkedEvent OnAvailableVersionChanged { get; }
  }
}