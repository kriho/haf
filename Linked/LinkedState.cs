using HAF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HAF {

  /// <summary>
  /// declares a state of a linked object
  /// </summary>
  /// <remarks>
  /// states can be used as conditions for link dependencies
  /// state names must always start with Can... or Is...
  /// </remarks>
  public class LinkedState: ObservableObject {

    private bool lastValue;

#if DEBUG
    public string Name;
#endif

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
#if DEBUG
          Console.WriteLine($"{this.Name}={value}");
# endif
          foreach (var dependency in dependencies) {
            dependency.Update();
          }
        }
      }
    }

    private List<LinkedDependency> dependencies = new List<LinkedDependency>();

    internal void RegisterDependency(LinkedDependency dependency) {
      if (!this.dependencies.Contains(dependency)) {
        this.dependencies.Add(dependency);
      }
    }

    public LinkedState(bool initialState = true) {
      this.lastValue = initialState;
    }

    public static implicit operator bool(LinkedState state) {
      return state.Value;
    }
  }
}
