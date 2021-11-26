using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HAF {
  public interface ISettingsDrawerMeta {
    Type AssociatedType { get; }
    string Features { get; }
  }
}
