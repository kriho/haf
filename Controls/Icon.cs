using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HAF.Controls {
  public class Icon : Image {

    public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register("Foreground", typeof(Color), typeof(Icon), new PropertyMetadata(Colors.Black, new PropertyChangedCallback(OnForegroundChanged)));

    public Color Foreground {
      get { return (Color)GetValue(ForegroundProperty); }
      set { SetValue(ForegroundProperty, value); }
    }

    static Icon() {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(Icon), new FrameworkPropertyMetadata(typeof(Icon)));
    }

    static void OnForegroundChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
      if (obj is Icon icon) {
        if (args.NewValue is Color color) {
          icon.ApplyColor(color);
        }
      }
    }

    private DrawingImage image = null;
    private List<GeometryDrawing> drawings = null;

    private List<GeometryDrawing> GetDrawings() {
      if (this.image == null) {
        if (this.Source is DrawingImage drawingImage) {
          this.image = drawingImage;
        } else {
          return new List<GeometryDrawing>();
        }
      }
      if (this.drawings == null) {
        // find all drawings of drawing image
        this.drawings = Icon.GetDrawings(this.image);
      }
      return drawings;
    }

    public void ApplyColor(Color color) {
      var drawings = this.GetDrawings();
      foreach (var drawing in drawings) {
        this.ApplyColor(drawing, color);
      }
    }

    private void ApplyColor(GeometryDrawing drawing, Color color) {
      if (drawing.Brush != null) {
        drawing.Brush = new SolidColorBrush(color);
      }
      if (drawing.Pen != null) {
        drawing.Pen.Brush = new SolidColorBrush(color);
      }
    }

    public static IEnumerable<T> FindLogicalChildren<T>(DependencyObject dependencyObject) where T : DependencyObject {
      if (dependencyObject != null) {
        foreach (var rawChild in LogicalTreeHelper.GetChildren(dependencyObject)) {
          if (rawChild is DependencyObject child) {
            if (child is T t) {
              yield return t;
            } else {
              foreach (var childOfChild in FindLogicalChildren<T>(child)) {
                yield return childOfChild;
              }
            }
          }
        }
      }
    }

    public static List<GeometryDrawing> GetDrawings(DrawingImage drawingImage) {
      if (drawingImage == null) {
        return new List<GeometryDrawing>();
      }
      if (drawingImage.Drawing is DrawingGroup drawingGroup) {
        return Icon.GetDrawings(drawingGroup);
      } else if (drawingImage.Drawing is GeometryDrawing geometryDrawing) {
        return new List<GeometryDrawing>() {
          geometryDrawing
        };
      }
      return new List<GeometryDrawing>();
    }

    public static List<GeometryDrawing> GetDrawings(DrawingGroup drawingGroup) {
      var drawings = new List<GeometryDrawing>();
      if (drawingGroup == null) {
        return drawings;
      }
      foreach (var drawing in drawingGroup.Children) {
        if (drawing is DrawingGroup) {
          drawings.AddRange(Icon.GetDrawings(drawing as DrawingGroup));
        } else if (drawing is GeometryDrawing geometryDrawing) {
          drawings.Add(geometryDrawing);
        }
      }
      return drawings;
    }
  }
}
