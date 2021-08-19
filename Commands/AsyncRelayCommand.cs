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

  public interface IAsyncRelayCommand: INotifyPropertyChanged, ICommand {
    bool CanBeCanceled { get; }
    Task Task { get; }
    bool IsCancellationRequested { get; }
    bool IsRunning { get; }
    void Cancel();
    Task ExecuteAsync();
    void RaiseCanExecuteChanged();
  }

  public class AsyncRelayCommand: ObservableObject, IAsyncRelayCommand {
    private readonly Func<Task> execute;
    private readonly Func<CancellationToken, Task> cancellableExecute;
    private readonly Func<bool> canExecute;
    private LinkedDependency dependency;
    private CancellationTokenSource cancellationTokenSource;

    public event EventHandler CanExecuteChanged;

    public bool CanBeCanceled {
      get => this.cancellationTokenSource != null && this.IsRunning;
    }

    public Task Task { get; private set; }

    private bool isRunning;
    public bool IsRunning {
      get => this.isRunning;
      set {
        if (this.SetValue(ref this.isRunning, value)) {
          this.NotifyPropertyChanged(() => this.CanBeCanceled);
          this.RaiseCanExecuteChanged();
        }
      }
    }

    public bool IsCancellationRequested {
      get => this.cancellationTokenSource != null && this.cancellationTokenSource.IsCancellationRequested;
    }

    public AsyncRelayCommand(Func<Task> execute) {
      this.execute = execute ?? throw new ArgumentNullException("execute");
      this.cancellableExecute = null;
      this.canExecute = null;
      this.dependency = null;
      this.Task = null;
    }

    public AsyncRelayCommand(Func<CancellationToken, Task> execute) {
      this.execute = null;
      this.cancellableExecute = execute ?? throw new ArgumentNullException("execute");
      this.canExecute = null;
      this.dependency = null;
      this.Task = null;
      this.cancellationTokenSource = null;
    }

    public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute) {
      this.execute = execute ?? throw new ArgumentNullException("execute");
      this.cancellableExecute = null;
      this.canExecute = canExecute ?? throw new ArgumentNullException("canExecute");
      this.dependency = null;
      this.Task = null;
      this.cancellationTokenSource = null;
    }

    public AsyncRelayCommand(Func<CancellationToken, Task> execute, Func<bool> canExecute) {
      this.execute = null;
      this.cancellableExecute = execute ?? throw new ArgumentNullException("execute");
      this.canExecute = canExecute ?? throw new ArgumentNullException("canExecute");
      this.dependency = null;
      this.Task = null;
      this.cancellationTokenSource = null;
    }

    public AsyncRelayCommand(Func<Task> execute, LinkedDependency dependency) {
      this.execute = execute ?? throw new ArgumentNullException("execute");
      this.cancellableExecute = null;
      this.canExecute = null;
      this.dependency = dependency ?? throw new ArgumentNullException("dependency");
      this.dependency.RegisterUpdate(this.RaiseCanExecuteChanged);
      this.Task = null;
      this.cancellationTokenSource = null;
    }

    public AsyncRelayCommand(Func<CancellationToken, Task> execute, LinkedDependency dependency) {
      this.execute = null;
      this.cancellableExecute = execute ?? throw new ArgumentNullException("execute");
      this.canExecute = null;
      this.dependency = dependency ?? throw new ArgumentNullException("dependency");
      this.dependency.RegisterUpdate(this.RaiseCanExecuteChanged);
      this.Task = null;
      this.cancellationTokenSource = null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CanExecute(object parameter) {
    #if DEBUG
      // disable all commands in designe time
      if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) {
        return false;
      }
    #endif
      if (this.isRunning) {
        return false;
      }
      if (this.canExecute != null) {
        return this.canExecute.Invoke();
      }
      if (this.dependency != null) {
        return this.dependency;
      }
      return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CanExecute() {
      return this.CanExecute(null);
    }

    public void Execute(object parameter) {
    #pragma warning disable CS4014 // async not supported
      this.ExecuteAsync();
    #pragma warning restore CS4014
    }

    public async Task ExecuteAsync() {
      if (this.CanExecute()) {
        try {
          if (this.execute != null) {
            this.Task = this.execute.Invoke();
          }
          if (this.cancellableExecute != null) {
            this.cancellationTokenSource = new CancellationTokenSource();
            this.Task = this.cancellableExecute.Invoke(this.cancellationTokenSource.Token);
          }
          this.IsRunning = true;
          await this.Task;
        } catch (TaskCanceledException) {
        } finally {
          this.IsRunning = false;
        }
      }
    }

    public void RaiseCanExecuteChanged() {
      this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Cancel() {
      if (this.CanBeCanceled) {
        this.cancellationTokenSource.Cancel();
      }
    }
  }


  public interface IAsyncRelayCommand<T>: INotifyPropertyChanged, ICommand {
    bool CanBeCanceled { get; }
    Task Task { get; }
    bool IsCancellationRequested { get; }
    bool IsRunning { get; }
    void Cancel();
    Task ExecuteAsync(T parameter);
    void RaiseCanExecuteChanged();
  }

  public class AsyncRelayCommand<T>: ObservableObject, IAsyncRelayCommand<T> {
    private readonly Func<T, Task> execute;
    private readonly Func<T, CancellationToken, Task> cancellableExecute;
    private readonly Func<T, bool> canExecute;
    private LinkedDependency dependency;
    private CancellationTokenSource cancellationTokenSource;

    public event EventHandler CanExecuteChanged;

    public bool CanBeCanceled {
      get => this.cancellationTokenSource != null && this.IsRunning;
    }

    public Task Task { get; private set; }

    private bool isRunning;
    public bool IsRunning {
      get => this.isRunning;
      set {
        if (this.SetValue(ref this.isRunning, value)) {
          this.NotifyPropertyChanged(() => this.CanBeCanceled);
          this.RaiseCanExecuteChanged();
        }
      }
    }

    public bool IsCancellationRequested {
      get => this.cancellationTokenSource != null && this.cancellationTokenSource.IsCancellationRequested;
    }

    public AsyncRelayCommand(Func<T, Task> execute) {
      this.execute = execute ?? throw new ArgumentNullException("execute");
      this.cancellableExecute = null;
      this.canExecute = null;
      this.dependency = null;
      this.Task = null;
      this.isRunning = false;
      this.cancellationTokenSource = null;
    }


    public AsyncRelayCommand(Func<T, CancellationToken, Task> execute) {
      this.execute = null;
      this.cancellableExecute = execute ?? throw new ArgumentNullException("execute");
      this.canExecute = null;
      this.dependency = null;
      this.Task = null;
      this.isRunning = false;
      this.cancellationTokenSource = null;
    }

    public AsyncRelayCommand(Func<T, Task> execute, Func<T, bool> canExecute) {
      this.execute = execute ?? throw new ArgumentNullException("execute");
      this.cancellableExecute = null;
      this.canExecute = canExecute ?? throw new ArgumentNullException("canExecute");
      this.dependency = null;
      this.Task = null;
      this.isRunning = false;
      this.cancellationTokenSource = null;
    }

    public AsyncRelayCommand(Func<T, CancellationToken, Task> execute, Func<T, bool> canExecute) {
      this.execute = null;
      this.cancellableExecute = execute ?? throw new ArgumentNullException("execute");
      this.canExecute = canExecute ?? throw new ArgumentNullException("canExecute");
      this.dependency = null;
      this.Task = null;
      this.isRunning = false;
      this.cancellationTokenSource = null;
    }

    public AsyncRelayCommand(Func<T, Task> execute, LinkedDependency dependency) {
      this.execute = execute ?? throw new ArgumentNullException("execute");
      this.cancellableExecute = null;
      this.canExecute = null;
      this.dependency = dependency ?? throw new ArgumentNullException("dependency");
      this.dependency.RegisterUpdate(this.RaiseCanExecuteChanged);
      this.Task = null;
      this.isRunning = false;
      this.cancellationTokenSource = null;
    }

    public AsyncRelayCommand(Func<T, CancellationToken, Task> execute, LinkedDependency dependency) {
      this.execute = null;
      this.cancellableExecute = execute ?? throw new ArgumentNullException("execute");
      this.canExecute = null;
      this.dependency = dependency ?? throw new ArgumentNullException("dependency");
      this.dependency.RegisterUpdate(this.RaiseCanExecuteChanged);
      this.Task = null;
      this.isRunning = false;
      this.cancellationTokenSource = null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CanExecute(object parameter) {
    #if DEBUG
      // disable all commands in designe time
      if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) {
        return false;
      }
    #endif
      if(this.isRunning) {
        return false;
      }
      if (this.canExecute != null) {
        return this.canExecute.Invoke((T)parameter);
      }
      if (this.dependency != null) {
        return this.dependency;
      }
      return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CanExecute(T parameter) {
      return this.CanExecute((object)parameter);
    }

    public void Execute(object parameter) {
    #pragma warning disable CS4014 // async not supported
      this.ExecuteAsync((T)parameter);
    #pragma warning restore CS4014
    }

    public async Task ExecuteAsync(T parameter) {
      if (this.CanExecute(parameter)) {
        try {
          if (this.execute != null) {
            this.Task = this.execute.Invoke(parameter);
          }
          if (this.cancellableExecute != null) {
            this.cancellationTokenSource = new CancellationTokenSource();
            this.Task = this.cancellableExecute.Invoke(parameter, this.cancellationTokenSource.Token);
          }
          this.IsRunning = true;
          await this.Task;
        } catch (TaskCanceledException) {
        } finally {
          this.IsRunning = false;
        }
      }
    }

    public void RaiseCanExecuteChanged() {
      this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Cancel() {
      if(this.CanBeCanceled) {
        this.cancellationTokenSource.Cancel();
      }
    }
  }
  
}
