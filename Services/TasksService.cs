using HAF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml;

namespace HAF {

  [Export(typeof(ITasksService)), PartCreationPolicy(CreationPolicy.Shared)]
  public class TasksService: Service, ITasksService {

    private NotifyCollection<ObservableTaskPool> taskPools = new NotifyCollection<ObservableTaskPool>();
    public IReadOnlyNotifyCollection<ObservableTaskPool> TaskPools => this.taskPools;

    public ObservableTaskPool this[string name] {
      get { return this.TaskPools.FirstOrDefault(t => t.Name == name); }
    }

    public void AddTaskPool(string name, bool allowParallelExecution) {
      if (this.TaskPools.Any(t => t.Name == name)) {
        throw new Exception($"the task pool {name} already exists");
      }
      this.taskPools.Add(new ObservableTaskPool(name, allowParallelExecution));
    }
  }
}
