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

    public override void LoadConfiguration(Configuration configuration) {
      if (configuration.ReadEntry("window", out var window)) {
        this.Window.Topmost = window.ReadBooleanAttribute("topmost", false);
        if (window.ReadIntegerAttribute("width", out var width)) {
          this.Window.Width = width;
        }
        if (window.ReadIntegerAttribute("height", out var height)) {
          this.Window.Height = height;
        }
      }
    }

    public override void SaveConfiguration(Configuration configuration) {
      configuration.WriteEntry("window", true)
        .WriteAttribute("topmost", this.Window.Topmost)
        .WriteAttribute("width", this.Window.Width)
        .WriteAttribute("height", this.Window.Height);
    }
  }
}
