using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public class PaneMeta: IPaneMeta {
    public string Name { get; set; }
    public Type Type { get; set; }
    public bool CanUserClose { get; set; } = true;
  }
}