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

  public class ServiceState {

    public bool Value { get; private set; } = true;

    private List<ServiceDependency> dependencies = new List<ServiceDependency>();

    /// <summary>
    /// update the value of a condition and update relevant dependencies
    /// </summary>
    public void Update(bool value) {
      this.Value = value;
      foreach (var dependency in dependencies) {
        dependency.Update();
      }
    }

    public void AddDependency(ServiceDependency dependency) {
      if (!this.dependencies.Contains(dependency)) {
        this.dependencies.Add(dependency);
      }
    }
  }
}
