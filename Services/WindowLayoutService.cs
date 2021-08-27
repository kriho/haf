using HAF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {

  [Export(typeof(IWindowLayoutService)), PartCreationPolicy(CreationPolicy.Shared)]
  public class WindowLayoutService : Service, IWindowLayoutService {

    public LinkedDependency MayChangeWindowLayout { get; private set; } = new LinkedDependency();

    public LinkedEvent OnActiveWindowLayoutChanged { get; private set; } = new LinkedEvent(nameof(OnActiveWindowLayoutChanged));

    [Import]
#pragma warning disable CS0649 // imported by MEF
    private IDockingWindowService dockingWindow;
#pragma warning restore CS0649

    public RelayCommand<WindowLayout> DoLoad { get; private set; }

    public RelayCommand<WindowLayout> DoSave { get; private set; }

    public RelayCommand<WindowLayout> DoDelete { get; private set; }

    public RelayCommand<WindowLayout> DoSetDefault { get; private set; }

    public RelayCommand<PaneMeta> DoShowPane { get; private set; }

    private ObservableCollection<WindowLayout> windowLayouts = new ObservableCollection<WindowLayout>();
    public IReadOnlyObservableCollection<WindowLayout> WindowLayouts => this.windowLayouts;

    private List<WindowLayout> defaultindowLayouts = new List<WindowLayout>();
    public IReadOnlyCollection<WindowLayout> DefaultWindowLayouts => this.defaultindowLayouts;

    public ObservableCollection<PaneMeta> AvailablePanes { get; private set; } = new ObservableCollection<PaneMeta>();

    private WindowLayout activeWindowLayout = null;
    public WindowLayout ActiveWindowLayout {
      get { return this.activeWindowLayout; }
      set {
        if (this.SetValue(ref this.activeWindowLayout, value)) {
          foreach (var windowLayout in this.WindowLayouts) {
            windowLayout.IsCurrent = windowLayout == value;
          }
          this.DoLoad.RaiseCanExecuteChanged();
          this.DoDelete.RaiseCanExecuteChanged();
          this.OnActiveWindowLayoutChanged.Fire();
        }
      }
    }

    private WindowLayout defaultWindowLayout = null;
    public WindowLayout DefaultWindowLayout {
      get { return this.defaultWindowLayout; }
      set {
        if (this.SetValue(ref this.defaultWindowLayout, value)) {
          foreach (var project in this.WindowLayouts) {
            project.IsDefault = project == value;
          }
          this.DoSetDefault.RaiseCanExecuteChanged();
        }
      }
    }

    public WindowLayoutService() {
      this.DoLoad = new RelayCommand<Models.WindowLayout>((windowLayout) => {
        this.LoadWindowLayout(windowLayout);
      }, (windowLayout) => {
        return this.MayChangeWindowLayout;
      });
      this.DoDelete = new RelayCommand<Models.WindowLayout>((windowLayout) => {
        this.windowLayouts.Remove(windowLayout);
      });
      this.DoSave = new RelayCommand<Models.WindowLayout>((windowLayout) => {
        windowLayout.Layout = this.dockingWindow.GetWindowLayout();
      });
      this.DoShowPane = new RelayCommand<PaneMeta>((meta) => {
        this.dockingWindow.ShowPane(meta.Name, meta.Type, meta.CanUserClose);
      });
      this.DoSetDefault = new RelayCommand<Models.WindowLayout>((windowLayout) => {
        this.DefaultWindowLayout = windowLayout;
      }, (windowLayout) => {
        return this.defaultWindowLayout != windowLayout;
      });
      this.MayChangeWindowLayout.RegisterUpdate(() => {
        this.DoLoad.RaiseCanExecuteChanged();
      });
    }

    private void LoadWindowLayout(WindowLayout windowLayout) {
      this.dockingWindow.SetWindowLayout(windowLayout.Layout);
      // set as current
      this.ActiveWindowLayout = windowLayout;
    }

    public override void LoadConfiguration(ServiceConfiguration configuration) {
      this.windowLayouts.Clear();
      this.DefaultWindowLayout = null;
      this.ActiveWindowLayout = null;
      var windowLayouts = new List<WindowLayout>();
      string defaultWindowLayoutName = null;
      // load previous window layout and default
      if (configuration.ReadEntry("window", out var entry)) {
        foreach (var windowLayoutEntry in entry.ReadEntries("layout")) {
          windowLayouts.Add(new Models.WindowLayout() {
            Name = windowLayoutEntry.ReadStringAttribute("name", "unnamed"),
            Layout = windowLayoutEntry.ReadStringAttribute("configuration", null),
          });
        }
        defaultWindowLayoutName = entry.ReadStringAttribute("defaultLayout", null);
      }
      // add default layouts
      foreach (var windowLayout in this.DefaultWindowLayouts) {
        if (!windowLayouts.Any(w => w.Name == windowLayout.Name)) {
          windowLayouts.Add(windowLayout);
        }
      }
      this.windowLayouts.AddRange(windowLayouts);
      var defaultWindowLayout = this.windowLayouts.FirstOrDefault(w => w.Name == defaultWindowLayoutName);
      if (defaultWindowLayout != null) {
        this.DefaultWindowLayout = defaultWindowLayout;
        this.LoadWindowLayout(defaultWindowLayout);
      } else {
        this.DefaultWindowLayout = null;
        this.ActiveWindowLayout = null;
      }
    }

    public override void SaveConfiguration(ServiceConfiguration configuration) {
      // apply current changes
      if (this.activeWindowLayout != null) {
        this.activeWindowLayout.Layout = this.dockingWindow.GetWindowLayout();
      }
      // save window layouts
      var entry = configuration.WriteEntry("window", true);
      if (this.defaultWindowLayout != null) {
        entry.WriteAttribute("defaultLayout", this.defaultWindowLayout.Name);
      }
      foreach (var windowLayout in this.WindowLayouts) {
        entry.WriteEntry("layout", false)
          .WriteAttribute("name", windowLayout.Name)
          .WriteAttribute("configuration", windowLayout.Layout);
      }
    }

    public void AddWindowLayout(string name) {
      var windowLayout = new Models.WindowLayout() {
        Name = name,
      };
      windowLayout.Layout = this.dockingWindow.GetWindowLayout();
      this.windowLayouts.Add(windowLayout);
      this.ActiveWindowLayout = windowLayout;
    }
  }
}