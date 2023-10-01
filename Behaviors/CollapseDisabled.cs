using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using System.Windows.Media;

namespace HAF.Behaviors {
  public class CollapseDisabled: Behavior<ButtonBase> {
    protected override void OnAttached() {
      this.AssociatedObject.IsEnabledChanged += this.AssociatedObject_IsEnabledChanged;
      if(this.AssociatedObject.Command != null) {
        this.AssociatedObject.Command.CanExecuteChanged += this.Command_CanExecuteChanged;
      }
      this.HandleVisibility();
    }

    protected override void OnDetaching() {
      this.AssociatedObject.IsEnabledChanged -= this.AssociatedObject_IsEnabledChanged;
      if(this.AssociatedObject.Command != null) {
        this.AssociatedObject.Command.CanExecuteChanged -= this.Command_CanExecuteChanged;
      }
    }

    private void HandleVisibility() {
      this.AssociatedObject.Visibility = this.AssociatedObject.IsEnabled && (this.AssociatedObject.Command == null || this.AssociatedObject.Command.CanExecute(this.AssociatedObject.CommandParameter)) ? Visibility.Visible : Visibility.Collapsed;
    }

    private void Command_CanExecuteChanged(object sender, EventArgs e) {
      this.HandleVisibility();
    }

    private void AssociatedObject_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
      this.AssociatedObject.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
    }

  }
}
