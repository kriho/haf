using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace HAF {
  public class ManualNotifyCollection<T>: List<T>, INotifyCollectionChanged {
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    public void NotifyCollectionChanged() {
      this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
  }
}