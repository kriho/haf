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

namespace HAF {

  [Export(typeof(IProjectsService)), PartCreationPolicy(CreationPolicy.Shared)]
  public class ProjectsService: Service, IProjectsService {
    private Event onProjectsChanged = new Event(nameof(OnProjectsChanged));
    public IReadOnlyEvent OnProjectsChanged => this.onProjectsChanged;

    public ICompoundState MayChangeProject { get; private set; } = new CompoundState();

    public IRelayCommand DoRefresh { get; private set; }

    public IRelayCommand<IProject> DoLoadProject { get; private set; }

    public IRelayCommand<IProject> DoDeleteProject { get; private set; }

    public IRelayCommand<IProject> DoSetDefaultProject { get; private set; }

    public IRelayCommand DoOpenDirectory { get; private set; }

    public IRelayCommand DoAddProject { get; private set; }

    private readonly RangeObservableCollection<IProject> projects = new RangeObservableCollection<IProject>();
    public IReadOnlyObservableCollection<IProject> Projects => this.projects;

    private List<IService> configuredServices = new List<IService>();
    public IReadOnlyList<IService> ConfiguredServices => this.configuredServices;

    private IProject currentProject = null;
    public IProject CurrentProject {
      get { return this.currentProject; }
      set {
        if(this.SetValue(ref this.currentProject, value)) {
          foreach(var project in this.projects) {
            project.IsCurrent = project == value;
          }
          this.DoLoadProject.RaiseCanExecuteChanged();
          this.DoDeleteProject.RaiseCanExecuteChanged();
          this.onProjectsChanged.Fire();
        }
      }
    }

    private IProject defaultProject = null;
    public IProject DefaultProject {
      get { return this.defaultProject; }
      set {
        if(this.SetValue(ref this.defaultProject, value)) {
          foreach(var project in this.projects) {
            project.IsDefault = project == value;
          }
          this.DoSetDefaultProject.RaiseCanExecuteChanged();
          this.onProjectsChanged.Fire();
        }
      }
    }

    private string editName = null;
    public string EditName {
      get { return this.editName; }
      set {
        if(this.SetValue(ref this.editName, value)) {
          this.DoAddProject.RaiseCanExecuteChanged();
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
                         if(document.SelectSingleNode("/project") != null) {
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
      IProject defaultProject = null;
      // add default project if none exist
      if(this.projects.Count == 0) {
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
      if(defaultProject == null) {
        defaultProject = this.projects[0];
      }
      this.DefaultProject = defaultProject;
      this.LoadProject(defaultProject);
    }

    public ProjectsService() {
      this.DoLoadProject = new RelayCommand<IProject>((project) => {
        // load new project
        this.LoadProject(project);
      }, (project) => {
        return this.currentProject != project && this.MayChangeProject.Value;
      });
      this.DoDeleteProject = new RelayCommand<IProject>((project) => {
        this.DeleteProject(project);
      }, (project) => {
        return this.currentProject != project && this.projects.Count > 1;
      });
      this.DoSetDefaultProject = new RelayCommand<IProject>((project) => {
        this.DefaultProject = project;
      }, (project) => {
        return this.defaultProject != project;
      });
      this.DoRefresh = new RelayCommand(() => {
        // save changes to current project
        this.SaveProject(this.currentProject);
        // reload projects
        this.LoadProjects(this.defaultProject?.Name);
      }, this.MayChangeProject);
      this.DoOpenDirectory = new RelayCommand(() => {
        System.Diagnostics.Process.Start(Configuration.ConfigurationDirectory);
      });
      this.MayChangeProject.RegisterUpdate(() => {
        this.DoLoadProject.RaiseCanExecuteChanged();
      });
      this.DoAddProject = new RelayCommand(async () => {
        await this.AddProject(this.editName);
        this.EditName = "";
      }, () => {
        return !string.IsNullOrWhiteSpace(this.editName) && !this.projects.Any(p => p.Name == this.editName);
      });
      this.projects.CollectionChanged += (sender, e) => {
        this.DoDeleteProject.RaiseCanExecuteChanged();
        this.onProjectsChanged.Fire();
      };
    }

    public async Task SaveProject(IProject project) {
      var configuration = new ServiceConfiguration("project");
      foreach(var service in this.ConfiguredServices) {
        await service.SaveConfiguration(configuration);
      }
      configuration.SaveToFile(project.FilePath);
    }

    public async Task LoadProject(IProject project) {
      if(!File.Exists(project.FilePath)) {
        return;
      }
      // save current project
      if(this.currentProject != null) {
        await this.SaveProject(this.currentProject);
      }
      // clear project
      await this.ClearProject();
      // load project from configuration
      var configuration = ServiceConfiguration.FromFile(project.FilePath);
      foreach(var service in this.ConfiguredServices) {
        await service.LoadConfiguration(configuration);
      }
      // set current project
      this.CurrentProject = project;
    }

    public async Task ClearProject() {
      foreach(var service in this.ConfiguredServices) {
        await service.Reset();
      }
    }

    public override Task LoadConfiguration(ServiceConfiguration configuration) {
      var defaultProject = configuration.ReadValue("defaultProject", null);
      this.LoadProjects(defaultProject);
      return Task.CompletedTask;
    }

    public override async Task SaveConfiguration(ServiceConfiguration configuration) {
      configuration.WriteValue("defaultProject", this.defaultProject?.Name);
      if(this.CurrentProject != null) {
        await this.SaveProject(this.currentProject);
      }
    }

    public async Task AddProject(string name) {
      await this.ClearProject();
      var project = new Project() {
        Name = name,
        FilePath = Path.Combine(Configuration.ConfigurationDirectory, name + ".xml"),
      };
      this.projects.Add(project);
      this.CurrentProject = project;
      await this.SaveProject(project);
    }

    public void DeleteProject(IProject project) {
      if(this.currentProject == project) {
        return;
      }
      this.projects.Remove(project);
      File.Delete(project.FilePath);
      if(this.defaultProject == project) {
        this.DefaultProject = this.projects.FirstOrDefault();
      }
    }

    public void RegisterService(IService service) {
      if(this.configuredServices.Contains(service)) {
        throw new InvalidOperationException($"the service of type {service.GetType().Name} was already registered");
      }
      this.configuredServices.Add(service);
    }
  }
}
