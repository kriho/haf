using System.Collections.Generic;

namespace HAF {
  public interface IUpdatesService : IService {
    RelayCommand DoApply { get; }
    RelayCommand DoCancel { get; }
    RelayCommand DoFetch { get; }
    RelayCommand DoInstall { get; }
    string AvaliableVersion { get; }
    string CurrentVersion { get; }
    IReadOnlyState IsBusy { get; }
    IReadOnlyState IsRestartRequired { get; }
    IReadOnlyState HasUpdate { get; }
    int Progress { get; }
    ICompoundState MayUpdate { get; }
    IReadOnlyState CanUpdate { get; }
    Event OnAvailableVersionChanged { get; }
    Dictionary<string, string[]> PatchNotes { get; }
  }
}