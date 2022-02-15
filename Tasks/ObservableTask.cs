using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HAF {

  public class ObservableTask: ObservableObject, IObservableTask {
    public IObservableTaskPool Pool { get; private set; }

    private readonly ObservableTaskProgress progress = new ObservableTaskProgress();
    public IObservableTaskProgress Progress => this.progress;

    private readonly ObservableTaskProgress initialProgress;
    
    private Func<Task> work;

    private CancellationTokenSource cancellationTokenSource;

    private State isRunning = new State(false);
    public IReadOnlyState IsRunning => this.isRunning;

    public bool IsCancelled {
      get { return this.cancellationTokenSource?.IsCancellationRequested == true; }
    }

    private void RevertProgress() {
      this.progress.IsIndeterminate = this.initialProgress.IsIndeterminate;
      this.progress.Value = this.initialProgress.Value;
      this.progress.Maximum = this.initialProgress.Maximum;
      this.progress.Description = this.initialProgress.Description;
    }

    public RelayCommand DoCancel { get; private set; }

    public ObservableTask(Func<Task> work, string description, IObservableTaskPool pool = null) {
      this.initialProgress = new ObservableTaskProgress(description);
      this.Initialize(work, pool);
    }

    public ObservableTask(Func<CancellationToken, Task> work, string description, IObservableTaskPool pool = null) {
      this.initialProgress = new ObservableTaskProgress(description);
      this.Initialize(async () => {
        await work(this.cancellationTokenSource.Token);
      }, pool);
    }

    public ObservableTask(Func<IProgress<int>, Task> work, string description, int maximum = 0, int value = 0, IObservableTaskPool pool = null) {
      this.initialProgress = new ObservableTaskProgress(description, maximum, value);
      this.Initialize(async () => {
        await work(this.progress);
      }, pool);
    }

    public ObservableTask(Func<IObservableTaskProgress, Task> work, string description = "", int maximum = 0, int value = 0, IObservableTaskPool pool = null) {
      this.initialProgress = new ObservableTaskProgress(description, maximum, value);
      this.Initialize(async () => {
        await work(this.progress);
      }, pool);
    }

    public ObservableTask(Func<IProgress<int>, CancellationToken, Task> work, string description, int maximum = 0, int value = 0, IObservableTaskPool pool = null) {
      this.initialProgress = new ObservableTaskProgress(description, maximum, value);
      this.Initialize(async () => {
        await work(this.progress, this.cancellationTokenSource.Token);
      }, pool);
    }

    public ObservableTask(Func<IObservableTaskProgress, CancellationToken, Task> work, string description = "", int maximum = 0, int value = 0, IObservableTaskPool pool = null) {
      this.initialProgress = new ObservableTaskProgress(description, maximum, value);
      this.Initialize(async () => {
        await work(this.progress, this.cancellationTokenSource.Token);
      }, pool);
    }

    private void Initialize(Func<Task> work, IObservableTaskPool pool) {
      // apply initial progress
      this.RevertProgress();
      this.DoCancel = new RelayCommand(this.Cancel, this.isRunning);
      this.work = work;
      this.Pool = pool;
      Core.Container.ComposeParts(this);
    }

    public async Task Run() {
      try {
        // revert progress to make tast restartable
        this.RevertProgress();
        // add and indicate as running
        this.isRunning.Value = true;
        // create new cancellation token source
        this.cancellationTokenSource = new CancellationTokenSource();
        await this.work.Invoke();
      } catch(TaskCanceledException) {
      } finally {
        this.isRunning.Value = false;
      }
    }

    public Task Schedule() {
      if(this.Pool == null) {
        return this.Run();
      } else {
        return this.Pool.ScheduleTask(this);
      }
    }

    public void Cancel() {
      if (this.cancellationTokenSource != null && !this.cancellationTokenSource.IsCancellationRequested) {
        this.cancellationTokenSource.Cancel();
      }
    }
  }
}
