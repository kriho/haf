using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public interface IPlugin {
    LocalizedText Name { get; }
    LocalizedText Description { get; }
    string Icon { get; }
  }
}
