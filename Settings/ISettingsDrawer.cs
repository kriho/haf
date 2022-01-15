using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HAF {
  public interface ISettingsDrawer {
    object DataContext { set; }
    string DisplayName { get; }
    string Description { get; }
  }
}
