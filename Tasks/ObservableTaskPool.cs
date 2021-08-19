using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HAF {
  public class ObservableTaskPool: LinkedObject {
    public string Name { get; private set; }

    public bool AllowParallelExecution { get; private set; }

    public LinkedState IsIdle { get; private set; } = new LinkedState(true);

    private NotifyCollection<ObservableTask> activeTasks = new NotifyCollection<ObservableTask>();
    public IReadOnlyNotifyCollection<ObservableTask> ActiveTasks => this.activeTasks;

    private NotifyCollection<ObservableTask> registeredTasks = new NotifyCollection<ObservableTask>();
    public IReadOnlyNotifyCollection<ObservableTask> RegisteredTasks => this.registeredTasks;

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