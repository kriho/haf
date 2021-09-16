using HAF.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HAF {
  public interface IProjectsService : IService {
    LinkedEvent OnProjectsChanged { get; }
    LinkedDependency MayChangeProject { get; }
    RelayCommand<Project> DoDeleteProject { get; }
    RelayCommand<Project> DoLoadProject { get; }
    RelayCommand DoOpenDirectory { get; }
    RelayCommand DoRefresh { get; }
    RelayCommand<Project> DoSetDefaultProject { get; }
    Project CurrentProject { get; }
    Project DefaultProject { get; }
    List<IService> ConfiguredServices { get; }
    IReadOnlyRangeObservableCollection<Project> Projects { get; }

    void AddProject(string name);
    void ClearProject();
    void DeleteProject(Project project);
    void LoadProject(Project project);
    void LoadProjects(string defaultProjectName);
    void SaveProject(Project project);
  }
}