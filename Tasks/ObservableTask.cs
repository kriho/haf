using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HAF {
  public class ObservableTask : ObservableObject {
    public static CancellationToken InvalidCancellationToken = CancellationToken.None;

    private TaskProgress initialProgress;

    private TaskProgress progress = new TaskProgress();
    public TaskProgress Progress {
      get { return this.progress; }
      private set { this.SetValue(ref this.progress, value); }
    }

    private bool isRunning = false;
    public bool IsRunning {
      get { return this.isRunning; }
      set { this.SetValue(ref this.isRunning, value); }
    }

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
          this.progress.Description = description;
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
      this.progress.Normalizer = normalizer;
    }

    public void Report(Action action) {
      Application.Current.Dispatcher.Invoke(new Action(() => {
        action();
      }));
    }

    public ObservableTask(string description = "", int maximum = 0, int value = 0, CancellationTokenSource cancellationTokenSource = null) {
      // store cancellation token source
      this.cancellationTokenSource = cancellationTokenSource;
      // apply initial progress
      this.initialProgress = new TaskProgress(description, maximum, value);
      this.RevertProgress();
    }

    public void SetPending() {
      // add to pool
      ObservableTaskPool.Default.RegisterTask(this);
    }

    public void RevertProgress() {
      this.Progress.IsIndeterminate = this.initialProgress.IsIndeterminate;
      this.progress.Value = this.initialProgress.Value;
      this.progress.Maximum = this.initialProgress.Maximum;
      this.progress.Description = this.initialProgress.Description;
    }

    private RelayCommand abortCommand;
    public RelayCommand AbortCommand {
      get {
        return this.abortCommand
             ?? (this.abortCommand = new RelayCommand(() => {
               this.Abort();
             }, () => {
               return !this.IsCancelled;
             }));
      }
    }

    public void Reset() {
      this.cancellationTokenSource = new CancellationTokenSource();
    }

    public async Task Run(Func<ObservableTask, Task> workItem) {
      Application.Current.Dispatcher.Invoke(new Action(() => {
        // revert progress to make tast restartable
        this.RevertProgress();
        // add and indicate as running
        this.IsRunning = true;
        ObservableTaskPool.Default.RegisterTask(this);
      }));
      // perform work item
      await workItem(this);
      Application.Current.Dispatcher.Invoke(new Action(() => {
        // remove and indicate as completed
        this.IsRunning = false;
        ObservableTaskPool.Default.UnregisterTask(this);
      }));
    }

    public static async Task Run(Func<ObservableTask, Task> workItem, string description = "", int maximum = 0, int value = 0, CancellationTokenSource cancellationTokenSource = null) {
      await new ObservableTask(description, maximum, value, cancellationTokenSource).Run(workItem);
    }

    public async Task<T> Run<T>(Func<ObservableTask, Task<T>> workItem) {
      Application.Current.Dispatcher.Invoke(new Action(() => {
        // revert progress to make tast restartable
        this.RevertProgress();
        // add and indicate as running
        this.IsRunning = true;
        ObservableTaskPool.Default.RegisterTask(this);
      }));
      // perform work item
      var result = await workItem(this);
      Application.Current.Dispatcher.Invoke(new Action(() => {
        // remove and indicate as completed
        this.IsRunning = false;
        ObservableTaskPool.Default.UnregisterTask(this);
      }));
      return result;
    }

    public static async Task<T> Run<T>(Func<ObservableTask, Task<T>> workItem, string description = "", int maximum = 0, int value = 0, CancellationTokenSource cancellationTokenSource = null) {
      return await new ObservableTask(description, maximum, value, cancellationTokenSource).Run(workItem);
    }

    public async Task Run(Action<ObservableTask> workItem) {
      Application.Current.Dispatcher.Invoke(new Action(() => {
        // revert progress to make tast restartable
        this.RevertProgress();
        // add and indicate as running
        this.IsRunning = true;
        ObservableTaskPool.Default.RegisterTask(this);
      }));
      // perform work item
      await Task.Run(() => {
        workItem(this);
      });
      Application.Current.Dispatcher.Invoke(new Action(() => {
        // remove and indicate as completed
        this.IsRunning = false;
        ObservableTaskPool.Default.UnregisterTask(this);
      }));
    }

    public static async Task Run(Action<ObservableTask> workItem, string description = "", int maximum = 0, int value = 0, CancellationTokenSource cancellationTokenSource = null) {
      await new ObservableTask(description, maximum, value, cancellationTokenSource).Run(workItem);
    }

    public async Task<T> Run<T>(Func<ObservableTask, T> workItem) {
      Application.Current.Dispatcher.Invoke(new Action(() => {
        // revert progress to make tast restartable
        this.RevertProgress();
        // add and indicate as running
        this.IsRunning = true;
        ObservableTaskPool.Default.RegisterTask(this);
      }));
      // perform work item
      var result = await Task.Run(() => {
        return workItem(this);
      });
      Application.Current.Dispatcher.Invoke(new Action(() => {
        // remove and indicate as completed
        this.IsRunning = false;
        ObservableTaskPool.Default.UnregisterTask(this);
      }));
      return result;
    }

    public static async Task<T> Run<T>(Func<ObservableTask, T> workItem, string description = "", int maximum = 0, int value = 0, CancellationTokenSource cancellationTokenSource = null) {
      return await new ObservableTask(description, maximum, value, cancellationTokenSource).Run(workItem);
    }
  }
}
