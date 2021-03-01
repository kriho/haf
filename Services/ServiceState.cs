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

    /// <summary>
    /// get the current state
    /// </summary>
    /// <remarks>
    /// when resolving the value of the state, this member is optional as the state supports an implicit cast to boolean
    /// </remarks>
    public bool Value { get; private set; }

    private List<ServiceDependency> dependencies = new List<ServiceDependency>();

    /// <summary>
    /// update the value of a condition and update relevant dependencies
    /// </summary>
    public void Update(bool value) {
      if(value != this.Value) { 
        this.Value = value;
        foreach(var dependency in dependencies) {
          dependency.Update();
        }
      }
    }

    internal void RegisterDependency(ServiceDependency dependency) {
      if (!this.dependencies.Contains(dependency)) {
        this.dependencies.Add(dependency);
      }
    }

    public ServiceState(bool initialState = true) {
      this.Value = initialState;
    }

    public static implicit operator bool(ServiceState state) {
      return state.Value;
    }
  }
}
