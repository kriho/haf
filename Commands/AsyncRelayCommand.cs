using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Runtime.CompilerServices;

namespace HAF {

  public class AsyncRelayCommand: ObservableObject, IAsyncRelayCommand {
    public IObservableTask Task { get; private set; }
    private readonly Func<bool> canExecute;
    private IReadOnlyState dependentState;

    public event EventHandler CanExecuteChanged;

    public AsyncRelayCommand(IObservableTask task) {
      this.Task = task ?? throw new ArgumentNullException(nameof(task));
      this.canExecute = null;
      this.dependentState = null;
      task.PropertyChanged += this.Task_PropertyChanged;
    }

    public AsyncRelayCommand(IObservableTask task, Func<bool> canExecute) {
      this.Task = task ?? throw new ArgumentNullException(nameof(task));
      this.canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
      this.dependentState = null;
    }

    public AsyncRelayCommand(IObservableTask task, IReadOnlyState dependentState) {
      this.Task = task ?? throw new ArgumentNullException(nameof(task));
      this.canExecute = null;
      this.dependentState = dependentState ?? throw new ArgumentNullException(nameof(dependentState));
      this.dependentState.RegisterUpdate(this.RaiseCanExecuteChanged);
    }

    private void Task_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      if(e.PropertyName == nameof(this.Task.IsRunning)) {
        this.RaiseCanExecuteChanged();
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CanExecute() {
      return this.CanExecute(null);
    }

    public bool CanExecute(object parameter) {
#if DEBUG
      // enable all commands in designe time
      if(this.IsInDesignMode) {
        return true;
      }
#endif
      if(this.Task.IsRunning.Value) {
        return false;
      }
      if(this.canExecute != null && !this.canExecute.Invoke()) {
        return false;
      }
      if(this.dependentState != null && !this.dependentState.Value) {
        return false;
      }
      return true;
    }

    public Task ExecuteAsync() {
      if(this.CanExecute()) {
        return this.Task.Schedule();
      } else {
        return System.Threading.Tasks.Task.CompletedTask;
      }
    }

    public void Execute(object parameter) {
      _ = this.ExecuteAsync();
    }

    public void RaiseCanExecuteChanged() {
      this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
  }
}
