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
  public class GeometryButton: Button {
    public static readonly DependencyProperty GeometryProperty = DependencyProperty.Register("Geometry", typeof(Geometry), typeof(GeometryButton), new PropertyMetadata(null));
    public Geometry Geometry {
      get { return (Geometry)this.GetValue(GeometryButton.GeometryProperty); }
      set { this.SetValue(GeometryButton.GeometryProperty, value); }
    }

    static GeometryButton() {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(GeometryButton), new FrameworkPropertyMetadata(typeof(GeometryButton)));
    }
  }
}
