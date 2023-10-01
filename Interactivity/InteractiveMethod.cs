using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace HAF {
  public class InteractiveMethod : TriggerAction<FrameworkElement> {
    protected override void Invoke(object parameter) {
      if (this.AssociatedObject != null) {
        var type = this.AssociatedObject.DataContext.GetType();
        var method = type.GetMethod(this.Method);
        method.Invoke(this.AssociatedObject.DataContext, new object[] { parameter });
      }
    }

    public static readonly DependencyProperty MethodProperty = DependencyProperty.Register("Method", typeof(string), typeof(InteractiveMethod), new PropertyMetadata(""));

    public string Method {
      get { return (string)this.GetValue(MethodProperty); }
      set { this.SetValue(MethodProperty, value); }
    }
  }
}
