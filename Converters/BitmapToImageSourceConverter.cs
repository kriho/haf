using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#if NET48

namespace HAF.Converters {
  public class BitmapToImageSourceConverter: ValueConverter<Bitmap, ImageSource> {

    protected override ImageSource convert(Bitmap value) {
      if (value == null) {
        return null;
      }
      // convert to image source
      using (Stream stream = new MemoryStream()) {
        value.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
        stream.Seek(0, SeekOrigin.Begin);
        BitmapDecoder decoder = new BmpBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
        return decoder.Frames[0];
      }
    }
  }
}

#endif