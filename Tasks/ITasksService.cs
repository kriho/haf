using System.Collections.ObjectModel;

namespace HAF {
  public interface ITasksService: IService {
    IObservableTaskPool this[string name] { get; }
    IReadOnlyObservableCollection<IObservableTaskPool> TaskPools { get; }
    IObservableTaskPool AddTaskPool(string name, int parallelExecutionLimit);
  }
}