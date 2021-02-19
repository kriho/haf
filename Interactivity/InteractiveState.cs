using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace HAF {
  public class InteractiveState : TriggerAction<FrameworkElement> {
    protected override void Invoke(object parameter) {
      if (this.Target != null && !string.IsNullOrWhiteSpace(this.StateName)) {
        VisualStateManager.GoToElementState(this.Target, this.StateName, this.UseTransitions);
      }
    }

    public static readonly DependencyProperty StateNameProperty = DependencyProperty.Register("StateName", typeof(string), typeof(InteractiveState), new PropertyMetadata(""));

    public string StateName {
      get { return (string)this.GetValue(StateNameProperty); }
      set { this.SetValue(StateNameProperty, value); }
    }

    public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(FrameworkElement), typeof(InteractiveState), new PropertyMetadata(null));

    public FrameworkElement Target {
      get { return (FrameworkElement)this.GetValue(TargetProperty); }
      set { this.SetValue(TargetProperty, value); }
    }

    public static readonly DependencyProperty UseTransitionsProperty = DependencyProperty.Register("UseTransitions", typeof(bool), typeof(InteractiveState), new PropertyMetadata(true));

    public bool UseTransitions {
      get { return (bool)this.GetValue(UseTransitionsProperty); }
      set { this.SetValue(UseTransitionsProperty, value); }
    }
  }
}
