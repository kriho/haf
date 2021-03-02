using System.Collections.ObjectModel;

namespace HAF {
  public interface ITasksService: IService {
    ObservableTaskPool this[string name] { get; }
    ObservableCollection<ObservableTaskPool> TaskPools { get; set; }
    void AddTaskPool(string name, bool allowParallelExecution);
  }
}