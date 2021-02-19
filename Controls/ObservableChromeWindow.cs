using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HAF.Controls {
  public class ObservableChromeWindow : Window {

    public object Header {
      get { return (object)GetValue(HeaderProperty); }
      set { SetValue(HeaderProperty, value); }
    }

    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(ObservableChromeWindow ), new UIPropertyMetadata(null));

    static ObservableChromeWindow () {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ObservableChromeWindow ), new FrameworkPropertyMetadata(typeof(ObservableChromeWindow)));
    }

    public override void OnApplyTemplate() {
      var closeButton = this.GetTemplateChild("PART_Close") as Button;
      closeButton.Click += (s, e) => {
        this.Close();
      }; 
      var minimizeButton = this.GetTemplateChild("PART_Minimize") as Button;
      minimizeButton.Click += (s, e) => {
        this.WindowState = WindowState.Minimized;
      };
      var restoreButton = this.GetTemplateChild("PART_Restore") as Button;
      restoreButton.Click += (s, e) => {
        this.WindowState = WindowState.Normal;
      };
      var maximizeButton = this.GetTemplateChild("PART_Maximize") as Button;
      maximizeButton.Click += (s, e) => {
        this.WindowState = WindowState.Maximized;
      };
      base.OnApplyTemplate();
    }
  }
}
