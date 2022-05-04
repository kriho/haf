using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HAF {

  public interface IReadOnlyObservableCollection<T>: IReadOnlyList<T>, IReadOnlyCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged {
    int IndexOf(T item);
  }

  public interface IObservableCollection<T>: IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged {
    new void Clear();
    new int Count { get; }
    new void RemoveAt(int index);
  }

  public class ObservableCollection<T>: System.Collections.ObjectModel.ObservableCollection<T>, IObservableCollection<T>, IReadOnlyObservableCollection<T> {

    public ObservableCollection(): base() {
    }

    public ObservableCollection(IEnumerable<T> collection): base(collection) {
    }

    public ObservableCollection(List<T> list): base(list) {
    }
  }
}