using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HAF {
  public partial class Behavior {
    public static readonly DependencyProperty SupportUrlProperty = DependencyProperty.RegisterAttached("SupportUrl", typeof(string), typeof(Behavior), new UIPropertyMetadata(null));

    public static string GetSupportUrl(DependencyObject obj) {
      return (string)obj.GetValue(SupportUrlProperty);
    }

    public static void SetSupportUrl(DependencyObject obj, string value) {
      obj.SetValue(SupportUrlProperty, value);
    }
  }
}
