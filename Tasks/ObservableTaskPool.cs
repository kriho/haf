using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HAF {
  public class ObservableTaskPool: LinkedObject, IObservableTaskPool {
    public string Name { get; private set; }

    public bool AllowParallelExecution { get; private set; }

    public LinkedState IsIdle { get; private set; } = new LinkedState(true);

    private NotifyCollection<IObservableTask> activeTasks = new NotifyCollection<IObservableTask>();
    public IReadOnlyNotifyCollection<IObservableTask> ActiveTasks => this.activeTasks;

    private NotifyCollection<IObservableTask> registeredTasks = new NotifyCollection<IObservableTask>();
    public IReadOnlyNotifyCollection<IObservableTask> RegisteredTasks => this.registeredTasks;

    public ObservableTaskPool(string name, bool allowParallelExecution) {
      this.Name = name;
      this.Link = name;
      this.AllowParallelExecution = allowParallelExecution;
    }

    internal void RegisterTask(ObservableTask task) {
      if (!this.registeredTasks.Contains(task)) {
        this.registeredTasks.Add(task);
      }
    }

    internal async Task ScheduleTask(ObservableTask task) {
      this.IsIdle.Value = false;
      if (!this.AllowParallelExecution) {
        await Task.WhenAll(this.activeTasks.Select(t => t.WaitForCompletion()));
      }
      if(this.activeTasks.Contains(task)) {
        await task.WaitForCompletion();
      }
      this.activeTasks.Add(task);
      try {
        await task.Run();
      } finally {
        this.activeTasks.Remove(task);
      }
      this.IsIdle.Value = this.activeTasks.Count == 0;
    }
  }
}