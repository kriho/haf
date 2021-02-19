using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace HAF {
  public partial class Behaviours {

    public static readonly DependencyProperty IgnoreMouseWheelProperty = DependencyProperty.RegisterAttached("IgnoreMouseWheel", typeof(bool), typeof(Behaviours), new UIPropertyMetadata(false, Behaviours.OnIgnoreMouseWheelPropertyChanged)); 
    
    public static bool GetIgnoreMouseWheel(DependencyObject obj) {
      return (bool)obj.GetValue(IgnoreMouseWheelProperty);
    }

    public static void SetIgnoreMouseWheel(DependencyObject obj, bool value) {
      obj.SetValue(IgnoreMouseWheelProperty, value);
    }

    public static void OnIgnoreMouseWheelPropertyChanged(object sender, DependencyPropertyChangedEventArgs e) {
      if (!(sender is UIElement element)) {
        throw new ArgumentException("The dependency property can only be attached to a UI elements", "sender");
      }
      if ((bool)e.NewValue == true) {
        element.PreviewMouseWheel += HandlePreviewMouseWheel;
      } else if ((bool)e.NewValue == false) {
        element.PreviewMouseWheel -= HandlePreviewMouseWheel;
      }
    }

    private static void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs e) {
      if (sender is UIElement && !e.Handled) {
        e.Handled = true;
        var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
        eventArg.RoutedEvent = UIElement.MouseWheelEvent;
        eventArg.Source = sender;
        var parent = ((Control)sender).Parent as UIElement;
        parent.RaiseEvent(eventArg);
      }
    }
  }
}
