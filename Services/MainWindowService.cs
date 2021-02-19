using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {

  public interface IMainWindow {
    Telerik.Windows.Controls.RadDocking Docking { get; }
    string Title { get; set; }
    int Width { get; set; }
    int Height { get; set; }
    bool Topmost { get; set; }
    event CancelEventHandler Closing;
  }

  [Export(typeof(IMainWindowService)), PartCreationPolicy(CreationPolicy.Shared)]
  public class MainWindowService : Service, IMainWindowService {

    public enum Events {
      ActivePaneChanged,
      WindowClosing,
    }

    public override int Id {
      get {
        return (int)ServiceId.MainWindow;
      }
    }

    public string Title {
      get {
        return this.mainWindow.Title;
      }
      set {
        this.mainWindow.Title = value;
      }
    }

    public int Width {
      get {
        return this.mainWindow.Width;
      }
      set {
        this.mainWindow.Width = value;
      }
    }

    public int Height {
      get {
        return this.mainWindow.Height;
      }
      set {
        this.mainWindow.Height = value;
      }
    }

    public bool Topmost {
      get {
        return this.mainWindow.Topmost;
      }
      set {
        this.mainWindow.Topmost = value;
      }
    }

    private IMainWindow mainWindow;

    public Telerik.Windows.Controls.RadPane ActivePane {
      get { return this.mainWindow.Docking.ActivePane; }
    }

    public void SetWindow(IMainWindow mainWindow) {
      this.mainWindow = mainWindow;
      // notify that active pane changed
      this.mainWindow.Docking.ActivePaneChanged += (sender, e) => {
        this.FireEvent(Events.ActivePaneChanged);
      };
      if (this.mainWindow.Docking.ActivePane != null) {
        this.FireEvent(Events.ActivePaneChanged);
      }
      // register closing event
      this.mainWindow.Closing += (s, e) => {
        // store backend settings
        Backend.SaveSettings();
        this.FireEvent(Events.WindowClosing);
      };
    }

    public string GetWindowLayout() {
      using (var stream = new MemoryStream()) {
        this.mainWindow.Docking.SaveLayout(stream);
        return Convert.ToBase64String(stream.GetBuffer());
      }
    }

    public void SetWindowLayout(string layout) {
      using (var stream = new MemoryStream(Convert.FromBase64String(layout))) {
        this.mainWindow.Docking.LoadLayout(stream);
      }
    }

    public void ShowPane(string name) {
      var existingPane = this.mainWindow.Docking.Panes.FirstOrDefault(p => p.Header.ToString() == name);
      if (existingPane != null) {
        existingPane.IsActive = true;
      }
    }

    public void ShowPane(string name, Type type, bool canUserClose) {
      var existingPane = this.mainWindow.Docking.Panes.FirstOrDefault(p => p.Header.ToString() == name);
      if (existingPane != null) {
        existingPane.IsHidden = false;
        existingPane.IsActive = true;
      } else {
        if (this.mainWindow.Docking.ActivePane != null) {
          var pane = new Telerik.Windows.Controls.RadPane() {
            Content = Activator.CreateInstance(type),
            Header = name,
            CanUserClose = canUserClose,
            CanUserPin = false,
          };
          Telerik.Windows.Controls.RadDocking.SetSerializationTag(pane, name);
          this.mainWindow.Docking.ActivePane.PaneGroup.Items.Add(pane);
        } else {
          throw new Exception("select a pane to add new panes to the same pane group");
        }
      }
    }
  }
}
