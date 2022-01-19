using System.Collections.Generic;
using System.Windows.Data;

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
    IReadOnlyList<IPatchNotes> PatchNotes { get; }
    CollectionViewSource FilteredPatchNotes { get; }
    string Filter { get; set; }
    IRelayCommand DoClearFilter { get; }
    IPatchNotes AddPatchNotes(string version);
  }
}