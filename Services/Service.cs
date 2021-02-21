using HAF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Telerik.Windows.Data;

namespace HAF {

  public abstract class Service : ObservableObject, IService {

    public virtual void LoadConfiguration(Configuration configuration) {
    }

    public virtual void SaveConfiguration(Configuration configuration) {
    }

    public virtual void ClearConfiguration() {
      // use defaults from LoadConfiguration()
      this.LoadConfiguration(new Configuration("empty"));
    }
  }
}
