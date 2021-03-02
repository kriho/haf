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
    public string Name { get; set; }

    public bool AllowParallelExecution { get; set; }

    public LinkedState CanScheduleTasks { get; private set; } = new LinkedState();

    public ObservableCollection<ObservableTask> Tasks { get; private set; } = new ObservableCollection<ObservableTask>();

    protected override void Initialize() {
      this.Link = this.Name;
    }

    public void ScheduleTask(ObservableTask task) {
      if(this.Tasks.Count > 0 && !this.AllowParallelExecution) {
        throw new Exception("task group does not allow parallel execution");
      }
      task.AssignPool(this);
      this.Tasks.Add(task);
      if(!this.AllowParallelExecution) {
        this.CanScheduleTasks.Value = false;
      }
    }

    public void UnscheduleTask(ObservableTask task) {
      if(this.Tasks.Contains(task)) {
        this.Tasks.Remove(task);
        if (this.AllowParallelExecution || this.Tasks.Count == 0) {
          this.CanScheduleTasks.Value = true;
        }
      }
    }
  }
}