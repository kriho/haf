using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HAF {

  public class RelayCommand : ICommand {
    private readonly WeakAction execute;
    private readonly WeakFunc<bool> canExecute;

    public event EventHandler CanExecuteChanged;

    public RelayCommand(Action execute) : this(execute, (Func<bool>)null) {
    }

    public RelayCommand(Action execute, Func<bool> canExecute = null) {
      if (execute == null) {
        throw new ArgumentNullException("execute");
      }
      this.execute = new WeakAction(execute);
      if (canExecute != null) {
        this.canExecute = new WeakFunc<bool>(canExecute);
      }
    }

    public RelayCommand(Action execute, LinkedDependency dependency) {
      if (execute == null) {
        throw new ArgumentNullException("execute");
      }
      this.execute = new WeakAction(execute);
      if (dependency != null) {
        var reference = new WeakReference<LinkedDependency>(dependency);
        this.canExecute = new WeakFunc<bool>(() => reference.TryGetTarget(out var target) ? target.Value : false);
      }
    }

    public bool CanExecute(object parameter) {
#if DEBUG
      // disable all commands in designe time
      if (ObservableObject.IsInDesignModeStatic) {
        return false;
      }
#endif
      return this.canExecute == null || (this.canExecute.IsAlive && this.canExecute.Execute());
    }

    public void Execute(object parameter) {
      if (this.CanExecute(parameter) && this.execute != null && this.execute.IsAlive) {
        this.execute.Execute();
      }
    }

    public void RaiseCanExecuteChanged() {
      this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
  }

  public class RelayCommand<T> : ICommand {
    private readonly WeakAction<T> execute;
    private readonly WeakFunc<T, bool> canExecute;

    public event EventHandler CanExecuteChanged;

    public RelayCommand(Action<T> execute) : this(execute, null) {
    }

    public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null) {
      if (execute == null) {
        throw new ArgumentNullException("execute");
      }
      this.execute = new WeakAction<T>(execute);
      if (canExecute != null) {
        this.canExecute = new WeakFunc<T, bool>(canExecute);
      }
    }

    public bool CanExecute(object parameter) {
#if DEBUG
      // disable all commands in designe time
      if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) {
        return false;
      }
#endif
      return this.canExecute == null || (this.canExecute.IsAlive && this.canExecute.Execute());
    }

    public void Execute(object parameter) {
      if (this.CanExecute(parameter) && this.execute != null && this.execute.IsAlive) {
        this.execute.Execute();
      }
    }

    public void RaiseCanExecuteChanged() {
      this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
  }
}
