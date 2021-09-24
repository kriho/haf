using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HAF {
  public interface IRelayCommand: ICommand {
    void Execute();
    bool CanExecute();
    void RaiseCanExecuteChanged();
  }

  public interface IRelayCommand<T>: ICommand {
    void Execute(T parameter);
    bool CanExecute(T parameter);
    void RaiseCanExecuteChanged();
  }
}
