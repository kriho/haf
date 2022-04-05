using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HAF.Controls {
  public class ScrollingForm: Form {
    static ScrollingForm() {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ScrollingForm), new FrameworkPropertyMetadata(typeof(ScrollingForm)));
    }
  }
}
