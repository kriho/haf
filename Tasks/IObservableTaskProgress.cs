using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {

  public interface IObservableTaskProgress: IProgress<int>, INotifyPropertyChanged {
    void ReportIndeterminate(string description = null);
    void ReportProgress(int? value = null);
    void ReportProgress(string description);
    void ReportProgress(int value, string description);
    void ReportProgress(int value, int maximum, string description = "", int? normalizer = null);
    void NormalizeProgress(int? normalizer);
    int Maximum { get; }
    int Value { get; }
    bool IsRunning { get; }
    bool IsIndeterminate { get; }
    string Description { get; }
    int? Normalizer { get; }
  }

}
