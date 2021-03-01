using HAF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Telerik.Windows.Data;

namespace HAF {

  public class ServiceDependency: ObservableObject {

#if DEBUG
    public string Name;
#endif

    private readonly List<WeakAction> callbacks = new List<WeakAction>();
    private readonly List<ServiceState> states = new List<ServiceState>();

    /// <summary>
    /// get the current state of the dependency
    /// </summary>
    /// <remarks>
    /// when resolving the value of the dependency, this member is optional as the dependency supports an implicit cast to boolean
    /// </remarks>
    public bool Value { get; private set; } = true;

    public void Update([CallerMemberName]string callerName = null) {
      // calculate new value
      var last = this.Value;
      this.Value = this.states.All(s => s);
      if(this.Value != last) {
#if DEBUG
        Console.WriteLine($"[{this.Name}]={this.Value}");
#endif
        // invoke update callbacks if value changed
        foreach (var callback in this.callbacks) {
          if(callback.IsAlive) {
            callback.Execute();
          }
        }
        this.NotifyPropertyChanged(() => this.Value);
      }
    }

    /// <summary>
    /// register update callback that is fired once the dependency value changes
    /// </summary>
    /// <param name="callback"></param>
    public void RegisterUpdate(Action callback) {
      this.callbacks.Add(new WeakAction(callback));
    }

    /// <summary>
    /// add states as conditions
    /// </summary>
    /// <remarks>
    /// if one conditional state evaluates to false, the dependency is false
    /// </remarks>
    public void AddConditions(params ServiceState[] states) {
      foreach(var state in states) {
        if (!this.states.Contains(state)) {
          this.states.Add(state);
          state.RegisterDependency(this);
        }
      }
      this.Update();
    }

    public ServiceDependency(string name, Action callback = null) {
#if DEBUG
      this.Name = name;
#endif
      if (callback != null) {
        this.RegisterUpdate(callback);
      }
    }

    public static implicit operator bool(ServiceDependency dependency) {
      return dependency.Value;
    }
  }
}
