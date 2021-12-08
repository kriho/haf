using HAF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HAF {
  public class CompoundOrState: CompoundState {
    protected override bool Evaluate() {
      return this.states.Any(s => s.Value);
    }

    public CompoundOrState() {
    }

    public CompoundOrState(params IReadOnlyState[] states): base(states) {
    }
  }
}
