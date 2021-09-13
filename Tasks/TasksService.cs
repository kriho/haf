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

    private readonly ObservableCollection<IObservableTaskPool> taskPools = new ObservableCollection<IObservableTaskPool>();
    public IReadOnlyObservableCollection<IObservableTaskPool> TaskPools => this.taskPools;

    public IObservableTaskPool this[string name] {
      get { return this.TaskPools.FirstOrDefault(t => t.Name == name); }
    }

    public IObservableTaskPool AddTaskPool(string name, int parallelExecutionLimit = 0) {
      if (this.TaskPools.Any(t => t.Name == name)) {
        throw new Exception($"the task pool {name} already exists");
      }
      var pool = new ObservableTaskPool(name, parallelExecutionLimit);
      this.taskPools.Add(pool);
      return pool;
    }
  }
}
