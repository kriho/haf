using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HAF.Controls {
  public class FormTitle: Control {

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(FormTitle), new PropertyMetadata("Title"));

    public string Title {
      get { return (string)GetValue(TitleProperty); }
      set { SetValue(TitleProperty, value); }
    }

    static FormTitle() {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(FormTitle), new FrameworkPropertyMetadata(typeof(FormTitle)));

    }
  }
}
