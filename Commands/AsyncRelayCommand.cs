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
    private LinkedDependency dependency;
    private CancellationTokenSource cancellationTokenSource;

    public event EventHandler CanExecuteChanged;

    public bool IsCancellationRequested {
      get => this.cancellationTokenSource != null && this.cancellationTokenSource.IsCancellationRequested;
    }

    public AsyncRelayCommand(IObservableTask task) {
      this.Task = task ?? throw new ArgumentNullException(nameof(task));
      this.canExecute = null;
      this.dependency = null;
      task.PropertyChanged += this.Task_PropertyChanged;
    }

    public AsyncRelayCommand(IObservableTask task, Func<bool> canExecute) {
      this.Task = task ?? throw new ArgumentNullException(nameof(task));
      this.canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
      this.dependency = null;
    }

    public AsyncRelayCommand(IObservableTask task, LinkedDependency dependency) {
      this.Task = task ?? throw new ArgumentNullException(nameof(task));
      this.canExecute = null;
      this.dependency = dependency ?? throw new ArgumentNullException(nameof(dependency));
      this.dependency.RegisterUpdate(this.RaiseCanExecuteChanged);
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
      if(this.Task.IsRunning) {
        return false;
      }
      if(this.canExecute != null) {
        return this.canExecute.Invoke();
      }
      if(this.dependency != null) {
        return this.dependency;
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
