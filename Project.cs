using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF.Models {

  public class Project : ObservableObject {

    public string Name { get; set; }

    public string FilePath { get; set; }

    private bool isDefault = false;
    public bool IsDefault {
      get { return this.isDefault; }
      set { this.SetValue(ref this.isDefault, value); }
    }

    private bool isCurrent = false;
    public bool IsCurrent {
      get { return this.isCurrent; }
      set { this.SetValue(ref this.isCurrent, value); }
    }

  }

}