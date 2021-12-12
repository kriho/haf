using HAF.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace HAF {
  public interface IProjectsService : IService {
    IReadOnlyEvent OnProjectsChanged { get; }
    ICompoundState MayChangeProject { get; }
    IRelayCommand<IProject> DoDeleteProject { get; }
    IRelayCommand<IProject> DoLoadProject { get; }
    IRelayCommand DoOpenDirectory { get; }
    IRelayCommand DoRefresh { get; }
    IRelayCommand DoAddProject { get; }
    string EditName { get; set; }
    IRelayCommand<IProject> DoSetDefaultProject { get; }
    IProject CurrentProject { get; }
    IProject DefaultProject { get; }
    IReadOnlyList<IService> ConfiguredServices { get; }
    IReadOnlyObservableCollection<IProject> Projects { get; }
    Task AddProject(string name);
    Task ClearProject();
    void DeleteProject(IProject project);
    Task LoadProject(IProject project);
    void LoadProjects(string defaultProjectName);
    Task SaveProject(IProject project);
    void RegisterService(IService service);
  }
}