using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HAF {
  public interface IObservableTask {
    IObservableTaskPool Pool { get; }
    IObservableTaskProgress Progress { get; }
    bool IsCancelled { get; }
    RelayCommand DoCancel { get; }
    Task Run();
    Task Schedule();
    void Cancel();
  }
}
