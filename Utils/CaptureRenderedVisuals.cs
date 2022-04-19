using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace HAF {
  public static partial class Utils {
    public static void CaptureRenderedVisual(UIElement source, Stream destination, Thickness croppedMargin) {
      var renderTarget = new RenderTargetBitmap((int)source.RenderSize.Width, (int)source.RenderSize.Height, 96, 96, PixelFormats.Default);
      var visualBrush = new VisualBrush(source);
      var drawingVisual = new DrawingVisual();
      using(var drawingContext = drawingVisual.RenderOpen()) {
        drawingContext.DrawRectangle(visualBrush, null, new Rect(new Point(croppedMargin.Left, croppedMargin.Top), new Point(source.RenderSize.Width - croppedMargin.Left - croppedMargin.Right, source.RenderSize.Height - croppedMargin.Top - croppedMargin.Bottom)));
      }
      renderTarget.Render(drawingVisual);
      var encoder = new PngBitmapEncoder();
      encoder.Frames.Add(BitmapFrame.Create(renderTarget));
      encoder.Save(destination);
    }

    public static void CaptureRenderedVisual(UIElement source, Stream destination) {
      CaptureRenderedVisual(source, destination, new Thickness(0));
    }
  }
}
