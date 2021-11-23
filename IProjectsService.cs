using HAF.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace HAF {
  public interface IProjectsService : IService {
    Event OnProjectsChanged { get; }
    ICompoundState MayChangeProject { get; }
    RelayCommand<Project> DoDeleteProject { get; }
    RelayCommand<Project> DoLoadProject { get; }
    RelayCommand DoOpenDirectory { get; }
    RelayCommand DoRefresh { get; }
    RelayCommand<Project> DoSetDefaultProject { get; }
    Project CurrentProject { get; }
    Project DefaultProject { get; }
    List<IService> ConfiguredServices { get; }
    IReadOnlyObservableCollection<Project> Projects { get; }
    Task AddProject(string name);
    Task ClearProject();
    void DeleteProject(Project project);
    void LoadProject(Project project);
    void LoadProjects(string defaultProjectName);
    void SaveProject(Project project);
  }
}