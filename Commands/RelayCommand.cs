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
    private LinkedDependency dependency;

    public event EventHandler CanExecuteChanged;

    public RelayCommand(Action execute) {
      this.execute = execute ?? throw new ArgumentNullException("execute");
      this.canExecute = null;
      this.dependency = null;
    }

    public RelayCommand(Action execute, Func<bool> canExecute) {
      this.execute = execute ?? throw new ArgumentNullException("execute");
      this.canExecute = canExecute ?? throw new ArgumentNullException("canExecute");
      this.dependency = null;
    }

    public RelayCommand(Action execute, LinkedDependency dependency) {
      this.execute = execute ?? throw new ArgumentNullException("execute");
      this.canExecute = null;
      this.dependency = dependency ?? throw new ArgumentNullException("dependency");
      this.dependency.RegisterUpdate(this.RaiseCanExecuteChanged);
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
      if(this.canExecute != null) {
        return this.canExecute.Invoke();
      }
      if(this.dependency != null) {
        return this.dependency;
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
    private LinkedDependency dependency;

    public event EventHandler CanExecuteChanged;

    public RelayCommand(Action<T> execute) {
      this.execute = execute ?? throw new ArgumentNullException("execute");
      this.canExecute = null;
      this.dependency = null;
    }

    public RelayCommand(Action<T> execute, Func<T, bool> canExecute) {
      this.execute = execute ?? throw new ArgumentNullException("execute");
      this.canExecute = canExecute ?? throw new ArgumentNullException("canExecute");
      this.dependency = null;
    }

    public RelayCommand(Action<T> execute, LinkedDependency dependency) {
      this.execute = execute ?? throw new ArgumentNullException("execute");
      this.canExecute = null;
      this.dependency = dependency ?? throw new ArgumentNullException("dependency");
      this.dependency.RegisterUpdate(this.RaiseCanExecuteChanged);
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
      if (this.canExecute != null) {
        return this.canExecute.Invoke((T)parameter);
      }
      if (this.dependency != null) {
        return this.dependency;
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
