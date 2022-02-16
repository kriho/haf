using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HAF {
  public interface IRelayCommand: ICommand {
    /// <summary>
    /// Execute the command.
    /// </summary>
    void Execute();

    /// <summary>
    /// Test if the command can be executed.
    /// </summary>
    /// <returns>True, if the command can be executed.</returns>
    bool CanExecute();
    void RaiseCanExecuteChanged();
  }

  public interface IRelayCommand<T>: ICommand {
    /// <summary>
    /// Execute the command with the provided command parameter.
    /// </summary>
    /// <param name="parameter">Command parameter.</param>
    void Execute(T parameter);

    /// <summary>
    /// Test if the command can be executed with the provided command parameter.
    /// </summary>
    /// <param name="parameter">Command parameter used for testing if the command can be executed.</param>
    /// <returns>True, if the command can be executed.</returns>
    bool CanExecute(T parameter);
    void RaiseCanExecuteChanged();
  }
}
