using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HAF.Splash {
  public class SplashWindow : System.Windows.Window {
    public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(string), typeof(SplashWindow), new PropertyMetadata(""));

    public string Status {
      get { return (string)GetValue(StatusProperty); }
      set { SetValue(StatusProperty, value); }
    }

    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(SplashWindow), new PropertyMetadata(""));

    public string Description {
      get { return (string)GetValue(DescriptionProperty); }
      set { SetValue(DescriptionProperty, value); }
    }

    public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register("Progress", typeof(int), typeof(SplashWindow), new PropertyMetadata(0));

    public int Progress {
      get { return (int)GetValue(ProgressProperty); }
      set { SetValue(ProgressProperty, value); }
    }

    public SplashWindow() {
      this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
    }
  }
}
