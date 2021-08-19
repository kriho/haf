using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HAF {

  public interface ITaskProgress: IProgress<int> {
    void ReportIndeterminate(string description = null);
    void ReportProgress(int? value = null);
    void ReportProgress(string description);
    void ReportProgress(int value, string description);
    void ReportProgress(int value, int maximum, string description = "", int? normalizer = null);
    void NormalizeProgress(int? normalizer);
  }

  public class ObservableTask {

    public ObservableTaskPool Pool { get; internal set; }
    
    public ObservableTaskProgress Progress { get; private set; }

    private ObservableTaskProgress initialProgress;
    
    private Func<Task> work;
    
    private Task task;

    private CancellationTokenSource cancellationTokenSource;

    public bool IsCancelled {
      get { return this.cancellationTokenSource?.IsCancellationRequested == true; }
    }

    public void RevertProgress() {
      this.Progress.IsIndeterminate = this.initialProgress.IsIndeterminate;
      this.Progress.Value = this.initialProgress.Value;
      this.Progress.Maximum = this.initialProgress.Maximum;
      this.Progress.Description = this.initialProgress.Description;
      this.Progress.IsRunning = false;
    }

    public RelayCommand DoCancel { get; private set; }

    public ObservableTask(Func<Task> work, string description, ObservableTaskPool pool = null) {
      this.initialProgress = new ObservableTaskProgress(description);
      this.Initialize(work, pool);
    }

    public ObservableTask(Func<CancellationToken, Task> work, string description, ObservableTaskPool pool = null) {
      this.initialProgress = new ObservableTaskProgress(description);
      this.Initialize(async () => {
        await work(this.cancellationTokenSource.Token);
      }, pool);
    }

    public ObservableTask(Func<IProgress<int>, Task> work, string description, int maximum = 0, int value = 0, ObservableTaskPool pool = null) {
      this.initialProgress = new ObservableTaskProgress(description, maximum, value);
      this.Initialize(async () => {
        await work(this.Progress);
      }, pool);
    }

    public ObservableTask(Func<ITaskProgress, Task> work, string description, int maximum = 0, int value = 0, ObservableTaskPool pool = null) {
      this.initialProgress = new ObservableTaskProgress(description, maximum, value);
      this.Initialize(async () => {
        await work(this.Progress);
      }, pool);
    }

    public ObservableTask(Func<IProgress<int>, CancellationToken, Task> work, string description, int maximum = 0, int value = 0, ObservableTaskPool pool = null) {
      this.initialProgress = new ObservableTaskProgress(description, maximum, value);
      this.Initialize(async () => {
        await work(this.Progress, this.cancellationTokenSource.Token);
      }, pool);
    }

    public ObservableTask(Func<ITaskProgress, CancellationToken, Task> work, string description, int maximum = 0, int value = 0) {
      this.initialProgress = new ObservableTaskProgress(description, maximum, value);
      this.Initialize(async () => {
        await work(this.Progress, this.cancellationTokenSource.Token);
      }, Pool);
    }

    private void Initialize(Func<Task> work, ObservableTaskPool pool) {
      // apply initial progress
      this.RevertProgress();
      this.DoCancel = new RelayCommand(this.Cancel, () => {
        return this.cancellationTokenSource != null && !this.cancellationTokenSource.IsCancellationRequested && this.Progress.IsRunning;
      });
      this.work = work;
      this.Pool = pool;
      // register task
      if(this.Pool != null) {
        this.Pool.RegisterTask(this);
      }
    }

    public async Task Run() {
      try {
        // revert progress to make tast restartable
        this.RevertProgress();
        // add and indicate as running
        this.Progress.IsRunning = true;
        // create new cancellation token source
        this.cancellationTokenSource = new CancellationTokenSource();
        this.task = this.work.Invoke();
        await this.task;
      } catch(TaskCanceledException) {
      } finally {
        this.Progress.IsRunning = false;
      }
    }

    public async Task Schedule() {
      if(this.Pool == null) {
        await this.Run();
      } else {
        await this.Pool.ScheduleTask(this);
      }
    }

    public void Cancel() {
      if (this.cancellationTokenSource != null && !this.cancellationTokenSource.IsCancellationRequested) {
        this.cancellationTokenSource.Cancel();
      }
    }

    public async Task WaitForCompletion() {
      if(this.Progress.IsRunning && this.task != null) {
        await this.task;
      }
    }
  }
}
