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
    /// <summary>
    /// Name of the pane.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Type of view hosted by the pane.
    /// </summary>
    Type Type { get; }

    /// <summary>
    /// Can the user close the pane.
    /// </summary>
    bool CanUserClose { get; }
  }
}