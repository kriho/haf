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
    public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.RegisterAttached("HighlightBrush", typeof(Brush), typeof(Behavior), new UIPropertyMetadata(null));

    public static Brush GetHighlightBrush(DependencyObject obj) {
      return (Brush)obj.GetValue(HighlightBrushProperty);
    }

    public static void SetHighlightBrush(DependencyObject obj, Brush value) {
      obj.SetValue(HighlightBrushProperty, value);
    }
  }
}
