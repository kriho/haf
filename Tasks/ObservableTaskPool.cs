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

    private readonly ObservableCollection<IObservableTask> activeTasks = new ObservableCollection<IObservableTask>();
    public IReadOnlyObservableCollection<IObservableTask> ActiveTasks => this.activeTasks;

    public ObservableTaskPool(string name, bool allowParallelExecution) {
      this.Name = name;
      this.Link = name;
      this.AllowParallelExecution = allowParallelExecution;
    }

    public async Task ScheduleTask(IObservableTask task) {
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