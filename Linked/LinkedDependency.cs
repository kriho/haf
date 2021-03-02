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

  /// <summary>
  /// declares a dependency of a linked object
  /// </summary>
  /// <remarks>
  /// dependencies evaluate link states to control what a linked object may do
  /// dependency names must always start with May...
  /// </remarks>
  public class LinkedDependency: ObservableObject {

#if DEBUG
    public string Link;
#endif

    private readonly List<WeakAction> callbacks = new List<WeakAction>();
    private readonly List<LinkedState> states = new List<LinkedState>();

    /// <summary>
    /// get the current state of the dependency
    /// </summary>
    /// <remarks>
    /// when resolving the value of the dependency, this member is optional as the dependency supports an implicit cast to boolean
    /// </remarks>
    public bool Value { get; private set; } = true;

    public void Update() {
      // calculate new value
      var last = this.Value;
      this.Value = this.states.All(s => s);
      if(this.Value != last) {
#if DEBUG
        Console.WriteLine($"{this.Link}={this.Value}");
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
    public void AddConditions(params LinkedState[] states) {
      foreach(var state in states) {
        if (!this.states.Contains(state)) {
          this.states.Add(state);
          state.RegisterDependency(this);
        }
      }
      this.Update();
    }

    public LinkedDependency(Action callback = null) {
      if (callback != null) {
        this.RegisterUpdate(callback);
      }
    }

    public static implicit operator bool(LinkedDependency dependency) {
      return dependency.Value;
    }
  }
}
