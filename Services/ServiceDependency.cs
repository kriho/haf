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

  public class ServiceDependency {

    private List<WeakAction> callbacks = new List<WeakAction>();
    private List<ServiceState> states = new List<ServiceState>();

    /// <summary>
    /// get the current state of the dependency
    /// </summary>
    /// <remarks>
    /// when resolving the value of the dependency, this member is optional as the dependency supports an implicit cast to boolean
    /// </remarks>
    public bool Value {
      get {
        return this.states.All(s => s.Value);
      }
    }

    public void Update() {
      foreach (var callback in this.callbacks) {
        if (callback.IsAlive) {
          callback.Execute();
        }
      }
    }

    public void RegisterUpdate(Action callback) {
      this.callbacks.Add(new WeakAction(callback));
    }

    public void AddState(ServiceState state) {
      if (!this.states.Contains(state)) {
        this.states.Add(state);
      }
    }

    public static implicit operator bool(ServiceDependency dependency) {
      return dependency.Value;
    }
  }
}
