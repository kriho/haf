using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HAF {

  [Export(typeof(IWindowService)), PartCreationPolicy(CreationPolicy.Shared)]
  public class WindowService: Service, IWindowService {
    private Dictionary<string, object> controls = new Dictionary<string, object>();

    public Window Window { get; set; }

    private Size? lastSize = null;

    public bool TryGetControl<T>(string name, out T control) {
      if(this.controls.TryGetValue(name, out var entry) && entry is T typedEntry) {
        control = typedEntry;
        return true;
      }
      control = default(T);
      return false;
    }


    public T GetControl<T>(string name) {
      if(this.TryGetControl<T>(name, out var control)) {
        return control;
      } else {
        throw new Exception($"the control \"{name}\" was accessed before its registration in the window service");
      }
    }

    public void RegisterControl<T>(string name, T control) {
      if(this.controls.ContainsKey(name)) {
        throw new InvalidOperationException($"the control with name \"{name}\" was already registered");
      }
      this.controls.Add(name, control);
    }

    public override Task LoadConfiguration(ServiceConfiguration configuration) {
      if(configuration.TryReadEntry("window", out var window)) {
        HAF.Configuration.StageAction(ConfigurationStage.WindowInitialization, () => {
          /*this.Window.SizeChanged += (s, e) => {
            if(this.Window.WindowState != System.Windows.WindowState.Maximized) {
              this.lastSize = new Size(this.Window.ActualWidth, this.Window.ActualHeight);
            }
          };*/
          this.Window.Topmost = window.ReadAttribute("topmost", false);
          if(window.TryReadAttribute<int>("top", out var top)) {
            this.Window.Top = top;
          }
          if(window.TryReadAttribute<int>("left", out var left)) {
            this.Window.Left = left;
          }
          if(window.TryReadAttribute("width", out int width) && window.TryReadAttribute("height", out int height)) {
            this.Window.Width = width;
            this.Window.Height = height;
            this.lastSize = new Size(width, height);
          }
        });
        HAF.Configuration.StageAction(ConfigurationStage.Running, () => {
          this.Window.WindowState = window.ReadAttribute("maximized", false) ? WindowState.Maximized : WindowState.Normal;
        });
      }
      return Task.CompletedTask;
    }

    public override Task SaveConfiguration(ServiceConfiguration configuration) {
      var entry = configuration.WriteEntry("window", true)
        .WriteAttribute("topmost", this.Window.Topmost)
        .WriteAttribute("top", (int)this.Window.Top)
        .WriteAttribute("left", (int)this.Window.Left);
      var maximized = this.Window.WindowState == System.Windows.WindowState.Maximized;
      entry.WriteAttribute("maximized", maximized);
      if(!maximized) {
        entry.WriteAttribute("width", (int)this.Window.Width)
          .WriteAttribute("height", (int)this.Window.Height);
      } else if(this.lastSize != null) {
        entry.WriteAttribute("width", (int)this.lastSize.Value.Width)
          .WriteAttribute("height", (int)this.lastSize.Value.Height);
      }
      return Task.CompletedTask;
    }
  }
}
