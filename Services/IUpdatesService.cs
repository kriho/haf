namespace HAF {
  public interface IUpdatesService : IService {
    RelayCommand _Apply { get; }
    RelayCommand _Cancel { get; }
    RelayCommand _Fetch { get; }
    RelayCommand _Install { get; }
    string AvaliableVersion { get; set; }
    string CurrentVersion { get; }
    bool IsBusy { get; set; }
    bool IsRestartRequired { get; set; }
    bool IsUpdateAvaliable { get; set; }
    int Progress { get; set; }
    bool SupportsUpdates { get; set; }
    ServiceEvent OnAvailableVersionChanged { get; }
  }
}