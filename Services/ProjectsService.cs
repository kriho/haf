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

    public LinkedEvent OnProjectsChanged { get; private set; } = new LinkedEvent();

    public LinkedDependency MayChangeProject { get; private set; } = new LinkedDependency();

    public RelayCommand DoRefresh { get; private set; }

    public RelayCommand<Project> DoLoadProject { get; private set; }

    public RelayCommand<Project> DoDeleteProject { get; private set; }

    public RelayCommand<Project> DoSetDefaultProject { get; private set; }

    public RelayCommand DoOpenDirectory { get; private set; }

    private RangeNotifyCollection<Project> projects = new RangeNotifyCollection<Project>();
    public IReadOnlyNotifyCollection<Project> Projects => this.projects;

    public List<IService> ConfiguredServices { get; private set; } = new List<IService>();

    private Project currentProject = null;
    public Project CurrentProject {
      get { return this.currentProject; }
      set {
        if (this.SetValue(ref this.currentProject, value)) {
          foreach (var project in this.projects) {
            project.IsCurrent = project == value;
          }
          this.DoLoadProject.RaiseCanExecuteChanged();
          this.DoDeleteProject.RaiseCanExecuteChanged();
          this.OnProjectsChanged.Fire();
        }
      }
    }

    private Project defaultProject = null;
    public Project DefaultProject {
      get { return this.defaultProject; }
      set {
        if (this.SetValue(ref this.defaultProject, value)) {
          foreach (var project in this.projects) {
            project.IsDefault = project == value;
          }
          this.DoSetDefaultProject.RaiseCanExecuteChanged();
          this.OnProjectsChanged.Fire();
        }
      }
    }

    public void LoadProjects(string defaultProjectName) {
      // get potential projects
      var projects = Directory.GetFiles(Configuration.ConfigurationDirectory, "*.xml", SearchOption.TopDirectoryOnly)
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
      this.projects.Clear();
      this.projects.AddRange(projects);
      Project defaultProject = null;
      // add default project if none exist
      if (this.projects.Count == 0) {
        var project = new Project() {
          Name = "default project",
          FilePath = Path.Combine(Configuration.ConfigurationDirectory, "default project.xml"),
        };
        this.SaveProject(project);
        this.projects.Add(project);
        defaultProject = project;
      } else {
        defaultProject = this.projects.FirstOrDefault(p => p.Name == defaultProjectName);
      }
      if (defaultProject == null) {
        defaultProject = this.projects[0];
      }
      this.DefaultProject = defaultProject;
      this.LoadProject(defaultProject);
    }

    public ProjectsService() {
      this.DoLoadProject = new RelayCommand<Project>((project) => {
        // load new project
        this.LoadProject(project);
      }, (project) => {
        return this.currentProject != project && this.MayChangeProject;
      });
      this.DoDeleteProject = new RelayCommand<Project>((project) => {
        this.DeleteProject(project);
      }, (project) => {
        return this.currentProject != project && this.projects.Count > 1;
      });
      this.DoSetDefaultProject = new RelayCommand<Project>((project) => {
        this.DefaultProject = project;
      }, (project) => {
        return this.defaultProject != project;
      });
      this.DoRefresh = new RelayCommand(() => {
        // save changes to current project
        this.SaveProject(this.currentProject);
        // reload projects
        this.LoadProjects(this.defaultProject?.Name);
      }, () => {
        return this.MayChangeProject;
      });
      this.DoOpenDirectory = new RelayCommand(() => {
        System.Diagnostics.Process.Start(Configuration.ConfigurationDirectory);
      });
      this.MayChangeProject.RegisterUpdate(() => {
        this.DoLoadProject.RaiseCanExecuteChanged();
        this.DoRefresh.RaiseCanExecuteChanged();
      });
      this.projects.CollectionChanged += (sender, e) => {
        this.DoDeleteProject.RaiseCanExecuteChanged();
        this.OnProjectsChanged.Fire();
      };
    }

    public void SaveProject(Project project) {
      var configuration = new ServiceConfiguration("project");
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
      var configuration = ServiceConfiguration.FromFile(project.FilePath);
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

    public override void LoadConfiguration(ServiceConfiguration configuration) {
      var defaultProject = configuration.ReadStringValue("defaultProject", null);
      this.LoadProjects(defaultProject);
    }

    public override void SaveConfiguration(ServiceConfiguration configuration) {
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
        FilePath = Path.Combine(Configuration.ConfigurationDirectory, name + ".xml"),
      };
      this.projects.Add(project);
      this.CurrentProject = project;
      this.SaveProject(project);
    }

    public void DeleteProject(Project project) {
      if (this.currentProject == project) {
        return;
      }
      this.projects.Remove(project);
      File.Delete(project.FilePath);
      if (this.defaultProject == project) {
        this.DefaultProject = this.projects.FirstOrDefault();
      }
    }
  }
}
