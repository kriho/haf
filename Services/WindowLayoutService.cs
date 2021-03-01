using HAF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Data;

namespace HAF {

  [Export(typeof(IWindowLayoutService)), PartCreationPolicy(CreationPolicy.Shared)]
  public class WindowLayoutService : Service, IWindowLayoutService {

    public ServiceDependency CanChangeWindowLayout { get; private set; } = new ServiceDependency("can cahnge window layout");

    public ServiceEvent OnActiveWindowLayoutChanged { get; private set; } = new ServiceEvent("on active window layout changed");

    [Import]
#pragma warning disable CS0649 // imported by MEF
    private IDockingWindowService dockingWindow;
#pragma warning restore CS0649

    public RelayCommand<WindowLayout> LoadCommand { get; private set; }

    public RelayCommand<WindowLayout> SaveCommand { get; private set; }

    public RelayCommand<WindowLayout> DeleteCommand { get; private set; }

    public RelayCommand<WindowLayout> SetDefaultCommand { get; private set; }

    public RelayCommand<PaneMeta> ShowPaneCommand { get; private set; }

    public RangeObservableCollection<WindowLayout> WindowLayouts { get; private set; } = new RangeObservableCollection<WindowLayout>();

    public RangeObservableCollection<WindowLayout> DefaultWindowLayouts { get; private set; } = new RangeObservableCollection<WindowLayout>();

    public ObservableCollection<PaneMeta> AvailablePanes { get; private set; } = new ObservableCollection<PaneMeta>();

    private WindowLayout activeWindowLayout = null;
    public WindowLayout ActiveWindowLayout {
      get { return this.activeWindowLayout; }
      set {
        if (this.SetValue(ref this.activeWindowLayout, value)) {
          foreach (var windowLayout in this.WindowLayouts) {
            windowLayout.IsCurrent = windowLayout == value;
          }
          this.LoadCommand.RaiseCanExecuteChanged();
          this.DeleteCommand.RaiseCanExecuteChanged();
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
          this.SetDefaultCommand.RaiseCanExecuteChanged();
        }
      }
    }

    public WindowLayoutService() {
      this.LoadCommand = new RelayCommand<Models.WindowLayout>((windowLayout) => {
        this.LoadWindowLayout(windowLayout);
      }, (windowLayout) => {
        return this.CanChangeWindowLayout;
      });
      this.DeleteCommand = new RelayCommand<Models.WindowLayout>((windowLayout) => {
        this.WindowLayouts.Remove(windowLayout);
      });
      this.SaveCommand = new RelayCommand<Models.WindowLayout>((windowLayout) => {
        windowLayout.Layout = this.dockingWindow.GetWindowLayout();
      });
      this.ShowPaneCommand = new RelayCommand<PaneMeta>((meta) => {
        this.dockingWindow.ShowPane(meta.Name, meta.Type, meta.CanUserClose);
      });
      this.SetDefaultCommand = new RelayCommand<Models.WindowLayout>((windowLayout) => {
        this.DefaultWindowLayout = windowLayout;
      }, (windowLayout) => {
        return this.defaultWindowLayout != windowLayout;
      });
      this.CanChangeWindowLayout.RegisterUpdate(() => {
        this.LoadCommand.RaiseCanExecuteChanged();
      });
#if DEBUG
      if (this.IsInDesignMode) {
        this.WindowLayouts.Add(new Models.WindowLayout() {
          Name = "default config",
          IsDefault = true,
        });
        this.WindowLayouts.Add(new Models.WindowLayout() {
          Name = "sniffer",
          IsCurrent = true,
        });
      }
#endif
    }

    private void LoadWindowLayout(WindowLayout windowLayout) {
      this.dockingWindow.SetWindowLayout(windowLayout.Layout);
      // set as current
      this.ActiveWindowLayout = windowLayout;
    }

    public override void LoadConfiguration(ServiceConfiguration configuration) {
      this.WindowLayouts.Clear();
      var windowLayouts = new List<WindowLayout>();
      this.DefaultWindowLayout = null;
      this.ActiveWindowLayout = null;
      if (configuration.ReadEntry("window", out var entry)) {
        foreach (var windowLayoutEntry in entry.ReadEntries("layout")) {
          windowLayouts.Add(new Models.WindowLayout() {
            Name = windowLayoutEntry.ReadStringAttribute("name", "unnamed"),
            Layout = windowLayoutEntry.ReadStringAttribute("configuration", null),
          });
        }
        // add default layouts
        foreach (var windowLayout in this.DefaultWindowLayouts) {
          if (!windowLayouts.Any(w => w.Name == windowLayout.Name)) {
            windowLayouts.Add(windowLayout);
          }
        }
        this.WindowLayouts.AddRange(windowLayouts);
        var defaultWindowLayoutName = entry.ReadStringAttribute("defaultLayout", null);
        var defaultWindowLayout = this.WindowLayouts.FirstOrDefault(w => w.Name == defaultWindowLayoutName);
        if (defaultWindowLayout != null) {
          this.DefaultWindowLayout = defaultWindowLayout;
          this.LoadWindowLayout(defaultWindowLayout);
        } else {
          this.DefaultWindowLayout = null;
          this.ActiveWindowLayout = null;
        }
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
      this.WindowLayouts.Add(windowLayout);
      this.ActiveWindowLayout = windowLayout;
    }
  }
}