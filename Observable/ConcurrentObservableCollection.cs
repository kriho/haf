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
  internal class SynchronizedCollectionChange<T> {
    public NotifyCollectionChangedAction Action;
    public int NewIndex;
    public List<T> NewItems;
    public int OldIndex;
    public List<T> OldItems;
  }

  public interface IReadOnlyConcurrentObservableCollection<T>: IReadOnlyList<T> {
    ConcurrentObservableCollection<T>.IObservableProxy<T> Observable { get; }
    int IndexOf(T item);
  }

  public interface IConcurrentObservableCollection<T>: IList<T>, IList {
    ConcurrentObservableCollection<T>.IObservableProxy<T> Observable { get; }
  }

  [DebuggerDisplay("Count = {Count}")]
  [Serializable]
  public class ConcurrentObservableCollection<T>: IConcurrentObservableCollection<T>, IReadOnlyConcurrentObservableCollection<T> {

    public interface IObservableProxy<U>: IList, IEnumerable<U>, INotifyCollectionChanged, INotifyPropertyChanged {
    }

    public class ObservableProxy<U>: IObservableProxy<U> {
      private readonly List<U> items;
      private readonly ConcurrentObservableCollection<U> parent;

      public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

      public event PropertyChangedEventHandler PropertyChanged;

      public ObservableProxy(ConcurrentObservableCollection<U> parent) {
        this.parent = parent;
        this.items = new List<U>();
      }

      public ObservableProxy(ConcurrentObservableCollection<U> parent, IEnumerable<U> collection) {
        this.parent = parent;
        this.items = new List<U>(collection);
      }

      internal void ApplyCollectionChanges(List<SynchronizedCollectionChange<U>> changes, bool reset) {
        foreach(var change in changes) {
          switch(change.Action) {
            case NotifyCollectionChangedAction.Reset:
              // reset means clear in this context
              this.items.Clear();
              break;
            case NotifyCollectionChangedAction.Add:
              if(!reset) {
                for(var index = 0; index < change.NewItems.Count; index++) {
                  this.items.Insert(change.NewIndex + index, change.NewItems[index]);
                  this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, change.NewItems[index], change.NewIndex + index));
                }
              } else {
                this.items.InsertRange(change.NewIndex, change.NewItems);
              }
              break;
            case NotifyCollectionChangedAction.Remove:
              if(!reset) {
                for(var index = 0; index < change.NewItems.Count; index++) {
                  this.items.RemoveAt(change.NewIndex);
                  this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, change.NewItems[index], change.NewIndex));
                }
              } else {
                this.items.RemoveRange(change.NewIndex, change.NewItems.Count);
              }
              break;
            case NotifyCollectionChangedAction.Replace:
              this.items[change.NewIndex] = change.NewItems[0];
              if(!reset) {
                this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, change.NewItems[0], change.OldItems[0], change.NewIndex));
              }
              break;
            case NotifyCollectionChangedAction.Move:
              if(!reset) {
                for(var index = 0; index < change.NewItems.Count; index++) {
                  this.items.RemoveAt(change.OldIndex + index);
                  this.items.Insert(change.NewIndex + index, change.NewItems[index]);
                  this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, change.NewItems[index], change.NewIndex + index, change.OldIndex + index));
                }
              } else {
                this.items.RemoveRange(change.OldIndex, change.NewItems.Count);
                this.items.InsertRange(change.NewIndex, change.NewItems);
              }
              break;
          }
        }
        if(reset) {
          this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Items[]"));
      }

      public bool IsReadOnly => false;

      public bool IsFixedSize => false;

      public int Count => this.items.Count;

      public object SyncRoot => this.parent.Lock;

      public bool IsSynchronized => true;

      public object this[int index] {
        get => this.items[index];
        set => ((IList)this.parent)[index] = value;
      }

      public int Add(object value) {
        return this.parent.Add(value);
      }

      public void Clear() {
        this.parent.Clear();
      }

      public bool Contains(object value) {
        return this.parent.Contains((U)value);
      }

      public void CopyTo(Array array, int index) {
        this.items.CopyTo((U[])array, index);
      }

      public IEnumerator GetEnumerator() {
        return this.items.GetEnumerator();
      }

      IEnumerator<U> IEnumerable<U>.GetEnumerator() {
        return this.items.GetEnumerator();
      }

      public int IndexOf(object value) {
        return this.items.IndexOf((U)value);
      }

      public void Insert(int index, object value) {
        this.parent.Insert(index, (U)value);
      }

      public void Remove(object value) {
        this.parent.Remove((U)value);
      }

      public void RemoveAt(int index) {
        this.parent.RemoveAt(index);
      }
    }

    private readonly List<T> items;

    private List<SynchronizedCollectionChange<T>> changes = new List<SynchronizedCollectionChange<T>>();

    private bool isInvoked = false;

    internal object Lock = new object();

    private Dispatcher dispatcher;

    private ObservableProxy<T> observable;
    public IObservableProxy<T> Observable => this.observable;

    public int ChunkSize { get; set; } = 5;

    public ConcurrentObservableCollection() {
      this.items = new List<T>();
      this.dispatcher = System.Windows.Application.Current.Dispatcher;
      this.observable = new ObservableProxy<T>(this);
    }

    public ConcurrentObservableCollection(IEnumerable<T> collection) {
      this.items = new List<T>(collection);
      this.dispatcher = System.Windows.Application.Current.Dispatcher;
      this.observable = new ObservableProxy<T>(this, collection);
    }

    /// <summary>
    /// run on UI thread to notify changes
    /// </summary>
    private void NotifyChanges() {
      var handledChanges = new List<SynchronizedCollectionChange<T>>();
      lock (this.Lock) {
        handledChanges.AddRange(this.changes);
        this.changes.Clear();
        this.isInvoked = false;
      }
      // check if a reset is needed
      var reset = false;
      var changeCount = 0;
      foreach(var change in handledChanges) {
        if(change.Action == NotifyCollectionChangedAction.Reset) {
          // reset means clear in this context
          reset = true;
          break;
        } else {
          changeCount += change.NewItems.Count;
          if(changeCount > this.ChunkSize) {
            reset = true;
            break;
          }
        }
      }
      // synchronize proxy
      this.observable.ApplyCollectionChanges(handledChanges, reset);
    }

    private void QueueChange(NotifyCollectionChangedAction action, T newItem, int newIndex, T oldItem, int oldIndex) {
      this.QueueChange(action, new List<T>() { newItem }, newIndex, new List<T>() { oldItem }, oldIndex);
    }

    private void QueueChange(NotifyCollectionChangedAction action, List<T> newItems, int newIndex, List<T> oldItems, int oldIndex) {
      if(action == NotifyCollectionChangedAction.Reset) {
        this.changes.Add(new SynchronizedCollectionChange<T> {
          Action = NotifyCollectionChangedAction.Reset
        });
      } else {
        var lastChange = this.changes.LastOrDefault();
        if(lastChange != null && lastChange.Action == action) {
          if(action == NotifyCollectionChangedAction.Add && (lastChange.NewIndex + lastChange.NewItems.Count) == newIndex) {
            lastChange.NewItems.AddRange(newItems);
            return;
          }
          if(action == NotifyCollectionChangedAction.Remove && lastChange.NewIndex == newIndex) {
            lastChange.NewItems.AddRange(newItems);
            return;
          }
        }
        this.changes.Add(new SynchronizedCollectionChange<T> {
          Action = action,
          NewIndex = newIndex,
          NewItems = newItems,
          OldIndex = oldIndex,
          OldItems = oldItems,
        });
      }
      if(!this.isInvoked) {
        this.isInvoked = true;
        _ = this.dispatcher.BeginInvoke(new Action(this.NotifyChanges), DispatcherPriority.Normal);
      }
    }

    public int Count {
      get {
        lock(this.Lock) {
          return this.items.Count;
        }
      }
    }

    public bool IsReadOnly => false;

    public bool IsFixedSize => false;

    public object SyncRoot => this.Lock;

    public bool IsSynchronized => true;

    public T this[int index] {
      get {
        lock(this.Lock) {
          return this.items[index];
        }
      }
      set {
        lock(this.Lock) {
          var oldItem = this[index];
          this.items[index] = value;
          this.QueueChange(NotifyCollectionChangedAction.Replace, value, index, oldItem, -1);
        }
      }
    }

    object IList.this[int index] {
      get => this[index];
      set => this[index] = (T)value;
    }

    public void Add(T item) {
      this.Add((object)item);
    }

    public int Add(object value) {
      lock(this.Lock) {
        var index = this.items.Count;
        this.items.Add((T)value);
        this.QueueChange(NotifyCollectionChangedAction.Add, (T)value, index, default(T), -1);
        return index;
      }
    }

    public void AddRange(IEnumerable<T> collection) {
      if(!(collection is List<T> items)) {
        items = new List<T>(collection);
      }
      lock(this.Lock) {
        var index = this.items.Count;
        this.items.AddRange(items);
        this.QueueChange(NotifyCollectionChangedAction.Add, items, index, null, -1);
      }
    }

    public void Insert(int index, T item) {
      lock(this.Lock) {
        this.items.Insert(index, item);
        this.QueueChange(NotifyCollectionChangedAction.Add, item, index, default(T), -1);
      }
    }

    public void Insert(int index, object value) {
      this.Insert(index, (T)value);
    }

    public void InserRange(int index, IEnumerable<T> collection) {
      if(!(collection is List<T> items)) {
        items = new List<T>(collection);
      }
      lock(this.Lock) {
        this.items.InsertRange(index, items);
        this.QueueChange(NotifyCollectionChangedAction.Add, items, index, null, -1);
      }
    }

    public void Clear() {
      lock(this.Lock) {
        this.items.Clear();
        // reset means clear in this context
        this.QueueChange(NotifyCollectionChangedAction.Reset, null, -1, null, -1);
      }
    }

    public void CopyTo(T[] array, int index) {
      lock(this.Lock) {
        this.items.CopyTo(array, index);
      }
    }

    public void CopyTo(Array array, int index) {
      this.CopyTo((T[])array, index);
    }

    public bool Contains(T item) {
      lock(this.Lock) {
        return this.items.Contains(item);
      }
    }

    public bool Contains(object value) {
      return (this.Contains((T)value));
    }

    public IEnumerator<T> GetEnumerator() {
      lock(this.Lock) {
        return this.items.GetEnumerator();
      }
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return this.items.GetEnumerator();
    }

    public int IndexOf(T item) {
      lock(this.Lock) {
        return this.items.IndexOf(item);
      }
    }

    public int IndexOf(object value) {
      return this.IndexOf((T)value);
    }

    public bool Remove(T item) {
      var index = this.IndexOf(item);
      if(index < 0) {
        return false;
      }
      this.RemoveAt(index);
      return true;
    }

    public void Remove(object value) {
      this.Remove((T)value);
    }

    public void RemoveAt(int index) {
      lock(this.Lock) {
        var item = this.items[index];
        this.items.RemoveAt(index);
        this.QueueChange(NotifyCollectionChangedAction.Remove, item, index, default(T), -1);
      }
    }

    public int RemoveAll(Predicate<T> match) {
      return this.RemoveAll(0, this.Count, match);
    }

    public int RemoveAll(int index, int count, Predicate<T> match) {
      lock(this.Lock) {
        var removedItemCount = 0;
        for(var offset = index; offset < this.items.Count; offset++) {
          var item = this.items[offset];
          if(match(item)) {
            this.items.RemoveAt(offset);
            this.QueueChange(NotifyCollectionChangedAction.Remove, item, offset, default(T), -1);
            removedItemCount++;
            if(removedItemCount == count) {
              break;
            }
          }
        }
        return removedItemCount;
      }
    }

    public void RemoveRange(int index, int count) {
      if(count == 1) {
        this.RemoveAt(index);
        return;
      }
      lock(this.Lock) {
        var removedItems = this.items.GetRange(index, count);
        this.items.RemoveRange(index, count);
        this.QueueChange(NotifyCollectionChangedAction.Remove, removedItems, index, null, -1);
      }
    }

    public void RemoveRange(IEnumerable<T> collection) {
      if(!(collection is List<T> items)) {
        items = new List<T>(collection);
      }
      lock(this.Lock) {
        var index = 0;
        while(index < this.items.Count) {
          if(items.Contains(this.items[index])) {
            var item = this.items[index];
            this.items.RemoveAt(index);
            this.QueueChange(NotifyCollectionChangedAction.Remove, item, index, default(T), -1);
          } else {
            index++;
          }
        }
      }
    }

    public void Move(int oldIndex, int newIndex) {
      if(oldIndex == newIndex) {
        return;
      }
      lock(this.Lock) {
        var item = this[oldIndex];
        this.items.RemoveAt(oldIndex);
        this.items.Insert(newIndex, item);
        this.QueueChange(NotifyCollectionChangedAction.Move, item, newIndex, default(T), oldIndex);
      }
    }
  }
}