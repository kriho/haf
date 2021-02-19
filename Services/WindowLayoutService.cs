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

    public enum Dependencies {
      CanChangeWindowLayout
    }

    public enum Events {
      ActiveWindowLayoutChanged
    }

    public override int Id {
      get {
        return (int)ServiceId.WindowLayout;
      }
    }

    private bool topmost;
    public bool Topmost {
      get { return this.topmost; }
      set {
        this.SetValue(ref this.topmost, value);
        this.mainWindow.Topmost = value;
      }
    }

    [Import]
    private IMainWindowService mainWindow;

    public RelayCommand<Models.WindowLayout> _Load { get; private set; }

    public RelayCommand<Models.WindowLayout> _Save { get; private set; }

    public RelayCommand<Models.WindowLayout> _Delete { get; private set; }

    public RelayCommand<Models.WindowLayout> _SetDefault { get; private set; }

    public RelayCommand<PaneMeta> _ShowPane { get; private set; }

    public RadObservableCollection<Models.WindowLayout> WindowLayouts { get; private set; } = new RadObservableCollection<Models.WindowLayout>();
    public RadObservableCollection<Models.WindowLayout> DefaultWindowLayouts { get; private set; } = new RadObservableCollection<Models.WindowLayout>();

    public ObservableCollection<PaneMeta> AvailablePanes { get; private set; } = new ObservableCollection<PaneMeta>();

    private Models.WindowLayout activeWindowLayout = null;
    public Models.WindowLayout ActiveWindowLayout {
      get { return this.activeWindowLayout; }
      set {
        if (this.SetValue(ref this.activeWindowLayout, value)) {
          foreach (var windowLayout in this.WindowLayouts) {
            windowLayout.IsCurrent = windowLayout == value;
          }
          this._Load.RaiseCanExecuteChanged();
          this._Delete.RaiseCanExecuteChanged();
        }
      }
    }

    private Models.WindowLayout defaultWindowLayout = null;
    public Models.WindowLayout DefaultWindowLayout {
      get { return this.defaultWindowLayout; }
      set {
        if (this.SetValue(ref this.defaultWindowLayout, value)) {
          foreach (var project in this.WindowLayouts) {
            project.IsDefault = project == value;
          }
          this._SetDefault.RaiseCanExecuteChanged();
        }
      }
    }

    public WindowLayoutService() {
      this._Load = new RelayCommand<Models.WindowLayout>((windowLayout) => {
        this.LoadWindowLayout(windowLayout);
      }, (windowLayout) => {
        return this.GetDependency(Dependencies.CanChangeWindowLayout);
      });
      this._Delete = new RelayCommand<Models.WindowLayout>((windowLayout) => {
        this.WindowLayouts.Remove(windowLayout);
      });
      this._Save = new RelayCommand<Models.WindowLayout>((windowLayout) => {
        windowLayout.Layout = this.mainWindow.GetWindowLayout();
      });
      this._ShowPane = new RelayCommand<PaneMeta>((meta) => {
        this.mainWindow.ShowPane(meta.Name, meta.Type, meta.CanUserClose);
      });
      this._SetDefault = new RelayCommand<Models.WindowLayout>((windowLayout) => {
        this.DefaultWindowLayout = windowLayout;
      }, (windowLayout) => {
        return this.defaultWindowLayout != windowLayout;
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

    private void LoadWindowLayout(Models.WindowLayout windowLayout) {
      this.mainWindow.SetWindowLayout(windowLayout.Layout);
      // set as current
      this.ActiveWindowLayout = windowLayout;
      // fire event
      this.FireEvent(Events.ActiveWindowLayoutChanged);
    }

    public override void Load(Settings storage) {
      this.WindowLayouts.SuspendNotifications();
      this.WindowLayouts.Clear();
      this.DefaultWindowLayout = null;
      this.ActiveWindowLayout = null;
      var entry = storage.ReadEntry("window");
      if (entry != null) {
        foreach (var windowLayoutEntry in entry.ReadEntries("layout")) {
          this.WindowLayouts.Add(new Models.WindowLayout() {
            Name = windowLayoutEntry.ReadAttribute("name"),
            Layout = windowLayoutEntry.ReadAttribute("configuration"),
          });
        }
        var defaultWindowLayout = entry.ReadAttribute("defaultLayout");
        var windowLayout = this.WindowLayouts.FirstOrDefault(w => w.Name == defaultWindowLayout);
        if (windowLayout != null) {
          this.DefaultWindowLayout = windowLayout;
          this.LoadWindowLayout(windowLayout);
        } else {
          this.DefaultWindowLayout = null;
          this.ActiveWindowLayout = null;
        }
        if(int.TryParse(entry.ReadAttribute("width"), out var width)) {
          this.mainWindow.Width = width;
        }
        if (int.TryParse(entry.ReadAttribute("height"), out var height)) {
          this.mainWindow.Height = width;
        }
        if (bool.TryParse(entry.ReadAttribute("topmost"), out var topmost)) {
          this.Topmost = topmost;
        }
      }
      // add default layouts
      foreach (var windowLayout in this.DefaultWindowLayouts) {
        if (!this.WindowLayouts.Any(w => w.Name == windowLayout.Name)) {
          this.WindowLayouts.Add(windowLayout);
        }
      }
      this.WindowLayouts.ResumeNotifications();
    }

    public override void Save(Settings storage) {
      // apply current changes
      if (this.activeWindowLayout != null) {
        this.activeWindowLayout.Layout = this.mainWindow.GetWindowLayout();
      }
      // save window layouts
      var entry = storage.WriteEntry("window")
        .WriteAttribute("width", this.mainWindow.Width)
        .WriteAttribute("height", this.mainWindow.Height)
        .WriteAttribute("topmost", this.Topmost);
      if (this.defaultWindowLayout != null) {
        entry.WriteAttribute("defaultLayout", this.defaultWindowLayout.Name);
      }
      foreach (var windowLayout in this.WindowLayouts) {
        entry.WriteEntry("layout")
          .WriteAttribute("name", windowLayout.Name)
          .WriteAttribute("configuration", windowLayout.Layout);
      }
    }

    public void AddWindowLayout(string name) {
      var windowLayout = new Models.WindowLayout() {
        Name = name,
      };
      windowLayout.Layout = this.mainWindow.GetWindowLayout();
      this.WindowLayouts.Add(windowLayout);
      this.ActiveWindowLayout = windowLayout;
    }
  }
}