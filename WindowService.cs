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

    public Window Window { get; set; }

    public override void LoadConfiguration(ServiceConfiguration configuration) {
      if (configuration.TryReadEntry("window", out var window)) {
        this.Window.Topmost = window.ReadAttribute("topmost", false);
        if (window.TryReadAttribute("width", out int width)) {
          this.Window.Width = width;
        }
        if (window.TryReadAttribute("height", out int height)) {
          this.Window.Height = height;
        }
      }
    }

    public override void SaveConfiguration(ServiceConfiguration configuration) {
      configuration.WriteEntry("window", true)
        .WriteAttribute("topmost", this.Window.Topmost)
        .WriteAttribute("width", (int)this.Window.Width)
        .WriteAttribute("height", (int)this.Window.Height);
    }
  }
}
