using HAF.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.Windows.Data;

namespace HAF {
  public interface IProjectsService : IService {
    ServiceEvent OnProjectsChanged { get; }
    ServiceDependency CanChangeProject { get; }
    RelayCommand<Project> DeleteCommand { get; }
    RelayCommand<Project> LoadCommand { get; }
    RelayCommand RefreshCommand { get; }
    RelayCommand<Project> SetDefaultCommand { get; }
    Project CurrentProject { get; set; }
    Project DefaultProject { get; set; }
    List<IService> ConfiguredServices { get; }
    RangeObservableCollection<Project> Projects { get; }

    void AddProject(string name);
    void ClearProject();
    void DeleteProject(Project project);
    void LoadProject(Project project);
    void LoadProjects(string defaultProjectName);
    void SaveProject(Project project);
  }
}