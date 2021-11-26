using HAF.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace HAF {
  public interface IProjectsService : IService {
    IReadOnlyEvent OnProjectsChanged { get; }
    ICompoundState MayChangeProject { get; }
    IRelayCommand<Project> DoDeleteProject { get; }
    IRelayCommand<Project> DoLoadProject { get; }
    IRelayCommand DoOpenDirectory { get; }
    IRelayCommand DoRefresh { get; }
    IRelayCommand DoAddProject { get; }
    string EditName { get; set; }
    IRelayCommand<Project> DoSetDefaultProject { get; }
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