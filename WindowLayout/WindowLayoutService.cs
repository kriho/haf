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
    public ICompoundState CanChangeWindowLayout { get; private set; } = new CompoundState();

    private Event onActiveWindowLayoutChanged = new Event(nameof(OnActiveWindowLayoutChanged));
    public IReadOnlyEvent OnActiveWindowLayoutChanged => this.OnActiveWindowLayoutChanged;

    public IRelayCommand<IWindowLayout> DoLoadWindowLayout { get; private set; }

    public IRelayCommand<IWindowLayout> DoSaveWindowLayout { get; private set; }

    public IRelayCommand<IWindowLayout> DoDeleteWindowLayout { get; private set; }

    public IRelayCommand<IWindowLayout> DoSetDefaultWindowLayout { get; private set; }

    public IRelayCommand<IPaneMeta> DoShowPane { get; private set; }

    public IRelayCommand DoAddWindowLayout { get; private set; }

    private readonly RangeObservableCollection<IWindowLayout> windowLayouts = new RangeObservableCollection<IWindowLayout>();
    public IReadOnlyObservableCollection<IWindowLayout> WindowLayouts => this.windowLayouts;

    private readonly List<IWindowLayout> defaultindowLayouts = new List<IWindowLayout>();
    public IReadOnlyCollection<IWindowLayout> DefaultWindowLayouts => this.defaultindowLayouts;

    private readonly ObservableCollection<IPaneMeta> availablePanes = new ObservableCollection<IPaneMeta>();
    public IReadOnlyObservableCollection<IPaneMeta> AvailablePanes => this.availablePanes;

    private IWindowLayout activeWindowLayout = null;
    public IWindowLayout ActiveWindowLayout {
      get { return this.activeWindowLayout; }
      set {
        if (this.SetValue(ref this.activeWindowLayout, value)) {
          foreach (var other in this.WindowLayouts) {
            other.IsCurrent = other == value;
          }
          this.DoLoadWindowLayout.RaiseCanExecuteChanged();
          this.DoDeleteWindowLayout.RaiseCanExecuteChanged();
          this.onActiveWindowLayoutChanged.Fire();
        }
      }
    }

    private IWindowLayout defaultWindowLayout = null;
    public IWindowLayout DefaultWindowLayout {
      get { return this.defaultWindowLayout; }
      set {
        if (this.SetValue(ref this.defaultWindowLayout, value)) {
          foreach (var other in this.WindowLayouts) {
            other.IsDefault = other == value;
          }
          this.DoSetDefaultWindowLayout.RaiseCanExecuteChanged();
        }
      }
    }

    private string editName = null;
    public string EditName {
      get { return this.editName; }
      set {
        if(this.SetValue(ref this.editName, value)) {
          this.DoAddWindowLayout.RaiseCanExecuteChanged();
        }
      }
    }

    private readonly IDockingWindowService dockingWindow;

    [ImportingConstructor]
    public WindowLayoutService(IDockingWindowService dockingWindow) {
      this.dockingWindow = dockingWindow;
      this.DoLoadWindowLayout = new RelayCommand<IWindowLayout>((windowLayout) => {
        this.LoadWindowLayout(windowLayout);
      }, this.CanChangeWindowLayout);
      this.DoDeleteWindowLayout = new RelayCommand<IWindowLayout>((windowLayout) => {
        this.windowLayouts.Remove(windowLayout);
      });
      this.DoSaveWindowLayout = new RelayCommand<IWindowLayout>((windowLayout) => {
        windowLayout.Layout = this.dockingWindow.GetWindowLayout();
      });
      this.DoShowPane = new RelayCommand<IPaneMeta>((meta) => {
        this.dockingWindow.ShowPane(meta.Name, meta.Type, meta.CanUserClose);
      });
      this.DoSetDefaultWindowLayout = new RelayCommand<IWindowLayout>((windowLayout) => {
        this.DefaultWindowLayout = windowLayout;
      }, (windowLayout) => {
        return this.defaultWindowLayout != windowLayout;
      });
      this.DoAddWindowLayout = new RelayCommand(async () => {
        this.AddWindowLayout(this.editName);
        this.EditName = "";
      }, () => {
        return !string.IsNullOrWhiteSpace(this.editName) && !this.windowLayouts.Any(w => w.Name == this.editName);
      });
    }

    private void LoadWindowLayout(IWindowLayout windowLayout) {
      this.dockingWindow.SetWindowLayout(windowLayout.Layout);
      // set as current
      this.ActiveWindowLayout = windowLayout;
    }

    public void RegisterAvailablePane(string name, Type type, bool canUserClose = true) {
      this.availablePanes.Add(new PaneMeta() {
        Name = name,
        Type = type,
        CanUserClose = canUserClose,
      });
    }

    public override Task LoadConfiguration(ServiceConfiguration configuration) {
      this.windowLayouts.Clear();
      this.DefaultWindowLayout = null;
      this.ActiveWindowLayout = null;
      var windowLayouts = new List<IWindowLayout>();
      string defaultWindowLayoutName = null;
      // load previous window layout and default
      if (configuration.TryReadEntry("window", out var entry)) {
        foreach (var windowLayoutEntry in entry.ReadEntries("layout")) {
          windowLayouts.Add(new WindowLayout() {
            Name = windowLayoutEntry.ReadAttribute("name", "unnamed"),
            Layout = windowLayoutEntry.ReadAttribute("configuration", null),
          });
        }
        defaultWindowLayoutName = entry.ReadAttribute("defaultLayout", null);
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
      return Task.CompletedTask;
    }

    public override Task SaveConfiguration(ServiceConfiguration configuration) {
      // apply current changes
      if (this.activeWindowLayout != null) {
        this.activeWindowLayout.Layout = this.dockingWindow.GetWindowLayout();
      }
      // save window layouts
      var entry = configuration
        .WriteEntry("window", true)
        .WriteAttribute("defaultLayout", this.defaultWindowLayout?.Name);
      foreach (var windowLayout in this.WindowLayouts) {
        entry.WriteEntry("layout", false)
          .WriteAttribute("name", windowLayout.Name)
          .WriteAttribute("configuration", windowLayout.Layout);
      }
      return Task.CompletedTask;
    }

    public void AddWindowLayout(string name) {
      var windowLayout = new WindowLayout() {
        Name = name,
      };
      windowLayout.Layout = this.dockingWindow.GetWindowLayout();
      this.windowLayouts.Add(windowLayout);
      this.ActiveWindowLayout = windowLayout;
    }
  }
}