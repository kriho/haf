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

    public static readonly DependencyProperty SecondaryForegroundProperty = Behavior.SecondaryForegroundProperty.AddOwner(typeof(GeometryIcon), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Green), FrameworkPropertyMetadataOptions.Inherits));
    public Brush SecondaryForeground {
      get { return (Brush)this.GetValue(GeometryIcon.SecondaryForegroundProperty); }
      set { this.SetValue(GeometryIcon.SecondaryForegroundProperty, value); }
    }

    public static readonly DependencyProperty GeometryProperty = DependencyProperty.Register("Geometry", typeof(Geometry), typeof(GeometryIcon), new PropertyMetadata(null));
    public Geometry Geometry {
      get { return (Geometry)this.GetValue(GeometryIcon.GeometryProperty); }
      set { this.SetValue(GeometryIcon.GeometryProperty, value); }
    }

    public static readonly DependencyProperty UseSecondaryForegroundProperty = DependencyProperty.Register("UseSecondaryForeground", typeof(bool), typeof(GeometryIcon), new PropertyMetadata(false));
    public bool UseSecondaryForeground {
      get { return (bool)this.GetValue(GeometryIcon.UseSecondaryForegroundProperty); }
      set { this.SetValue(GeometryIcon.UseSecondaryForegroundProperty, value); }
    }

    public static readonly DependencyProperty GeometryWidthProperty = DependencyProperty.Register("GeometryWidth", typeof(double), typeof(GeometryIcon), new PropertyMetadata(24.0));
    public double GeometryWidth {
      get { return (double)this.GetValue(GeometryIcon.GeometryWidthProperty); }
      set { this.SetValue(GeometryIcon.GeometryWidthProperty, value); }
    }

    public static readonly DependencyProperty GeometryHeightProperty = DependencyProperty.Register("GeometryHeight", typeof(double), typeof(GeometryIcon), new PropertyMetadata(24.0));
    public double GeometryHeight {
      get { return (double)this.GetValue(GeometryIcon.GeometryHeightProperty); }
      set { this.SetValue(GeometryIcon.GeometryHeightProperty, value); }
    }
  }
}