using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace HAF {
  public class InteractiveCommand : TriggerAction<DependencyObject> {
    protected override void Invoke(object parameter) {
      if (base.AssociatedObject != null) {
        if (this.Command != null && this.Command.CanExecute(parameter)) {
          this.Command.Execute(parameter);
        }
      }
    }

    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(InteractiveCommand), new PropertyMetadata(null));

    public ICommand Command {
      get { return (ICommand)this.GetValue(CommandProperty); }
      set { this.SetValue(CommandProperty, value); }
    }
  }
}
