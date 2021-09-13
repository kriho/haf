using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public class WaitingObservableTask: IWaitingObservableTask {
    public IObservableTask Task { get; set; }
    public TaskCompletionSource<bool> CompletionSource { get; set; }
    public DateTime ScheduledAt { get; set; }
  }
}
