using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HAF {
  public class ObservableTaskPool {
    private static ObservableTaskPool defaultInstance = null;
    public static ObservableTaskPool Default {
      get {
        // initialize when needed
        if (defaultInstance == null) {
          defaultInstance = new ObservableTaskPool();
        }
        return defaultInstance;
      }
    }

    public ObservableCollection<ObservableTask> Tasks { get; private set; }

    public ObservableTaskPool() {
      this.Tasks = new ObservableCollection<ObservableTask>();
    }

    internal void RegisterTask(ObservableTask task) {
      this.Tasks.Add(task);
    }

    internal void UnregisterTask(ObservableTask task) {
      this.Tasks.Remove(task);
    }
  }
}
