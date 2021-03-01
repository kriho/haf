using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HAF.Controls {
  public class FormGroup : ContentControl {

    public static readonly DependencyProperty BorderlessProperty = DependencyProperty.Register("Borderless", typeof(bool), typeof(FormGroup), new PropertyMetadata(true));

    public bool Borderless {
      get { return (bool)GetValue(BorderlessProperty); }
      set { SetValue(BorderlessProperty, value); }
    }

    static FormGroup() {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(FormGroup), new FrameworkPropertyMetadata(typeof(FormGroup)));
    }
  }
}
