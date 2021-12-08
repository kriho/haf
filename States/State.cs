using HAF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HAF {
  public class State: ObservableObject, IState {
    private bool lastValue;

#if DEBUG
    public string Link;
#endif

    private readonly List<WeakAction> callbacks = new List<WeakAction>();

    public bool Value {
      get => this.lastValue;
      set {
        if(this.SetValue(ref this.lastValue, value)) {
#if DEBUG
          Console.WriteLine($"{this.Link}={value}");
#endif
          // invoke update callbacks if value changed
          foreach(var callback in this.callbacks) {
            callback.Execute();
          }
        }
      }
    }

    private State negated;
    public IReadOnlyState Negated {
      get {
        if(this.negated == null) {
          this.negated = new State(!this.Value);
          this.RegisterUpdate(() => {
            this.negated.Value = !this.Value;
          });
        }
        return this.negated;
      }
    }

    public void RegisterUpdate(Action callback) {
      this.callbacks.Add(new WeakAction(callback));
    }

    public State(bool initialState = true) {
      this.lastValue = initialState;
    }

    public static implicit operator bool(State state) {
      return state.Value;
    }
  }
}
