using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HAF {
  public interface IObservableTask: INotifyPropertyChanged {
    IObservableTaskPool Pool { get; }
    IObservableTaskProgress Progress { get; }
    bool IsCancelled { get; }
    IReadOnlyState IsRunning { get; }
    RelayCommand DoCancel { get; }
    Task Run();
    Task Schedule();
    void Cancel();
    object Argument { get; set; }
  }
}
