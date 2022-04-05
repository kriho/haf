using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HAF.Controls {
  public class FormEntry: ContentControl {
    static FormEntry() {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(FormEntry), new FrameworkPropertyMetadata(typeof(FormEntry)));
    }
  }
}
