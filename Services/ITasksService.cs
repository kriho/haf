using System.Collections.ObjectModel;

namespace HAF {
  public interface ITasksService: IService {
    IObservableTaskPool this[string name] { get; }
    IReadOnlyNotifyCollection<IObservableTaskPool> TaskPools { get; }
    void AddTaskPool(string name, bool allowParallelExecution);
  }
}