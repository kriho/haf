using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HAF {
  public abstract class ConfigurablePlugin: Plugin, IConfigurablePlugin {
    public UserControl View { get; private set; }

    public ConfigurablePlugin(UserControl view) {
      if(view != null) {
        this.View = view;
        this.View.DataContext = this;
      }
    }

    public virtual void LoadConfiguration(ServiceConfiguration configuration) {
    }

    public virtual void SaveConfiguration(ServiceConfiguration configuration) {
    }

    public void ClearConfiguration() {
      // load empty configuration
      this.LoadConfiguration(new ServiceConfiguration("default"));
    }
  }
}
