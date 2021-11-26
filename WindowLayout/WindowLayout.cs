using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public class WindowLayout: ObservableObject, IWindowLayout {
    public string Name { get; set; }

    public string Layout { get; set; }

    private bool isDefault = false;
    public bool IsDefault {
      get => this.isDefault;
      set => this.SetValue(ref this.isDefault, value);
    }

    private bool isCurrent = false;
    public bool IsCurrent {
      get => this.isCurrent;
      set => this.SetValue(ref this.isCurrent, value);
    }

  }

}