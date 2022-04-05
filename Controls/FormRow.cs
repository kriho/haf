using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HAF.Controls {
  public class FormRow: HeaderedContentControl {
    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(FormRow), new PropertyMetadata(null));
    public string Description {
      get { return (string)this.GetValue(FormRow.DescriptionProperty); }
      set { this.SetValue(FormRow.DescriptionProperty, value); }
    }

    static FormRow() {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(FormRow), new FrameworkPropertyMetadata(typeof(FormRow)));
    }
  }
}
