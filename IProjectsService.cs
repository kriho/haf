using HAF.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace HAF {
  public interface IProjectsService : IService {
    /// <summary>
    /// Event that fires when the list of available projects changes.
    /// </summary>
    IReadOnlyEvent OnProjectsChanged { get; }

    /// <summary>
    /// Can the current project be changed.
    /// </summary>
    ICompoundState MayChangeProject { get; }

    /// <summary>
    /// Delete provided project.
    /// </summary>
    IRelayCommand<IProject> DoDeleteProject { get; }

    /// <summary>
    /// Load provided project.
    /// </summary>
    IRelayCommand<IProject> DoLoadProject { get; }

    /// <summary>
    /// Set provided project as default.
    /// </summary>
    IRelayCommand<IProject> DoSetDefaultProject { get; }

    /// <summary>
    /// Open the directory containing all project files.
    /// </summary>
    IRelayCommand DoOpenDirectory { get; }

    /// <summary>
    /// Refresh available projects.
    /// </summary>
    IRelayCommand DoRefresh { get; }

    /// <summary>
    /// Add new project with name provided by <c>EditName.</c>
    /// </summary>
    IRelayCommand DoAddProject { get; }

    /// <summary>
    /// Save current project.
    /// </summary>
    IRelayCommand DoSaveProject { get; }

    /// <summary>
    /// Name used for creating new projects.
    /// </summary>
    string EditName { get; set; }

    /// <summary>
    /// Project that is currently loaded.
    /// </summary>
    IProject CurrentProject { get; }

    /// <summary>
    /// Project that is loaded on application start.
    /// </summary>
    IProject DefaultProject { get; }

    /// <summary>
    /// Services with project-scoped configuration.
    /// </summary>
    IReadOnlyList<IService> ConfiguredServices { get; }

    /// <summary>
    /// Available projects.
    /// </summary>
    IReadOnlyObservableCollection<IProject> Projects { get; }

    /// <summary>
    /// Add new project with provided name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task AddProject(string name);

    /// <summary>
    /// Clear configuration of all configured services.
    /// </summary>
    Task ClearProject();

    /// <summary>
    /// Delete the provided project.
    /// </summary>
    /// <param name="project">Name of the project.</param>
    void DeleteProject(IProject project);

    /// <summary>
    /// Load configuration of provided project from file.
    /// </summary>
    Task LoadProject(IProject project);

    /// <summary>
    /// Load available projects and load a default project.
    /// </summary>
    /// <param name="defaultProjectName">Name of the default project.</param>
    /// <returns></returns>
    Task LoadProjects(string defaultProjectName);

    /// <summary>
    /// Save configuration of provided project to file.
    /// </summary>
    Task SaveProject(IProject project);

    /// <summary>
    /// Register service with project-scoped configuration.
    /// </summary>
    /// <param name="service"></param>
    void RegisterService(IService service);
  }
}