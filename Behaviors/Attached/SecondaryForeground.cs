using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HAF {
  public partial class Behavior {
    public static readonly DependencyProperty SecondaryForegroundProperty = DependencyProperty.RegisterAttached("SecondaryForeground", typeof(Brush), typeof(Behavior), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

    public static Brush GetSecondaryForeground(DependencyObject obj) {
      return (Brush)obj.GetValue(SecondaryForegroundProperty);
    }

    public static void SetSecondaryForeground(DependencyObject obj, Brush value) {
      obj.SetValue(SecondaryForegroundProperty, value);
    }
  }
}
