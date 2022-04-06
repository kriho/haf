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
  public class HelpButton: Button {
    public HelpButton() {
      this.Command = new RelayCommand(() => {
        try {
          System.Diagnostics.Process.Start(this.CommandParameter.ToString());
        } catch (Exception ex) {
          Log.Error($"failed to open help: {Utils.GetExceptionDescription(ex)}");
        }
      }, () => this.CommandParameter != null);
    }

    static HelpButton() {
      CommandParameterProperty.OverrideMetadata(typeof(Button), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCommandParameterPropertyChanged)));
    }

    private static void OnCommandParameterPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
      if(obj is HelpButton button && button.Command is IRelayCommand command) {
        command.RaiseCanExecuteChanged();
      }
    }
  }
}