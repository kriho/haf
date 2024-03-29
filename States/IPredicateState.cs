﻿using HAF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HAF {
  public interface IPredicateState: IReadOnlyState {
    /// <summary>
    /// Update current value.
    /// </summary>
    void UpdateValue();
  }
}
