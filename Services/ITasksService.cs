using System.Collections.ObjectModel;

namespace HAF {
  public interface ITasksService: IService {
    ObservableTaskPool this[string name] { get; }
    IReadOnlyNotifyCollection<ObservableTaskPool> TaskPools { get; }
    void AddTaskPool(string name, bool allowParallelExecution);
  }
}