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
    int ParallelExecutionLimit { get; }
    State IsIdle { get; }
    IReadOnlyObservableCollection<IWaitingObservableTask> WaitingTasks { get; }
    IReadOnlyObservableCollection<IObservableTask> ActiveTasks { get; }
    Task ScheduleTask(IObservableTask task);
  }
}