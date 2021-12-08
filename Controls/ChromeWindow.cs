using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HAF.Controls {
  public class ChromeWindow: Window {
    
    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(ChromeWindow), new UIPropertyMetadata(null));
    public object Header {
      get { return (object)GetValue(HeaderProperty); }
      set { SetValue(HeaderProperty, value); }
    }

    public static readonly DependencyProperty TitleIconProperty = DependencyProperty.Register("TitleIcon", typeof(ImageSource), typeof(ChromeWindow), new UIPropertyMetadata(null));
    public ImageSource TitleIcon {
      get { return (ImageSource)GetValue(TitleIconProperty); }
      set { SetValue(TitleIconProperty, value); }
    }

    public static readonly DependencyProperty CanMinimizeProperty = DependencyProperty.Register("CanMinimize", typeof(bool), typeof(ChromeWindow), new PropertyMetadata(true));
    public bool CanMinimize {
      get { return (bool)GetValue(CanMinimizeProperty); }
      set { SetValue(CanMinimizeProperty, value); }
    }

    public static readonly DependencyProperty CanMaximiseProperty = DependencyProperty.Register("CanMaximise", typeof(bool), typeof(ChromeWindow), new PropertyMetadata(true));
    public bool CanMaximise {
      get { return (bool)GetValue(CanMaximiseProperty); }
      set { SetValue(CanMaximiseProperty, value); }
    }

    static ChromeWindow() {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ChromeWindow), new FrameworkPropertyMetadata(typeof(ChromeWindow)));
    }

    public override void OnApplyTemplate() {
      var closeButton = this.GetTemplateChild("PART_Close") as Button;
      var minimizeButton = this.GetTemplateChild("PART_Minimize") as Button;
      var restoreButton = this.GetTemplateChild("PART_Restore") as Button;
      var maximizeButton = this.GetTemplateChild("PART_Maximize") as Button;
      // initial visibility
      if(this.WindowState == WindowState.Maximized) {
        restoreButton.Visibility = this.CanMaximise ? Visibility.Visible : Visibility.Collapsed;
        maximizeButton.Visibility = Visibility.Collapsed;
      } else {
        restoreButton.Visibility = Visibility.Collapsed;
        maximizeButton.Visibility = this.CanMaximise ? Visibility.Visible : Visibility.Collapsed;
      }
      minimizeButton.Visibility = this.CanMinimize ? Visibility.Visible : Visibility.Collapsed;
      // click handlers
      closeButton.Click += (s, e) => {
        this.Close();
      };
      minimizeButton.Click += (s, e) => {
        this.WindowState = WindowState.Minimized;
      };
      restoreButton.Click += (s, e) => {
        this.WindowState = WindowState.Normal;
        restoreButton.Visibility = Visibility.Collapsed;
        maximizeButton.Visibility = Visibility.Visible;
      };
      maximizeButton.Click += (s, e) => {
        this.WindowState = WindowState.Maximized;
        restoreButton.Visibility = Visibility.Visible;
        maximizeButton.Visibility = Visibility.Collapsed;
      };
      base.OnApplyTemplate();
    }
  }
}
