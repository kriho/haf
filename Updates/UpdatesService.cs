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

    public ICompoundState MayUpdate { get; private set; } = new CompoundState();

    public Event OnAvailableVersionChanged { get; private set; } = new Event(nameof(OnAvailableVersionChanged));

    public IState CanUpdate { get; private set; } = new State(false);
    IReadOnlyState IUpdatesService.CanUpdate => (IReadOnlyState)this.CanUpdate;

    private int progress = 0;
    public int Progress {
      get { return this.progress; }
      set { this.SetValue(ref this.progress, value); }
    }

    private bool isBusy = false;
    public bool IsBusy {
      get { return this.isBusy; }
      set {
        if (this.SetValue(ref this.isBusy, value)) {
          if (value) {
            // reset progress
            this.Progress = 0;
          }
          this.DoFetch.RaiseCanExecuteChanged();
          this.DoInstall.RaiseCanExecuteChanged();
          this.DoApply.RaiseCanExecuteChanged();
        }
      }
    }

    private bool isUpdateAvaliable = false;
    public bool IsUpdateAvaliable {
      get { return this.isUpdateAvaliable; }
      set {
        if (this.SetValue(ref this.isUpdateAvaliable, value)) {
          this.DoInstall.RaiseCanExecuteChanged();
        }
      }
    }

    private bool isRestartRequired = false;
    public bool IsRestartRequired {
      get { return this.isRestartRequired; }
      set {
        if (this.SetValue(ref this.isRestartRequired, value)) {
          this.DoApply.RaiseCanExecuteChanged();
        }
      }
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
        this.IsBusy = true;
        Task.Factory.StartNew(() => {
          ApplicationDeployment.CurrentDeployment.CheckForUpdateAsync();
        });
      }, () => {
        return !this.isBusy;
      });
      this.DoInstall = new RelayCommand(() => {
        this.IsBusy = true;
        Task.Factory.StartNew(() => {
          ApplicationDeployment.CurrentDeployment.UpdateAsync();
        });
      }, () => {
        return this.isUpdateAvaliable && !this.isBusy && this.MayUpdate.Value;
      });
      this.DoApply = new RelayCommand(() => {
        System.Windows.Forms.Application.Restart();
        System.Windows.Application.Current.Shutdown();
      }, () => {
        return this.isRestartRequired && !this.isBusy && this.MayUpdate.Value;
      });
      this.DoCancel = new RelayCommand(() => {
        ApplicationDeployment.CurrentDeployment.UpdateAsyncCancel();
      });
      // check if updates are supported
      this.CanUpdate.Value = ApplicationDeployment.IsNetworkDeployed;
      if (this.MayUpdate.Value) {
        // register event handlers
        ApplicationDeployment.CurrentDeployment.CheckForUpdateCompleted += (s, e) => {
          this.IsBusy = false;
          if (e.Error != null) {
            // TODO log $"failed to get update description, {e.Error.Message}"
            this.IsUpdateAvaliable = false;
          } else {
            this.IsUpdateAvaliable = e.UpdateAvailable;
          }
          if (this.IsUpdateAvaliable) {
            this.AvaliableVersion = e.AvailableVersion.Major.ToString() + "." + e.AvailableVersion.Revision.ToString();
          } else {
            this.AvaliableVersion = null;
          }
        };
        ApplicationDeployment.CurrentDeployment.CheckForUpdateProgressChanged += (s, e) => {
          this.Progress = e.ProgressPercentage;
        };
        ApplicationDeployment.CurrentDeployment.UpdateCompleted += (s, e) => {
          this.IsBusy = false;
          if (e.Error == null && !e.Cancelled) {
            this.IsRestartRequired = true;
            this.IsUpdateAvaliable = false;
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
