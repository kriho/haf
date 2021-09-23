using HAF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HAF {

  public abstract class Service: LinkedObject, IService {

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
