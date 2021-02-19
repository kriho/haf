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

    public enum Dependencies {
      CanChangeProject
    }

    public enum Events {
      ProjectsChanged
    }

    public override int Id {
      get {
        return (int)ServiceId.Projects;
      }
    }

    public RelayCommand _Refresh { get; private set; }

    public RelayCommand<Project> _Load { get; private set; }

    public RelayCommand<Project> _Delete { get; private set; }

    public RelayCommand<Project> _SetDefault { get; private set; }

    public RadObservableCollection<Project> Projects { get; private set; } = new RadObservableCollection<Project>();

    public List<IService> SavedServices { get; private set; } = new List<IService>();

    private Project currentProject = null;
    public Project CurrentProject {
      get { return this.currentProject; }
      set {
        if (this.SetValue(ref this.currentProject, value)) {
          foreach (var project in this.Projects) {
            project.IsCurrent = project == value;
          }
          this._Load.RaiseCanExecuteChanged();
          this._Delete.RaiseCanExecuteChanged();
          Messenger.Default.Send(new Messages.ProjectChanged());
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
          this._SetDefault.RaiseCanExecuteChanged();
          Messenger.Default.Send(new Messages.ProjectChanged());
        }
      }
    }

    public void LoadProjects(string defaultProjectName) {
      this.Projects.SuspendNotifications();
      this.Projects.Clear();
      // get potential projects
      var projects = Directory.GetFiles(Backend.ConfigurationDirectory, "*.xml", SearchOption.TopDirectoryOnly)
                     .Select(p => new Project() {
                       Name = Path.GetFileNameWithoutExtension(p),
                       FilePath = p,
                     });
      // filter out non-project xmls
      foreach (var project in projects) {
        try {
          var document = new XmlDocument();
          document.Load(project.FilePath);
          if (document.SelectSingleNode("/project") != null) {
            this.Projects.Add(project);
          }
        } catch { }
      }
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
      this.Projects.ResumeNotifications();
      this.DefaultProject = defaultProject;
      this.LoadProject(defaultProject);
    }

    public ProjectsService() {
      this._Load = new RelayCommand<Project>((project) => {
        // load new project
        this.LoadProject(project);
      }, (project) => {
        return this.currentProject != project && this.GetDependency(Dependencies.CanChangeProject);
      });
      this._Delete = new RelayCommand<Project>((project) => {
        this.DeleteProject(project);
      }, (project) => {
        return this.currentProject != project && this.Projects.Count > 1;
      });
      this._SetDefault = new RelayCommand<Project>((project) => {
        this.DefaultProject = project;
      }, (project) => {
        return this.defaultProject != project;
      });
      this._Refresh = new RelayCommand(() => {
        // save changes to current project
        this.SaveProject(this.currentProject);
        // reload projects
        this.LoadProjects(this.defaultProject?.Name);
      }, () => {
        return this.GetDependency(Dependencies.CanChangeProject);
      });
      this.RegisterDependency((int)Dependencies.CanChangeProject, () => {
        this._Load.RaiseCanExecuteChanged();
        this._Delete.RaiseCanExecuteChanged();
        this._Refresh.RaiseCanExecuteChanged();
      });
      this.Projects.CollectionChanged += (sender, e) => {
        this._Delete.RaiseCanExecuteChanged();
        this.FireEvent(Events.ProjectsChanged);
      };
      this.FireEvent(Events.ProjectsChanged);
    }

    public void SaveProject(Project project) {
      var storage = new Settings("project");
      foreach (var service in this.SavedServices) {
        service.Save(storage);
      }
      storage.SaveToFile(project.FilePath);
    }

    public void LoadProject(Project project) {
      if (!File.Exists(project.FilePath)) {
        return;
      }
      // save current project
      if (this.currentProject != null) {
        this.SaveProject(this.currentProject);
      }
      // clear project
      this.ClearProject();
      // load from storage
      var storage = Settings.FromFile(project.FilePath);
      foreach (var service in this.SavedServices) {
        service.Load(storage);
      }
      // set current project
      this.CurrentProject = project;
    }

    public void ClearProject() {
      foreach (var service in this.SavedServices) {
        service.Clear();
      }
    }

    public override void Load(Settings storage) {
      var defaultProject = storage.ReadValue("defaultProject");
      this.LoadProjects(defaultProject);
    }

    public override void Save(Settings storage) {
      if (this.defaultProject != null) {
        storage.WriteValue("defaultProject", this.defaultProject.Name);
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
