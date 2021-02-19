using HAF.Models;
using System.Collections.Generic;
using Telerik.Windows.Data;

namespace HAF {
  public interface IProjectsService : IService {
    RelayCommand<Project> _Delete { get; }
    RelayCommand<Project> _Load { get; }
    RelayCommand _Refresh { get; }
    RelayCommand<Project> _SetDefault { get; }
    Project CurrentProject { get; set; }
    Project DefaultProject { get; set; }
    List<IService> SavedServices { get; }
    RadObservableCollection<Project> Projects { get; }

    void AddProject(string name);
    void ClearProject();
    void DeleteProject(Project project);
    void LoadProject(Project project);
    void LoadProjects(string defaultProjectName);
    void SaveProject(Project project);
  }
}