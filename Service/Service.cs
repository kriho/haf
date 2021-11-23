using HAF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HAF {

  public abstract class Service: LinkedObservableObject, IService {
    public virtual Task LoadConfiguration(ServiceConfiguration configuration) {
      return Task.CompletedTask;
    }

    public virtual Task SaveConfiguration(ServiceConfiguration configuration) {
      return Task.CompletedTask;
    }

    public virtual Task Reset() {
      return Task.CompletedTask;
    }

    public void ClearConfiguration() {
      // load empty configuration
      this.LoadConfiguration(new ServiceConfiguration("default"));
    }
  }
}
