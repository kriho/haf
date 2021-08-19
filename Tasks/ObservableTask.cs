using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HAF {

  public class ObservableTask: IObservableTask {

    private ObservableTaskPool pool;
    public IObservableTaskPool Pool => this.pool;

    private ObservableTaskProgress progress = new ObservableTaskProgress();
    public IObservableTaskProgress Progress => this.progress;

    private ObservableTaskProgress initialProgress;
    
    private Func<Task> work;
    
    private Task task;

    private CancellationTokenSource cancellationTokenSource;

    public bool IsCancelled {
      get { return this.cancellationTokenSource?.IsCancellationRequested == true; }
    }

    private void RevertProgress() {
      this.progress.IsIndeterminate = this.initialProgress.IsIndeterminate;
      this.progress.Value = this.initialProgress.Value;
      this.progress.Maximum = this.initialProgress.Maximum;
      this.progress.Description = this.initialProgress.Description;
      this.progress.IsRunning = false;
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
        await work(this.progress);
      }, pool);
    }

    public ObservableTask(Func<IObservableTaskProgress, Task> work, string description, int maximum = 0, int value = 0, ObservableTaskPool pool = null) {
      this.initialProgress = new ObservableTaskProgress(description, maximum, value);
      this.Initialize(async () => {
        await work(this.progress);
      }, pool);
    }

    public ObservableTask(Func<IProgress<int>, CancellationToken, Task> work, string description, int maximum = 0, int value = 0, ObservableTaskPool pool = null) {
      this.initialProgress = new ObservableTaskProgress(description, maximum, value);
      this.Initialize(async () => {
        await work(this.progress, this.cancellationTokenSource.Token);
      }, pool);
    }

    public ObservableTask(Func<IObservableTaskProgress, CancellationToken, Task> work, string description, int maximum = 0, int value = 0, ObservableTaskPool pool = null) {
      this.initialProgress = new ObservableTaskProgress(description, maximum, value);
      this.Initialize(async () => {
        await work(this.progress, this.cancellationTokenSource.Token);
      }, pool);
    }

    private void Initialize(Func<Task> work, ObservableTaskPool pool) {
      // apply initial progress
      this.RevertProgress();
      this.DoCancel = new RelayCommand(this.Cancel, () => {
        return this.cancellationTokenSource != null && !this.cancellationTokenSource.IsCancellationRequested && this.progress.IsRunning;
      });
      this.work = work;
      this.pool = pool;
      // register task
      if(this.pool != null) {
        this.pool.RegisterTask(this);
      }
    }

    public async Task Run() {
      try {
        // revert progress to make tast restartable
        this.RevertProgress();
        // add and indicate as running
        this.progress.IsRunning = true;
        // create new cancellation token source
        this.cancellationTokenSource = new CancellationTokenSource();
        this.task = this.work.Invoke();
        await this.task;
      } catch(TaskCanceledException) {
      } finally {
        this.progress.IsRunning = false;
      }
    }

    public async Task Schedule() {
      if(this.pool == null) {
        await this.Run();
      } else {
        await this.pool.ScheduleTask(this);
      }
    }

    public void Cancel() {
      if (this.cancellationTokenSource != null && !this.cancellationTokenSource.IsCancellationRequested) {
        this.cancellationTokenSource.Cancel();
      }
    }

    public async Task WaitForCompletion() {
      if(this.progress.IsRunning && this.task != null) {
        await this.task;
      }
    }

    Task IObservableTask.Run() {
      throw new NotImplementedException();
    }

    Task IObservableTask.Schedule() {
      throw new NotImplementedException();
    }

    void IObservableTask.Cancel() {
      throw new NotImplementedException();
    }

    Task IObservableTask.WaitForCompletion() {
      throw new NotImplementedException();
    }
  }
}
