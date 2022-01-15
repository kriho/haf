using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HAF {
  public interface ITheme: INotifyPropertyChanged {
    LocalizedText Name { get; }
    Color BackgroundColor { get; set; }
    Color ControlColor { get; set; }
    Color LightColor { get; set; }
    Color StrongColor { get; set; }
    Color SecondaryColor { get; set; }
    Color TextColor { get; set; }
    Color AccentColor { get; set; }
    Color ActionColor { get; set; }
    Color InfoForegroundColor { get; set; }
    Color InfoBackgroundColor { get; set; }
    Color WarningForegroundColor { get; set; }
    Color WarningBackgroundColor { get; set; }
    Color ErrorForegroundColor { get; set; }
    Color ErrorBackgroundColor { get; set; }
    Color LastBackgroundColor { get; set; }
    Color LastControlColor { get; set; }
    Color LastLightColor { get; set; }
    Color LastStrongColor { get; set; }
    Color LastSecondaryColor { get; set; }
    Color LastTextColor { get; set; }
    Color LastAccentColor { get; set; }
    Color LastActionColor { get; set; }
    Color LastInfoForegroundColor { get; set; }
    Color LastInfoBackgroundColor { get; set; }
    Color LastWarningForegroundColor { get; set; }
    Color LastWarningBackgroundColor { get; set; }
    Color LastErrorForegroundColor { get; set; }
    Color LastErrorBackgroundColor { get; set; }
    bool IsActive { get; set; }
    bool IsEditable { get; }
    IReadOnlyState IsDirty { get; }
    IRelayCommand DoApplyChanges { get; }
  }
}
