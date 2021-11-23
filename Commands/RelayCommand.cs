using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Runtime.CompilerServices;

namespace HAF {

  public class RelayCommand: IRelayCommand {
    private readonly Action execute;
    private readonly Func<bool> canExecute;
    private IReadOnlyState dependentState;

    public event EventHandler CanExecuteChanged;

    public RelayCommand(Action execute) {
      this.execute = execute ?? throw new ArgumentNullException("execute");
      this.canExecute = null;
      this.dependentState = null;
    }

    public RelayCommand(Action execute, Func<bool> canExecute) {
      this.execute = execute ?? throw new ArgumentNullException("execute");
      this.canExecute = canExecute ?? throw new ArgumentNullException("canExecute");
      this.dependentState = null;
    }

    public RelayCommand(Action execute, IReadOnlyState dependentState) {
      this.execute = execute ?? throw new ArgumentNullException("execute");
      this.canExecute = null;
      this.dependentState = dependentState ?? throw new ArgumentNullException("dependency");
      this.dependentState.RegisterUpdate(this.RaiseCanExecuteChanged);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CanExecute() {
      return this.CanExecute(null);
    }

    public bool CanExecute(object parameter) {
#if DEBUG
      // disable all commands in designe time
      if(ObservableObject.IsInDesignModeStatic) {
        return true;
      }
#endif
      if(this.canExecute != null && !this.canExecute.Invoke()) {
        return false;
      }
      if(this.dependentState != null && !this.dependentState.Value) {
        return false;
      }
      return true;
    }

    public void Execute(object parameter) {
      if (this.CanExecute(parameter)) {
        this.execute.Invoke();
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Execute() {
      this.Execute(null);
    }

    public void RaiseCanExecuteChanged() {
      this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
  }

  public class RelayCommand<T>: IRelayCommand<T> {
    private readonly Action<T> execute;
    private readonly Func<T, bool> canExecute;
    private IReadOnlyState dependentState;

    public event EventHandler CanExecuteChanged;

    public RelayCommand(Action<T> execute) {
      this.execute = execute ?? throw new ArgumentNullException("execute");
      this.canExecute = null;
      this.dependentState = null;
    }

    public RelayCommand(Action<T> execute, Func<T, bool> canExecute) {
      this.execute = execute ?? throw new ArgumentNullException("execute");
      this.canExecute = canExecute ?? throw new ArgumentNullException("canExecute");
      this.dependentState = null;
    }

    public RelayCommand(Action<T> execute, IReadOnlyState dependentState) {
      this.execute = execute ?? throw new ArgumentNullException("execute");
      this.canExecute = null;
      this.dependentState = dependentState ?? throw new ArgumentNullException("dependency");
      this.dependentState.RegisterUpdate(this.RaiseCanExecuteChanged);
    }

    public RelayCommand(Action<T> execute, IReadOnlyState dependentState, Func<T, bool> canExecute) {
      this.execute = execute ?? throw new ArgumentNullException("execute");
      this.canExecute = canExecute ?? throw new ArgumentNullException("canExecute");
      this.dependentState = dependentState ?? throw new ArgumentNullException("dependency");
      this.dependentState.RegisterUpdate(this.RaiseCanExecuteChanged);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CanExecute(T parameter) {
      return this.CanExecute((object)parameter);
    }

    public bool CanExecute(object parameter) {
#if DEBUG
      // disable all commands in designe time
      if (ObservableObject.IsInDesignModeStatic) {
        return true;
      }
#endif
      if (this.canExecute != null && !this.canExecute.Invoke((T)parameter)) {
        return false;
      }
      if (this.dependentState != null && !this.dependentState.Value) {
        return false;
      }
      return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Execute(T parameter) {
      this.Execute((object)parameter);
    }

    public void Execute(object parameter) {
      if (this.CanExecute(parameter)) {
        this.execute.Invoke((T)parameter);
      }
    }

    public void RaiseCanExecuteChanged() {
      this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
  }
}
