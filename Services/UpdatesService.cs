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

    public LinkedDependency MayUpdate { get; private set; } = new LinkedDependency();

    public LinkedEvent OnAvailableVersionChanged { get; private set; } = new LinkedEvent();

    private bool supportsUpdates = false;
    public bool SupportsUpdates {
      get { return this.supportsUpdates; }
      set { this.SetValue(ref this.supportsUpdates, value); }
    }

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
          this._Fetch.RaiseCanExecuteChanged();
          this._Install.RaiseCanExecuteChanged();
          this._Apply.RaiseCanExecuteChanged();
        }
      }
    }

    private bool isUpdateAvaliable = false;
    public bool IsUpdateAvaliable {
      get { return this.isUpdateAvaliable; }
      set {
        if (this.SetValue(ref this.isUpdateAvaliable, value)) {
          this._Install.RaiseCanExecuteChanged();
        }
      }
    }

    private bool isRestartRequired = false;
    public bool IsRestartRequired {
      get { return this.isRestartRequired; }
      set {
        if (this.SetValue(ref this.isRestartRequired, value)) {
          this._Apply.RaiseCanExecuteChanged();
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

    public RelayCommand _Fetch { get; private set; }
    public RelayCommand _Install { get; private set; }
    public RelayCommand _Apply { get; private set; }
    public RelayCommand _Cancel { get; private set; }

    public UpdatesService() {
      // commands
      this._Fetch = new RelayCommand(() => {
        this.IsBusy = true;
        Task.Factory.StartNew(() => {
          ApplicationDeployment.CurrentDeployment.CheckForUpdateAsync();
        });
      }, () => {
        return !this.isBusy;
      });
      this._Install = new RelayCommand(() => {
        this.IsBusy = true;
        Task.Factory.StartNew(() => {
          ApplicationDeployment.CurrentDeployment.UpdateAsync();
        });
      }, () => {
        return this.isUpdateAvaliable && !this.isBusy && this.MayUpdate;
      });
      this._Apply = new RelayCommand(() => {
        System.Windows.Forms.Application.Restart();
        System.Windows.Application.Current.Shutdown();
      }, () => {
        return this.isRestartRequired && !this.isBusy && this.MayUpdate;
      });
      this._Cancel = new RelayCommand(() => {
        ApplicationDeployment.CurrentDeployment.UpdateAsyncCancel();
      });
      // check if updates are supported
      this.SupportsUpdates = ApplicationDeployment.IsNetworkDeployed;
      if (this.SupportsUpdates) {
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
        this._Fetch.Execute(null);
      }
      this.MayUpdate.RegisterUpdate(() => {
        this._Install.RaiseCanExecuteChanged();
        this._Apply.RaiseCanExecuteChanged();
      });
    }
  }
}
