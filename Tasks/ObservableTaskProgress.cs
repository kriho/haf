using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HAF {
  public class ObservableTaskProgress: ObservableObject, IObservableTaskProgress {
    private int maximum;
    public int Maximum {
      get {
        if(this.normalizer.HasValue) {
          return this.normalizer.Value;
        } else {
          return this.maximum;
        }
      }
      internal set { this.SetValue(ref this.maximum, value); }
    }

    private int value;
    public int Value {
      get {
        if(this.normalizer.HasValue) {
          if(this.maximum == 0) {
            return 0;
          } else {
            return this.value * this.normalizer.Value / this.maximum;
          }
        } else {
          return this.value;
        }
      }
      internal set { this.SetValue(ref this.value, value); }
    }

    private bool isIndeterminate;
    public bool IsIndeterminate {
      get { return this.isIndeterminate; }
      internal set { this.SetValue(ref this.isIndeterminate, value); }
    }

    private string description;
    public string Description {
      get { return this.description; }
      internal set { this.SetValue(ref this.description, value); }
    }

    private int? normalizer;
    public int? Normalizer {
      get { return this.normalizer; }
      internal set { this.SetValue(ref this.normalizer, value); }
    }

    public ObservableTaskProgress() {
      this.Description = "";
      this.IsIndeterminate = true;
      this.Maximum = 0;
      this.Value = 0;
      this.normalizer = null;
    }

    public ObservableTaskProgress(string description = "", int maximum = 0, int value = 0) {
      this.description = description;
      this.isIndeterminate = (maximum == value);
      this.maximum = maximum;
      this.value = value;
      this.normalizer = null;
    }

    public void ReportIndeterminate(string description = null) {
      if(Application.Current.Dispatcher.CheckAccess()) {
        this.IsIndeterminate = true;
        if(description != null) {
          this.Description = description;
        }
      } else {
        Application.Current.Dispatcher.Invoke(() => {
          this.IsIndeterminate = true;
          if(description != null) {
            this.Description = description;
          }
        });
      }
    }

    public void ReportProgress(int? value = null) {
      if(Application.Current.Dispatcher.CheckAccess()) {
        if(value.HasValue) {
          this.Value = value.Value;
        } else {
          this.IncreaseValue(1);
        }
      } else {
        Application.Current.Dispatcher.Invoke(() => {
          if(value.HasValue) {
            this.Value = value.Value;
          } else {
            this.IncreaseValue(1);
          }
        });
      }
    }

    public void ReportRelativeProgress(int value) {
      if(Application.Current.Dispatcher.CheckAccess()) {
        this.IncreaseValue(value);
      } else {
        Application.Current.Dispatcher.Invoke(() => {
          this.IncreaseValue(value);
        });
      }
    }

    public void ReportRelativeProgress(int value, string description) {
      if(Application.Current.Dispatcher.CheckAccess()) {
        this.IncreaseValue(value);
        this.Description = description;
      } else {
        Application.Current.Dispatcher.Invoke(() => {
          this.IncreaseValue(value);
          this.Description = description;
        });
      }
    }

    public void ReportProgress(string description) {
      if(Application.Current.Dispatcher.CheckAccess()) {
        this.Description = description;
      } else {
        Application.Current.Dispatcher.Invoke(() => {
          this.Description = description;
        });
      }
    }

    public void ReportProgress(int value, string description) {
      if(Application.Current.Dispatcher.CheckAccess()) {
        this.Value = value;
        this.Description = description;
      } else {
        Application.Current.Dispatcher.Invoke(() => {
          this.Value = value;
          this.Description = description;
        });
      }
    }

    public void ReportProgress(int value, int maximum, string description = "", int? normalizer = null) {
      if(Application.Current.Dispatcher.CheckAccess()) {
        this.IsIndeterminate = value == maximum;
        this.Value = value;
        this.Maximum = maximum;
        this.Normalizer = normalizer;
        if(description != "") {
          this.Description = description;
        }
      } else {
        Application.Current.Dispatcher.Invoke(() => {
          this.IsIndeterminate = value == maximum;
          this.Value = value;
          this.Maximum = maximum;
          this.Normalizer = normalizer;
          if(description != "") {
            this.Description = description;
          }
        });
      }
    }

    public void NormalizeProgress(int? normalizer) {
      this.Normalizer = normalizer;
    }

    public void IncreaseValue(int value) {
      this.value += value;
      this.NotifyPropertyChanged(() => this.Value);
    }

    public void Report(int value) {
      this.ReportProgress(value);
    }
  }
}
