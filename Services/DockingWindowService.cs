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

  [Export(typeof(IDockingWindowService)), PartCreationPolicy(CreationPolicy.Shared)]
  public class DockingWindowService : WindowService, IDockingWindowService {

    public Telerik.Windows.Controls.RadDocking Docking { get; set; }

    public string GetWindowLayout() {
      using (var stream = new MemoryStream()) {
        this.Docking.SaveLayout(stream);
        return Convert.ToBase64String(stream.GetBuffer());
      }
    }

    public void SetWindowLayout(string layout) {
      using (var stream = new MemoryStream(Convert.FromBase64String(layout))) {
        this.Docking.LoadLayout(stream);
      }
    }

    public void ShowPane(string name) {
      var existingPane = this.Docking.Panes.FirstOrDefault(p => p.Header.ToString() == name);
      if (existingPane != null) {
        existingPane.IsActive = true;
      }
    }

    public void ShowPane(string name, Type viewType, bool canUserClose) {
      var existingPane = this.Docking.Panes.FirstOrDefault(p => p.Header.ToString() == name);
      if (existingPane != null) {
        existingPane.IsHidden = false;
        existingPane.IsActive = true;
      } else {
        if (this.Docking.ActivePane != null) {
          var pane = new Telerik.Windows.Controls.RadPane() {
            Content = Activator.CreateInstance(viewType),
            Header = name,
            CanUserClose = canUserClose,
            CanUserPin = false,
          };
          Telerik.Windows.Controls.RadDocking.SetSerializationTag(pane, name);
          this.Docking.ActivePane.PaneGroup.Items.Add(pane);
        } else {
          throw new Exception("select a pane to add new panes to the same pane group");
        }
      }
    }
  }
}
