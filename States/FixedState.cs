using HAF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HAF {
  /// <summary>
  /// Represents a fixed state.
  /// </summary>
  public class FixedState: IReadOnlyState {
    public event PropertyChangedEventHandler PropertyChanged;

    public bool Value { get; private set; }

    private FixedState negated;
    public IReadOnlyState Negated {
      get {
        if(this.negated == null) {
          this.negated = new FixedState(!this.Value);
        }
        return this.negated;
      }
    }

    public void RegisterUpdate(Action callback) {
    }

    public FixedState(bool value) {
      this.Value = value;
    }

    public static implicit operator bool(FixedState state) {
      return state.Value;
    }
  }
}
