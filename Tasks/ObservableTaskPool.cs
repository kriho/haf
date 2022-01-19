using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HAF {
  public class ObservableTaskPool: LinkedObservableObject, IObservableTaskPool {
    public string Name { get; private set; }

    public int ParallelExecutionLimit { get; private set; }

    public State IsIdle { get; private set; } = new State(true);

    private readonly ObservableCollection<IWaitingObservableTask> waitingTasks = new ObservableCollection<IWaitingObservableTask>();
    public IReadOnlyObservableCollection<IWaitingObservableTask> WaitingTasks => this.waitingTasks;

    private readonly ObservableCollection<IObservableTask> activeTasks = new ObservableCollection<IObservableTask>();
    public IReadOnlyObservableCollection<IObservableTask> ActiveTasks => this.activeTasks;

    private static TaskScheduler taskScheduler;

    static ObservableTaskPool() {
      ObservableTaskPool.taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
    }

    public ObservableTaskPool(string name, int parallelExecutionLimit) {
      this.Name = name;
      this.Name = name;
      this.ParallelExecutionLimit = parallelExecutionLimit;
    }

    private void ScheduleWaitingTasks() {
      while(this.waitingTasks.Count > 0 && (this.activeTasks.Count < this.ParallelExecutionLimit || this.ParallelExecutionLimit == 0)) {
        this.RunWaitingTask(this.waitingTasks[0] as WaitingObservableTask);
      }
      if(this.activeTasks.Count == 0) {
        this.IsIdle.Value = false;
      }
    }

    private void RunWaitingTask(WaitingObservableTask waitingTask) {
      this.waitingTasks.Remove(waitingTask);
      this.activeTasks.Add(waitingTask.Task);
      waitingTask.Task.Run().ContinueWith(t => {
        if(t.Exception != null) {
          waitingTask.CompletionSource.SetException(t.Exception);
        }
        if(t.IsCanceled) {
          waitingTask.CompletionSource.SetCanceled();
        }
        waitingTask.CompletionSource.SetResult(false);
        this.activeTasks.Remove(waitingTask.Task);
        this.ScheduleWaitingTasks();
      });
    }

    public Task ScheduleTask(IObservableTask task) {
      this.IsIdle.Value = false;
      if((this.ParallelExecutionLimit != 0 && this.activeTasks.Count > this.ParallelExecutionLimit) || this.activeTasks.Contains(task)) {
        // task pool is too busy or task is already scheduled
        var waitingTask = new WaitingObservableTask() {
          Task = task,
          CompletionSource = new TaskCompletionSource<bool>(),
          ScheduledAt = DateTime.Now,
        };
        this.waitingTasks.Add(waitingTask);
        return waitingTask.CompletionSource.Task;
      }
      this.activeTasks.Add(task);
      var result = task.Run();
      result.ContinueWith(t => {
        this.activeTasks.Remove(task);
        this.ScheduleWaitingTasks();
      }, ObservableTaskPool.taskScheduler);
      return result;
    }
  }
}