using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HAF {

  public interface IReadOnlyObservableCollection<T>: IReadOnlyCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged {
  }

  public interface IObservableCollection<T>: ICollection<T>, IList<T>, INotifyCollectionChanged, INotifyPropertyChanged {
    void AddRange(IEnumerable<T> collection);
    void InsertRange(int index, IEnumerable<T> collection);
    void RemoveRange(IEnumerable<T> collection);
    int RemoveAll(Predicate<T> match);
    int RemoveAll(int index, int count, Predicate<T> match);
    void RemoveRange(int index, int count);
  }

  [DebuggerDisplay("Count = {Count}")]
  [Serializable]
  public class ObservableCollection<T>: ObservableObject, IObservableCollection<T>, IReadOnlyObservableCollection<T> {

    private readonly SimpleMonitor monitor = new SimpleMonitor();
    private DeferredEventsCollection deferredEvents;
    private readonly IList<T> items;

    public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

    public bool AllowDuplicates { get; set; } = true;

    EqualityComparer<T> comparer;
    public EqualityComparer<T> Comparer {
      get => comparer ?? EqualityComparer<T>.Default;
      private set => comparer = value;
    }

    public ObservableCollection() {
      this.items = new List<T>();
    }

    public ObservableCollection(IEnumerable<T> list) {
      this.items = new List<T>();
      this.items.CopyTo(list?.ToArray(), 0);
    }

    public int Count => this.items.Count;

    protected IList<T> Items => this.items;

    private void CheckReadOnly() {
      if (this.items.IsReadOnly) {
        throw new NotSupportedException("the collection is read-only");
      }
    }

    private void CheckIndex(int index) {
      if (index < 0 || index > this.items.Count) {
        throw new ArgumentOutOfRangeException();
      }
    }

    public T this[int index] {
      get => this.items[index];
      set {
        this.CheckReadOnly();
        if (index < 0 || index >= this.items.Count) {
          throw new ArgumentOutOfRangeException();
        }
        this.SetItem(index, value);
      }
    }

    public void Add(T item) {
      this.CheckReadOnly();
      this.InsertItem(this.items.Count, item);
    }

    public void AddRange(IEnumerable<T> collection) {
      this.InsertRange(this.Count, collection);
    }

    public void InsertRange(int index, IEnumerable<T> collection) {
      this.CheckReadOnly();
      this.CheckIndex(index);
      if (collection == null) {
        throw new ArgumentNullException(nameof(collection));
      }
      if (!this.AllowDuplicates) {
        collection = collection.Distinct(this.comparer).Where(item => !this.Items.Contains(item, this.comparer)).ToList();
      }
      if (collection is ICollection<T> countable) {
        if (countable.Count == 0) {
          return;
        }
      } else if (!collection.Any()) {
        return;
      }
      this.CheckReentrancy();
      var target = (List<T>)this.Items;
      target.InsertRange(index, collection);
      this.OnCollectionPropertiesChanged();
      if (!(collection is IList list)) {
        list = new List<T>(collection);
      }
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list, index));
    }

    public void RemoveRange(IEnumerable<T> collection) {
      if (collection == null) {
        throw new ArgumentNullException(nameof(collection));
      }
      if (this.Count == 0) {
        return;
      } else if (collection is ICollection<T> countable) {
        if (countable.Count == 0) {
          return;
        } else if (countable.Count == 1) {
          this.Remove(countable.First());
          return;
        }
      } else if (!collection.Any()) {
        return;
      }
      this.CheckReentrancy();
      var clusters = new Dictionary<int, List<T>>();
      var lastIndex = -1;
      List<T> lastCluster = null;
      foreach (var item in collection) {
        var index = this.IndexOf(item);
        if (index < 0) {
          continue;
        }
        this.Items.RemoveAt(index);
        if (lastIndex == index && lastCluster != null) {
          lastCluster.Add(item);
        } else {
          clusters[lastIndex = index] = lastCluster = new List<T> { item };
        }
      }
      this.OnCollectionPropertiesChanged();
      if (this.Count == 0) {
        this.OnCollectionReset();
      } else {
        foreach (var cluster in clusters) {
          this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, cluster.Value, cluster.Key));
        }
      }
    }

    public int RemoveAll(Predicate<T> match) {
      return this.RemoveAll(0, this.Count, match);
    }

    public int RemoveAll(int index, int count, Predicate<T> match) {
      if (index < 0) {
        throw new ArgumentOutOfRangeException(nameof(index));
      }
      if (count < 0) {
        throw new ArgumentOutOfRangeException(nameof(count));
      }
      if (index + count > Count) {
        throw new ArgumentOutOfRangeException(nameof(index));
      }
      if (match == null) {
        throw new ArgumentNullException(nameof(match));
      }
      if (this.Count == 0) {
        return 0;
      }
      List<T> cluster = null;
      var clusterIndex = -1;
      var removedCount = 0;
      using (this.BlockReentrancy()) {
        using (this.DeferEvents()) {
          for (var i = 0; i < count; i++, index++) {
            var item = this.Items[index];
            if (match(item)) {
              this.Items.RemoveAt(index);
              removedCount++;
              if (clusterIndex == index) {
                Debug.Assert(cluster != null);
                cluster.Add(item);
              } else {
                cluster = new List<T> { item };
                clusterIndex = index;
              }
              index--;
            } else if (clusterIndex > -1) {
              this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, cluster, clusterIndex));
              clusterIndex = -1;
              cluster = null;
            }
          }
          if (clusterIndex > -1) {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, cluster, clusterIndex));
          }
        }
      }
      if (removedCount > 0) {
        this.OnCollectionPropertiesChanged();
      }
      return removedCount;
    }

    public void RemoveRange(int index, int count) {
      if (index < 0) {
        throw new ArgumentOutOfRangeException(nameof(index));
      }
      if (count < 0) {
        throw new ArgumentOutOfRangeException(nameof(count));
      }
      if (index + count > Count) {
        throw new ArgumentOutOfRangeException(nameof(index));
      }
      if (count == 0) {
        return;
      }
      if (count == 1) {
        this.RemoveItem(index);
        return;
      }
      var items = (List<T>)this.Items;
      var removedItems = items.GetRange(index, count);
      this.CheckReentrancy();
      items.RemoveRange(index, count);
      this.OnCollectionPropertiesChanged();
      if (Count == 0) {
        this.OnCollectionReset();
      } else {
        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItems, index));
      }
    }

    public void Clear() {
      this.CheckReadOnly();
      this.ClearItems();
    }

    public void CopyTo(T[] array, int index) => this.items.CopyTo(array, index);

    public bool Contains(T item) => this.items.Contains(item, this.comparer);

    public IEnumerator<T> GetEnumerator() => this.items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.items.GetEnumerator();

    public int IndexOf(T item) => this.items.IndexOf(item);

    public void Insert(int index, T item) {
      this.CheckReadOnly();
      this.CheckIndex(index);
      this.InsertItem(index, item);
    }

    public bool Remove(T item) {
      this.CheckReadOnly();
      var index = this.items.IndexOf(item);
      if (index < 0) {
        return false;
      }
      this.RemoveItem(index);
      return true;
    }

    public void RemoveAt(int index) {
      this.CheckReadOnly();
      this.CheckIndex(index);
      this.RemoveItem(index);
    }

    bool ICollection<T>.IsReadOnly => this.items.IsReadOnly;

    public void Move(int oldIndex, int newIndex) => this.MoveItem(oldIndex, newIndex);

    protected virtual void InsertItem(int index, T item) {
      if (!this.AllowDuplicates && Items.Contains(item, this.comparer)) {
        return;
      }
      this.CheckReentrancy();
      this.items.Insert(index, item);
      this.OnCollectionPropertiesChanged();
      this.OnCollectionChanged(NotifyCollectionChangedAction.Add, (object)item, index);
    }

    protected virtual void SetItem(int index, T item) {
      if (this.AllowDuplicates) {
        if (this.Comparer.Equals(this[index], item)) {
          return;
        }
      } else if (this.Items.Contains(item, this.Comparer)) {
        return;
      }
      this.CheckReentrancy();
      var obj = this[index];
      this.items[index] = item;
      this.NotifyPropertyChanged("Item[]");
      this.OnCollectionChanged(NotifyCollectionChangedAction.Replace, (object)obj, (object)item, index);
    }

    protected virtual void MoveItem(int oldIndex, int newIndex) {
      this.CheckReentrancy();
      var obj = this[oldIndex];
      this.items.RemoveAt(oldIndex);
      this.items.Insert(newIndex, obj);
      this.NotifyPropertyChanged("Item[]");
      this.OnCollectionChanged(NotifyCollectionChangedAction.Move, (object)obj, newIndex, oldIndex);
    }

    protected virtual void ClearItems() {
      this.CheckReentrancy();
      this.items.Clear();
      this.NotifyPropertyChanged(() => this.Count);
      this.NotifyPropertyChanged("Item[]");
      this.OnCollectionReset();
    }

    protected virtual void RemoveItem(int index) {
      this.CheckReentrancy();
      var obj = this[index];
      this.items.RemoveAt(index);
      this.NotifyPropertyChanged(() => this.Count);
      this.NotifyPropertyChanged("Item[]");
      this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, (object)obj, index);
    }

    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
      if (this.CollectionChanged == null) {
        return;
      }
      using (this.BlockReentrancy()) {
        this.CollectionChanged((object)this, e);
      }
    }

    private void OnCollectionPropertiesChanged() {
      this.NotifyPropertyChanged(() => this.Count);
      this.NotifyPropertyChanged("Item[]");
    }

    private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index) {
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
    }

    private void OnCollectionChanged(
      NotifyCollectionChangedAction action,
      object item,
      int index,
      int oldIndex) {
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));
    }

    private void OnCollectionChanged(
      NotifyCollectionChangedAction action,
      object oldItem,
      object newItem,
      int index) {
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
    }

    private void OnCollectionReset() => this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

    protected void CheckReentrancy() {
      if (this.monitor.Busy && this.CollectionChanged != null && this.CollectionChanged.GetInvocationList().Length > 1) {
        throw new InvalidOperationException("reentrancy is not allowed for ObservableCollection");
      }
    }

    protected IDisposable BlockReentrancy() {
      this.monitor.Enter();
      return (IDisposable)this.monitor;
    }


    protected virtual IDisposable DeferEvents() => new DeferredEventsCollection(this);

    private class SimpleMonitor: IDisposable {
      private int _busyCount;

      public void Enter() => ++this._busyCount;

      public void Dispose() => --this._busyCount;

      public bool Busy => this._busyCount > 0;
    }

    private class DeferredEventsCollection: List<NotifyCollectionChangedEventArgs>, IDisposable {
      readonly ObservableCollection<T> collection;
      public DeferredEventsCollection(ObservableCollection<T> collection) {
        Debug.Assert(collection != null);
        Debug.Assert(collection.deferredEvents == null);
        this.collection = collection;
        this.collection.deferredEvents = this;
      }

      public void Dispose() {
        this.collection.deferredEvents = null;
        foreach (var args in this) {
          this.collection.OnCollectionChanged(args);
        }
      }
    }
  }
}