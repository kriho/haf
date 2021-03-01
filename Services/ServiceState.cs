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

  public class ServiceState : ObservableObject {

    private bool lastValue;

    /// <summary>
    /// the current state
    /// </summary>
    /// <remarks>
    /// when resolving the value of the state, this member is optional as the state supports an implicit cast to boolean
    /// </remarks>
    public bool Value {
      get => this.lastValue;
      set {
        if (this.SetValue(ref this.lastValue, value)) {
          foreach (var dependency in dependencies) {
            dependency.Update();
          }
        }
      }
    }

    private List<ServiceDependency> dependencies = new List<ServiceDependency>();

    internal void RegisterDependency(ServiceDependency dependency) {
      if (!this.dependencies.Contains(dependency)) {
        this.dependencies.Add(dependency);
      }
    }

    public ServiceState(bool initialState = true) {
      this.lastValue = initialState;
    }

    public static implicit operator bool(ServiceState state) {
      return state.Value;
    }
  }
}
