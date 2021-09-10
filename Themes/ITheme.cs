using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HAF {
  public interface ITheme {
    LocalizedText Name { get; }
    Color BackgroundColor { get; }
    Color ControlColor { get; }
    Color TextColor { get; }
    Color AccentColor { get; }
    Color ActionColor { get; }
    Color LightColor { get; }
    Color StrongColor { get; }
    Color InfoColor { get; }
    Color WarningColor { get; }
    Color ErrorColor { get; }
  }
}
