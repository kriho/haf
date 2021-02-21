using HAF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Telerik.Windows.Data;

namespace HAF {

  [Export(typeof(IProjectsService)), PartCreationPolicy(CreationPolicy.Shared)]
  public class ProjectsService : Service, IProjectsService {

    public ServiceEvent OnProjectsChanged { get; private set; } = new ServiceEvent();

    public ServiceDependency CanChangeProject { get; private set; } = new ServiceDependency();

    public RelayCommand RefreshCommand { get; private set; }

    public RelayCommand<Project> LoadCommand { get; private set; }

    public RelayCommand<Project> DeleteCommand { get; private set; }

    public RelayCommand<Project> SetDefaultCommand { get; private set; }

    public RangeObservableCollection<Project> Projects { get; private set; } = new RangeObservableCollection<Project>();

    public List<IService> ConfiguredServices { get; private set; } = new List<IService>();

    private Project currentProject = null;
    public Project CurrentProject {
      get { return this.currentProject; }
      set {
        if (this.SetValue(ref this.currentProject, value)) {
          foreach (var project in this.Projects) {
            project.IsCurrent = project == value;
          }
          this.LoadCommand.RaiseCanExecuteChanged();
          this.DeleteCommand.RaiseCanExecuteChanged();
          this.OnProjectsChanged.Fire();
        }
      }
    }

    private Project defaultProject = null;
    public Project DefaultProject {
      get { return this.defaultProject; }
      set {
        if (this.SetValue(ref this.defaultProject, value)) {
          foreach (var project in this.Projects) {
            project.IsDefault = project == value;
          }
          this.SetDefaultCommand.RaiseCanExecuteChanged();
          this.OnProjectsChanged.Fire();
        }
      }
    }

    public void LoadProjects(string defaultProjectName) {
      // get potential projects
      var projects = Directory.GetFiles(Backend.ConfigurationDirectory, "*.xml", SearchOption.TopDirectoryOnly)
                     .Where(filePath => {
                       // filter out non-project xmls
                       try {
                         var document = new XmlDocument();
                         document.Load(filePath);
                         if (document.SelectSingleNode("/project") != null) {
                           return true;
                         }
                       } catch { }
                       return false;
                     })
                     .Select(p => new Project() {
                       Name = Path.GetFileNameWithoutExtension(p),
                       FilePath = p,
                     });
      this.Projects.Clear();
      this.Projects.AddRange(projects);
      Project defaultProject = null;
      // add default project if none exist
      if (this.Projects.Count == 0) {
        var project = new Project() {
          Name = "default project",
          FilePath = Path.Combine(Backend.ConfigurationDirectory, "default project.xml"),
        };
        this.SaveProject(project);
        this.Projects.Add(project);
        defaultProject = project;
      } else {
        defaultProject = this.Projects.FirstOrDefault(p => p.Name == defaultProjectName);
      }
      if (defaultProject == null) {
        defaultProject = this.Projects[0];
      }
      this.DefaultProject = defaultProject;
      this.LoadProject(defaultProject);
    }

    public ProjectsService() {
      this.LoadCommand = new RelayCommand<Project>((project) => {
        // load new project
        this.LoadProject(project);
      }, (project) => {
        return this.currentProject != project && this.CanChangeProject;
      });
      this.DeleteCommand = new RelayCommand<Project>((project) => {
        this.DeleteProject(project);
      }, (project) => {
        return this.currentProject != project && this.Projects.Count > 1;
      });
      this.SetDefaultCommand = new RelayCommand<Project>((project) => {
        this.DefaultProject = project;
      }, (project) => {
        return this.defaultProject != project;
      });
      this.RefreshCommand = new RelayCommand(() => {
        // save changes to current project
        this.SaveProject(this.currentProject);
        // reload projects
        this.LoadProjects(this.defaultProject?.Name);
      }, () => {
        return this.CanChangeProject;
      });
      this.CanChangeProject.RegisterUpdate(() => {
        this.LoadCommand.RaiseCanExecuteChanged();
        this.RefreshCommand.RaiseCanExecuteChanged();
      });
      this.Projects.CollectionChanged += (sender, e) => {
        this.DeleteCommand.RaiseCanExecuteChanged();
        this.OnProjectsChanged.Fire();
      };
    }

    public void SaveProject(Project project) {
      var configuration = new Configuration("project");
      foreach (var service in this.ConfiguredServices) {
        service.SaveConfiguration(configuration);
      }
      configuration.SaveToFile(project.FilePath);
    }

    public void LoadProject(Project project) {
      if (!File.Exists(project.FilePath)) {
        return;
      }
      // save current project
      if (this.currentProject != null) {
        this.SaveProject(this.currentProject);
      }
      // load from configuration
      var configuration = Configuration.FromFile(project.FilePath);
      foreach (var service in this.ConfiguredServices) {
        service.LoadConfiguration(configuration);
      }
      // set current project
      this.CurrentProject = project;
    }

    public void ClearProject() {
      foreach (var service in this.ConfiguredServices) {
        service.ClearConfiguration();
      }
    }

    public override void LoadConfiguration(Configuration configuration) {
      var defaultProject = configuration.ReadStringValue("defaultProject", null);
      this.LoadProjects(defaultProject);
    }

    public override void SaveConfiguration(Configuration configuration) {
      if (this.defaultProject != null) {
        configuration.WriteValue("defaultProject", this.defaultProject.Name);
      }
      if (this.CurrentProject != null) {
        this.SaveProject(this.currentProject);
      }
    }

    public void AddProject(string name) {
      this.ClearProject();
      var project = new Project() {
        Name = name,
        FilePath = Path.Combine(Backend.ConfigurationDirectory, name + ".xml"),
      };
      this.Projects.Add(project);
      this.CurrentProject = project;
      this.SaveProject(project);
    }

    public void DeleteProject(Project project) {
      if (this.currentProject == project) {
        return;
      }
      this.Projects.Remove(project);
      File.Delete(project.FilePath);
      if (this.defaultProject == project) {
        this.DefaultProject = this.Projects.FirstOrDefault();
      }
    }
  }
}
