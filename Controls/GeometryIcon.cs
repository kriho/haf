using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace HAF.Controls {
  public class GeometryIcon: Control {
    public static readonly new DependencyProperty ForegroundProperty = TextElement.ForegroundProperty.AddOwner(typeof(GeometryIcon), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Green), FrameworkPropertyMetadataOptions.Inherits));
    public new Brush Foreground {
      get { return (Brush)this.GetValue(GeometryIcon.ForegroundProperty); }
      set { this.SetValue(GeometryIcon.ForegroundProperty, value); }
    }

    public static readonly DependencyProperty GeometryProperty = DependencyProperty.Register("Geometry", typeof(Geometry), typeof(GeometryIcon), new PropertyMetadata(null));
    public Geometry Geometry {
      get { return (Geometry)this.GetValue(GeometryIcon.GeometryProperty); }
      set { this.SetValue(GeometryIcon.GeometryProperty, value); }
    }
  }
}
