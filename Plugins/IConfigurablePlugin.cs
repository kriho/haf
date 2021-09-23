using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HAF {
  public interface IConfigurablePlugin: IPlugin {
    UserControl View { get; }
    void LoadConfiguration(ServiceConfiguration configuration);
    void SaveConfiguration(ServiceConfiguration configuration);
  }
}
