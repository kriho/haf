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
    Color Background {get; set; }
    Color LastBackground { get; set; }
    Color BackgroundInfo { get; set; }
    Color LastBackgroundInfo { get; set; }
    Color BackgroundWarning { get; set; }
    Color LastBackgroundWarning { get; set; }
    Color BackgroundError { get; set; }
    Color LastBackgroundError { get; set; }
    Color Control {get; set; }
    Color LastControl { get; set; }
    Color Light {get; set; }
    Color LastLight { get; set; }
    Color Strong {get; set; }
    Color LastStrong { get; set; }
    Color Secondary {get; set; }
    Color LastSecondary { get; set; }
    Color InvertedSecondary { get; set; }
    Color LastInvertedSecondary { get; set; }
    Color Text {get; set; }
    Color LastText {get; set; }
    Color InvertedText { get; set; }
    Color LastInvertedText { get; set; }
    Color TextInfo { get; set; }
    Color LastTextInfo { get; set; }
    Color TextWarning { get; set; }
    Color LastTextWarning { get; set; }
    Color TextError { get; set; }
    Color LastTextError { get; set; }
    Color Accent {get; set; }
    Color LastAccent {get; set; }
    Color Action {get; set; }
    Color LastAction {get; set; }
    IState IsActive { get; }
    IReadOnlyState IsEditable { get; }
    IReadOnlyState IsDirty { get; }
    IRelayCommand DoApplyChanges { get; }
  }
}
