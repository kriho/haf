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
    public static readonly DependencyProperty ServiceProperty = DependencyProperty.RegisterAttached("Service", typeof(Type), typeof(Behavior), new UIPropertyMetadata(null, OnServicePropertyChanged));

    public static Type GetService(DependencyObject obj) {
      return (Type)obj.GetValue(ServiceProperty);
    }

    public static void SetService(DependencyObject obj, Type value) {
      obj.SetValue(ServiceProperty, value);
    }

    private static void OnServicePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
      if(obj is FrameworkElement element) {
        element.DataContext = Core.Container.GetExportedValue<object>(e.NewValue.ToString());
      }
    }
  }
}
