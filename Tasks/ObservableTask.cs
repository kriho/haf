using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HAF {
  public class ObservableTask {
    public static CancellationToken InvalidCancellationToken = CancellationToken.None;

    private TaskProgress initialProgress;
    private ObservableTaskPool pool;

    public TaskProgress Progress { get; private set; }

    private CancellationTokenSource cancellationTokenSource;
    public CancellationToken CancellationToken {
      get {
        return this.cancellationTokenSource.Token;
      }
    }

    public void Abort() {
      if (!this.IsCancelled) {
        this.cancellationTokenSource.Cancel();
      }
    }

    public bool IsCancelled {
      get { return this.cancellationTokenSource.IsCancellationRequested; }
    }

    public void ReportIndeterminate(string description = null) {
      Application.Current.Dispatcher.Invoke(new Action(() => {
        this.Progress.IsIndeterminate = true;
        if (description != null) {
          this.Progress.Description = description;
        }
      }));
    }

    public void ReportProgress(int? value = null) {
      Application.Current.Dispatcher.Invoke(new Action(() => {
        if (value.HasValue) {
          this.Progress.Value = value.Value;
        } else {
          this.Progress.IncreaseValue(1);
        }
      }));
    }

    public void ReportProgress(string description) {
      Application.Current.Dispatcher.Invoke(new Action(() => {
        this.Progress.Description = description;
      }));
    }

    public void ReportProgress(int value, string description) {
      Application.Current.Dispatcher.Invoke(new Action(() => {
        this.Progress.Value = value;
        this.Progress.Description = description;
      }));
    }

    public void ReportProgress(int value, int maximum, string description = "", int? normalizer = null) {
      Application.Current.Dispatcher.Invoke(new Action(() => {
        this.Progress.IsIndeterminate = (value == maximum);
        this.Progress.Value = value;
        this.Progress.Maximum = maximum;
        this.Progress.Normalizer = normalizer;
        if (description != "") {
          this.Progress.Description = description;
        }
      }));
    }

    public void NormalizeProgress(int? normalizer) {
      this.Progress.Normalizer = normalizer;
    }

    public void Report(Action action) {
      Application.Current.Dispatcher.Invoke(new Action(() => {
        action();
      }));
    }

    public void AssignPool(ObservableTaskPool pool) {
      this.pool = pool;
    }

    public void RevertProgress() {
      this.Progress.IsIndeterminate = this.initialProgress.IsIndeterminate;
      this.Progress.Value = this.initialProgress.Value;
      this.Progress.Maximum = this.initialProgress.Maximum;
      this.Progress.Description = this.initialProgress.Description;
      this.Progress.IsRunning = false;
    }

    public RelayCommand DoAbort { get; private set; }

    public ObservableTask(string description = "", int maximum = 0, int value = 0, CancellationTokenSource cancellationTokenSource = null) {
      // store cancellation token source
      this.cancellationTokenSource = cancellationTokenSource;
      // apply initial progress
      this.initialProgress = new TaskProgress(description, maximum, value);
      this.RevertProgress();
      this.DoAbort = new RelayCommand(() => {
        this.Abort();
      }, () => {
        return !this.IsCancelled;
      });
    }

    public void Reset() {
      this.cancellationTokenSource = new CancellationTokenSource();
    }

    private void BeginRun() {
      Application.Current.Dispatcher.Invoke(new Action(() => {
        // revert progress to make tast restartable
        this.RevertProgress();
        // add and indicate as running
        this.Progress.IsRunning = true;
      }));
    }

    private void EndRun() {
      Application.Current.Dispatcher.Invoke(new Action(() => {
        // remove and indicate as completed
        this.Progress.IsRunning = false;
        this.pool?.UnscheduleTask(this);
      }));
    }

    public async Task Run(Action<ObservableTask> workItem) {
      this.BeginRun();
      await Task.Run(() => {
        workItem(this);
      });
      this.EndRun();
    }

    public async Task Run(Func<ObservableTask, Task> workItem) {
      this.BeginRun();
      // perform work item
      await workItem(this);
      this.EndRun();
    }

    public async Task<T> Run<T>(Func<ObservableTask, T> workItem) {
      this.BeginRun();
      // perform work item
      var result = await Task.Run(() => {
        return workItem(this);
      });
      this.EndRun();
      return result;
    }

    public async Task<T> Run<T>(Func<ObservableTask, Task<T>> workItem) {
      this.BeginRun();
      // perform work item
      var result = await workItem(this);
      this.EndRun();
      return result;
    }

    public static Task Run(Action<ObservableTask> workItem, string description = "", int maximum = 0, int value = 0, CancellationTokenSource cancellationTokenSource = null) {
      return new ObservableTask(description, maximum, value, cancellationTokenSource).Run(workItem);
    }

    public static Task Run(Func<ObservableTask, Task> workItem, string description = "", int maximum = 0, int value = 0, CancellationTokenSource cancellationTokenSource = null) {
      return new ObservableTask(description, maximum, value, cancellationTokenSource).Run(workItem);
    }

    public static Task<T> Run<T>(Func<ObservableTask, T> workItem, string description = "", int maximum = 0, int value = 0, CancellationTokenSource cancellationTokenSource = null) {
      return new ObservableTask(description, maximum, value, cancellationTokenSource).Run(workItem);
    }

    public static Task<T> Run<T>(Func<ObservableTask, Task<T>> workItem, string description = "", int maximum = 0, int value = 0, CancellationTokenSource cancellationTokenSource = null) {
      return new ObservableTask(description, maximum, value, cancellationTokenSource).Run(workItem);
    }
  }
}
