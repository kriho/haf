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

  [Export(typeof(IWindowService)), PartCreationPolicy(CreationPolicy.Shared)]
  public class WindowService : Service, IWindowService {
    private Dictionary<string, object> controls = new Dictionary<string,object>();

    public Window Window { get; set; }

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
      HAF.Configuration.StageAction(ConfigurationStage.WindowInitialization, () => {
        if(configuration.TryReadEntry("window", out var window)) {
          this.Window.Topmost = window.ReadAttribute("topmost", false);
          if(window.TryReadAttribute("width", out int width)) {
            this.Window.Width = width;
          }
          if(window.TryReadAttribute("height", out int height)) {
            this.Window.Height = height;
          }
        }
      });
      return Task.CompletedTask;
    }

    public override Task SaveConfiguration(ServiceConfiguration configuration) {
      configuration.WriteEntry("window", true)
        .WriteAttribute("topmost", this.Window.Topmost)
        .WriteAttribute("width", (int)this.Window.Width)
        .WriteAttribute("height", (int)this.Window.Height); 
      return Task.CompletedTask;
    }
  }
}
