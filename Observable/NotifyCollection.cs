using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {

  public interface IReadOnlyNotifyCollection<out T>: IReadOnlyCollection<T>, INotifyCollectionChanged {
  }

  public interface INotifyCollection<T>: ICollection<T>, INotifyCollectionChanged {
  }

  public interface IRangeNotifyCollection<T>: INotifyCollection<T> {
    void AddRange(IEnumerable<T> collection);
    void InsertRange(int index, IEnumerable<T> collection);
    void RemoveRange(IEnumerable<T> collection);
    int RemoveAll(Predicate<T> match);
    int RemoveAll(int index, int count, Predicate<T> match);
    void RemoveRange(int index, int count);
    void ReplaceRange(IEnumerable<T> collection);
    void ReplaceRange(int index, int count, IEnumerable<T> collection);
  }

  public class NotifyCollection<T>: ObservableCollection<T>, INotifyCollection<T>, IReadOnlyNotifyCollection<T> {

    public NotifyCollection(IEnumerable<T> collection): base(collection) {
    }

    public NotifyCollection(List<T> list) : base(list) {
    }

    public NotifyCollection() : base() {
    }
  }

  public class RangeNotifyCollection<T>: RangeObservableCollection<T>, IRangeNotifyCollection<T>, IReadOnlyNotifyCollection<T> {
  }

}
