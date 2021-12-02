using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HAF {
  public partial class Behaviors2 {

    public static readonly DependencyProperty DisabledVisibilityProperty = DependencyProperty.RegisterAttached("DisabledVisibility", typeof(Visibility), typeof(Behaviors2), new UIPropertyMetadata(Visibility.Visible, Behaviors2.OnDisabledVisibilityPropertyChanged)); 
    
    public static Visibility GetDisabledVisibility(DependencyObject obj) {
      return (Visibility)obj.GetValue(DisabledVisibilityProperty);
    }

    public static void SetDisabledVisibility(DependencyObject obj, Visibility value) {
      obj.SetValue(DisabledVisibilityProperty, value);
    }

    public static void OnDisabledVisibilityPropertyChanged(object sender, DependencyPropertyChangedEventArgs e) {
      if (!(sender is UIElement element)) {
        throw new ArgumentException("The dependency property can only be attached to a UI elements", "sender");
      }
      if(e.NewValue is Visibility visibility) {
        element.IsEnabledChanged += (s, e2) => {
          element.Visibility = (bool)e2.NewValue ? Visibility.Visible : visibility;
        };
      }
    }
  }
}
