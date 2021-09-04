using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HAF.Controls {
  public class HeaderedFormGroup: HeaderedContentControl {

    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(HeaderedFormGroup), new PropertyMetadata(Orientation.Vertical));

    public Orientation Orientation {
      get { return (Orientation)GetValue(OrientationProperty); }
      set { SetValue(OrientationProperty, value); }
    }

    static HeaderedFormGroup() {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(HeaderedFormGroup), new FrameworkPropertyMetadata(typeof(HeaderedFormGroup)));
    }
  }
}
