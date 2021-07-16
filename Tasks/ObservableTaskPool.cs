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

    public LinkedState CanScheduleTasks { get; private set; } = new LinkedState();

    private NotifyCollection<ObservableTask> tasks = new NotifyCollection<ObservableTask>();
    public IReadOnlyNotifyCollection<ObservableTask> Tasks => this.tasks;


    public ObservableTaskPool(string name, bool allowParallelExecution) {
      this.Name = name;
      this.AllowParallelExecution = allowParallelExecution;
    }

    public ObservableTaskPool() {
      this.Link = this.Name;
    }

    public void ScheduleTask(ObservableTask task) {
      if(this.Tasks.Count > 0 && !this.AllowParallelExecution) {
        throw new Exception("task group does not allow parallel execution");
      }
      task.AssignPool(this);
      this.tasks.Add(task);
      if(!this.AllowParallelExecution) {
        this.CanScheduleTasks.Value = false;
      }
    }

    public void UnscheduleTask(ObservableTask task) {
      if(this.tasks.Contains(task)) {
        this.tasks.Remove(task);
        if (this.AllowParallelExecution || this.Tasks.Count == 0) {
          this.CanScheduleTasks.Value = true;
        }
      }
    }
  }
}