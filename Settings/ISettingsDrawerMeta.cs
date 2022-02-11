using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HAF {
  public interface ISettingsDrawerMeta {
    /// <summary>
    /// Associated type of the drawer.
    /// </summary>
    Type AssociatedType { get; }

    /// <summary>
    /// Features supported by the drawer.
    /// </summary>
    string Features { get; }
  }
}
