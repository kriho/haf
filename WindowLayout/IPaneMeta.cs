using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public interface IPaneMeta {
    string Name { get; }
    Type Type { get; }
    bool CanUserClose { get; }
  }
}