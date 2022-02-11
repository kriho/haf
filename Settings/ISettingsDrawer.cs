using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HAF {
  public interface ISettingsDrawer {
    /// <summary>
    /// <c>DataContext</c> of the drawer.
    /// </summary>
    object DataContext { set; }

    /// <summary>
    /// Display name of the drawer. Used for filtering search results.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Description of the drawer. Used for filtering search results.
    /// </summary>
    string Description { get; }
  }
}
