using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HAF {
  public partial class Behavior {
    private static IInspectorService inspectorService = null;

    public static readonly DependencyProperty InspectOnLeftMouseDownProperty = DependencyProperty.RegisterAttached("InspectOnLeftMouseDown", typeof(bool), typeof(Behavior), new UIPropertyMetadata(false, Behavior.OnInspectOnLeftMouseDownPropertyChanged)); 
    
    public static bool GetInspectOnLeftMouseDown(DependencyObject obj) {
      return (bool)obj.GetValue(InspectOnLeftMouseDownProperty);
    }

    public static void SetInspectOnLeftMouseDown(DependencyObject obj, bool value) {
      obj.SetValue(InspectOnLeftMouseDownProperty, value);
    }

    public static void OnInspectOnLeftMouseDownPropertyChanged(object sender, DependencyPropertyChangedEventArgs e) {
      if (!(sender is FrameworkElement element)) {
        throw new ArgumentException("The dependency property can only be attached to a FrameworkElement.", "sender");
      }
      if ((bool)e.NewValue == true) {
        element.MouseLeftButtonDown += Element_MouseLeftButtonDown;
      } else if ((bool)e.NewValue == false) {
        element.MouseLeftButtonDown -= Element_MouseLeftButtonDown;
      }
    }

    private static void Element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
      if(sender is FrameworkElement element && !e.Handled) {
        if(Behavior.inspectorService == null) {
          Behavior.inspectorService = Core.Container.GetExportedValue<IInspectorService>();
        }
        Behavior.inspectorService.SelectedItem = element.DataContext;
      }
    }
  }
}
