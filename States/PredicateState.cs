using HAF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HAF {
  public class PredicateState: ObservableObject, IPredicateState {
#if DEBUG
    public string Link;
#endif

    private readonly List<WeakAction> callbacks = new List<WeakAction>();

    private readonly List<IReadOnlyState> states = new List<IReadOnlyState>();

    private Func<bool> predicate = null;

    private bool lastValue = true;
    public bool Value {
      get => this.lastValue;
      set {
        throw new NotSupportedException("a compund state cannot be assigned a value");
      }
    }

    public void UpdateValue() {
      // calculate new value
      var value = (this.predicate == null || this.predicate()) && this.states.All(s => s.Value);
      if(value != this.lastValue) {
        this.lastValue = value;
#if DEBUG
        Console.WriteLine($"{this.Link}={this.Value}");
#endif
        this.NotifyPropertyChanged(() => this.Value);
        // invoke update callbacks if value changed
        foreach(var callback in this.callbacks) {
          callback.Execute();
        }
      }
    }

    public void RegisterUpdate(Action callback) {
      this.callbacks.Add(new WeakAction(callback));
    }

    public void AddStates(params IReadOnlyState[] states) {
      foreach(var state in states) {
        if(state != null && !this.states.Contains(state)) {
          this.states.Add(state);
          state.RegisterUpdate(this.UpdateValue);
        }
      }
      this.UpdateValue();
    }

    public void SetPredicate(Func<bool> predicate) {
      this.predicate = predicate;
    }

    public PredicateState() {
    }

    public PredicateState(Func<bool> predicate) {
      this.SetPredicate(predicate);
    }

    public PredicateState(Func<bool> predicate, params IReadOnlyState[] states) {
      this.SetPredicate(predicate);
      this.AddStates(states);
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

    public static implicit operator bool(PredicateState state) {
      return state.Value;
    }
  }
}
