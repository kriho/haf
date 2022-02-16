using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HAF {
  public interface IAsyncRelayCommand: INotifyPropertyChanged, ICommand {
    /// <summary>
    /// Task that is executed by the command.
    /// </summary>
    IObservableTask Task { get; }

    /// <summary>
    /// Execute the command and thereby its underlying task.
    /// </summary>
    Task ExecuteAsync();

    /// <summary>
    /// Test if the command can be executed.
    /// </summary>
    /// <returns>True, if the command can be executed.</returns>
    bool CanExecute();

    /// <summary>
    /// Notify a potential change in the executability of the command.
    /// </summary>
    void RaiseCanExecuteChanged();
  }
}
