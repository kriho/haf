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
  public interface IReadOnlyState: INotifyPropertyChanged  {
    /// <summary>
    /// Current state.
    /// </summary>
    bool Value { get; }

    /// <summary>
    /// Register callback that is invoked then the current state changed.
    /// </summary>
    void RegisterUpdate(Action callback);

    /// <summary>
    /// Get state with negated value.
    /// </summary>
    IReadOnlyState Negated { get; }
  }
}
