using HAF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HAF {

  [Export(typeof(IUpdatesService)), PartCreationPolicy(CreationPolicy.Shared)]
  public class UpdatesService : Service, IUpdatesService {
    public Dictionary<string, string[]> PatchNotes { get; private set; } = new Dictionary<string, string[]>();

    public ICompoundState MayUpdate { get; private set; } = new CompoundState();

    public Event OnAvailableVersionChanged { get; private set; } = new Event(nameof(OnAvailableVersionChanged));

    public IState CanUpdate { get; private set; } = new State(false);
    IReadOnlyState IUpdatesService.CanUpdate => (IReadOnlyState)this.CanUpdate;

    public IState HasUpdate { get; private set; } = new State(false);
    IReadOnlyState IUpdatesService.HasUpdate => (IReadOnlyState)this.HasUpdate;

    public IState IsBusy { get; private set; } = new State(false);
    IReadOnlyState IUpdatesService.IsBusy => (IReadOnlyState)this.IsBusy;


    public IState IsRestartRequired { get; private set; } = new State(false);
    IReadOnlyState IUpdatesService.IsRestartRequired => (IReadOnlyState)this.IsRestartRequired;

    private int progress = 0;
    public int Progress {
      get { return this.progress; }
      set { this.SetValue(ref this.progress, value); }
    }

    private string avaliableVersion;
    public string AvaliableVersion {
      get { return this.avaliableVersion; }
      set {
        if (this.SetValue(ref this.avaliableVersion, value)) {
          this.OnAvailableVersionChanged.Fire();
        }
      }
    }

    public string CurrentVersion {
      get {
        if (!ApplicationDeployment.IsNetworkDeployed) {
          return null;
        }
        return $"{ApplicationDeployment.CurrentDeployment.CurrentVersion.Major}.{ApplicationDeployment.CurrentDeployment.CurrentVersion.Revision}";
      }
    }

    public RelayCommand DoFetch { get; private set; }
    public RelayCommand DoInstall { get; private set; }
    public RelayCommand DoApply { get; private set; }
    public RelayCommand DoCancel { get; private set; }

    public UpdatesService() {
      this.MayUpdate.AddStates(this.CanUpdate);
      // commands
      this.DoFetch = new RelayCommand(() => {
        this.IsBusy.Value = true;
        this.Progress = 0;
        Task.Factory.StartNew(() => {
          ApplicationDeployment.CurrentDeployment.CheckForUpdateAsync();
        });
      }, this.IsBusy.Negated);
      this.DoInstall = new RelayCommand(() => {
        this.IsBusy.Value = true;
        this.Progress = 0;
        Task.Factory.StartNew(() => {
          ApplicationDeployment.CurrentDeployment.UpdateAsync();
        });
      }, new CompoundState(this.HasUpdate, this.MayUpdate, this.IsBusy.Negated));
      this.DoApply = new RelayCommand(() => {
        System.Windows.Forms.Application.Restart();
        System.Windows.Application.Current.Shutdown();
      }, new CompoundState(this.IsRestartRequired, this.IsBusy.Negated, this.MayUpdate));
      this.DoCancel = new RelayCommand(() => {
        ApplicationDeployment.CurrentDeployment.UpdateAsyncCancel();
      }, this.IsBusy);
      // check if updates are supported
      this.CanUpdate.Value = ApplicationDeployment.IsNetworkDeployed;
      if (this.MayUpdate.Value) {
        // register event handlers
        ApplicationDeployment.CurrentDeployment.CheckForUpdateCompleted += (s, e) => {
          this.IsBusy.Value = false;
          if (e.Error != null) {
            // TODO log $"failed to get update description, {e.Error.Message}"
            this.HasUpdate.Value = false;
          } else {
            this.HasUpdate.Value = e.UpdateAvailable;
          }
          if (this.HasUpdate.Value) {
            this.AvaliableVersion = e.AvailableVersion.Major.ToString() + "." + e.AvailableVersion.Revision.ToString();
          } else {
            this.AvaliableVersion = null;
          }
        };
        ApplicationDeployment.CurrentDeployment.CheckForUpdateProgressChanged += (s, e) => {
          this.Progress = e.ProgressPercentage;
        };
        ApplicationDeployment.CurrentDeployment.UpdateCompleted += (s, e) => {
          this.IsBusy.Value = false;
          if (e.Error == null && !e.Cancelled) {
            this.IsRestartRequired.Value = true;
            this.HasUpdate.Value = false;
          }
        };
        ApplicationDeployment.CurrentDeployment.UpdateProgressChanged += (s, e) => {
          this.Progress = e.ProgressPercentage;
        };
        this.DoFetch.Execute(null);
      }
      this.CanUpdate.RegisterUpdate(() => {
        this.DoInstall.RaiseCanExecuteChanged();
        this.DoApply.RaiseCanExecuteChanged();
      });
    }
  }
}
