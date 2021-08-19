using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HAF {
  public interface IObservableTaskPool {
    string Name { get; }
    bool AllowParallelExecution { get; }
    LinkedState IsIdle { get; }
    IReadOnlyNotifyCollection<IObservableTask> ActiveTasks { get; }
    IReadOnlyNotifyCollection<IObservableTask> RegisteredTasks { get; }
  }
}