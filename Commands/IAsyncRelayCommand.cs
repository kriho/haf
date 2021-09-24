using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HAF {
  public interface IAsyncRelayCommand: INotifyPropertyChanged, ICommand {
    IObservableTask Task { get; }
    Task ExecuteAsync();
    bool CanExecute();
    void RaiseCanExecuteChanged();
  }
}
