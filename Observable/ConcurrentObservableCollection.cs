using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace HAF {

  [DebuggerDisplay("Count = {Count}")]
  [Serializable]
  public class ConcurrentObservableCollection<T>: ObservableObject, IReadOnlyObservableCollection<T> {
    private class CollectionChange {
      public NotifyCollectionChangedAction Action;
      public int NewIndex;
      public List<T> NewItems;
      public int OldIndex;
      public List<T> OldItems;
    }

    private readonly List<T> items;

    private List<CollectionChange> changes = new List<CollectionChange>();

    private bool isInvoked = false;

    public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

    private object @lock = new object();

    private Dispatcher dispatcher;

    public ConcurrentObservableCollection() {
      this.items = new List<T>();
      this.dispatcher = System.Windows.Application.Current.Dispatcher;
    }

    public ConcurrentObservableCollection(IEnumerable<T> collection) {
      this.items = new List<T>(collection);
      this.dispatcher = System.Windows.Application.Current.Dispatcher;
    }

    private void NotifyChanges() {
      if(this.CollectionChanged != null) {
        lock(this.@lock) {
          foreach(var change in this.changes) {
            switch(change.Action) {
              case NotifyCollectionChangedAction.Reset:
                this.CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                break;
              case NotifyCollectionChangedAction.Add:
              case NotifyCollectionChangedAction.Remove:
                if(change.NewItems.Count == 1) {
                  this.CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(change.Action, change.NewItems[0], change.NewIndex));
                } else {
                  this.CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(change.Action, change.NewItems, change.NewIndex));
                }
                break;
              case NotifyCollectionChangedAction.Replace:
                if(change.NewItems.Count == 1) {
                  this.CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(change.Action, change.NewItems[0], change.OldItems[0], change.NewIndex));
                } else {
                  this.CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(change.Action, change.NewItems, change.OldItems, change.NewIndex));
                }
                break;
              case NotifyCollectionChangedAction.Move:
                if(change.NewItems.Count == 1) {
                  this.CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(change.Action, change.NewItems[0], change.NewIndex, change.OldIndex));
                } else {
                  this.CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(change.Action, change.NewItems, change.NewIndex, change.OldIndex));
                }
                break;
            }
          }
          this.NotifyPropertyChanged("Count");
          this.NotifyPropertyChanged("Item[]");
          this.changes.Clear();
        }
      }
      this.isInvoked = false;
    }

    private void QueueChangeAdd(T item, int index) {
      var items = new List<T>();
      items.Add(item);
      this.QueueChange(NotifyCollectionChangedAction.Add, items, index, null, 0);
    }

    private void QueueChangeAdd(List<T> items, int index) {
      this.QueueChange(NotifyCollectionChangedAction.Add, items, index, null, 0);
    }

    private void QueueChangeRemove(T item, int index) {
      var items = new List<T>();
      items.Add(item);
      this.QueueChange(NotifyCollectionChangedAction.Remove, items, index, null, 0);
    }

    private void QueueChangeRemove(List<T> items, int index) {
      this.QueueChange(NotifyCollectionChangedAction.Remove, items, index, null, 0);
    }

    private void QueueChangeReplace(T oldItem, T newItem, int index) {
      var oldItems = new List<T>();
      oldItems.Add(oldItem);
      var newItems = new List<T>();
      newItems.Add(newItem);
      this.QueueChange(NotifyCollectionChangedAction.Replace, oldItems, index, newItems, 0);
    }

    private void QueueChangeReplace(List<T> oldItems, List<T> newItems, int index) {
      this.QueueChange(NotifyCollectionChangedAction.Replace, oldItems, index, newItems, 0);
    }

    private void QueueChangeMove(T item, int newIndex, int oldIndex) {
      var items = new List<T>();
      items.Add(item);
      this.QueueChange(NotifyCollectionChangedAction.Move, items, newIndex, null, oldIndex);
    }

    private void QueueChangeMove(List<T> items, int newIndex, int oldIndex) {
      this.QueueChange(NotifyCollectionChangedAction.Move, items, newIndex, null, oldIndex);
    }

    private void QueueChangeReset() {
      this.QueueChange(NotifyCollectionChangedAction.Reset, null, 0, null, 0);
    }

    private void QueueChange(NotifyCollectionChangedAction action, List<T> newItems, int newIndex, List<T> oldItems, int oldIndex) {
      this.UpdateChanges(action, newItems, newIndex, oldItems, oldIndex);
      if(!this.isInvoked) {
        this.isInvoked = true;
        _ = this.dispatcher.BeginInvoke(new Action(this.NotifyChanges), DispatcherPriority.Render);
      }
    }

    private void UpdateChanges(NotifyCollectionChangedAction action, List<T> newItems, int newIndex, List<T> oldItems, int oldIndex) {
      if(action == NotifyCollectionChangedAction.Reset) {
        this.changes.Clear();
        this.changes.Add(new CollectionChange {
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
        this.changes.Add(new CollectionChange {
          Action = action,
          NewIndex = newIndex,
          NewItems = newItems,
          OldIndex = oldIndex,
          OldItems = oldItems,
        });
      }
    }

    public int Count {
      get {
        lock(this.@lock) {
          return this.items.Count;
        }
      }
    }

    public T this[int index] {
      get {
        lock(this.@lock) {
          return this.items[index];
        }
      }
      set {
        lock(this.@lock) {
          var oldItem = this[index];
          this.items[index] = value;
          this.QueueChangeReplace(value, oldItem, index);
        }
      }
    }

    public void Add(T item) {
      lock(this.@lock) {
        var index = this.items.Count;
        this.items.Add(item);
        this.QueueChangeAdd(item, index);
      }
    }

    public void AddRange(IEnumerable<T> collection) {
      if(!(collection is List<T> items)) {
        items = new List<T>(collection);
      }
      lock(this.@lock) {
        var index = this.items.Count;
        this.items.AddRange(items);
        this.QueueChangeAdd(items, index);
      }
    }

    public void Insert(int index, T item) {
      lock(this.@lock) {
        this.items.Insert(index, item);
        this.QueueChangeAdd(item, index);
      }
    }

    public void InserRange(int index, IEnumerable<T> collection) {
      if(!(collection is List<T> items)) {
        items = new List<T>(collection);
      }
      lock(this.@lock) {
        this.items.InsertRange(index, items);
        this.QueueChangeAdd(items, index);
      }
    }

    public void Clear() {
      lock(this.@lock) {
        this.items.Clear();
        this.QueueChangeReset();
      }
    }

    public void CopyTo(T[] array, int index) {
      lock(this.@lock) {
        this.items.CopyTo(array, index);
      }
    }

    public bool Contains(T item) {
      lock(this.@lock) {
        return this.items.Contains(item);
      }
    }

    public IEnumerator<T> GetEnumerator() {
      lock(this.@lock) {
        return this.items.GetEnumerator();
      }
    }

    IEnumerator IEnumerable.GetEnumerator() {
      lock(this.@lock) {
        return this.items.GetEnumerator();
      }
    }

    public int IndexOf(T item) {
      lock(this.@lock) {
        return this.items.IndexOf(item);
      }
    }

    public bool Remove(T item) {
      var index = this.IndexOf(item);
      if(index < 0) {
        return false;
      }
      this.RemoveAt(index);
      return true;
    }

    public void RemoveAt(int index) {
      lock(this.@lock) {
        var item = this[index];
        this.items.RemoveAt(index);
        this.QueueChangeRemove(item, index);
      }
    }

    public int RemoveAll(Predicate<T> match) {
      return this.RemoveAll(0, this.Count, match);
    }

    public int RemoveAll(int index, int count, Predicate<T> match) {
      lock(this.@lock) {
        var removedItemCount = 0;
        for(var offset = index; offset < this.items.Count; offset++) {
          var item = this.items[offset];
          if(match(item)) {
            this.items.RemoveAt(offset);
            this.QueueChangeRemove(item, offset);
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
      lock(this.@lock) {
        var removedItems = this.items.GetRange(index, count);
        this.items.RemoveRange(index, count);
        this.QueueChangeRemove(removedItems, index);
      }
    }

    public void RemoveRange(IEnumerable<T> collection) {
      if(!(collection is List<T> items)) {
        items = new List<T>(collection);
      }
      lock(this.@lock) {
        this.items.RemoveAll(i => items.Contains(i));
        this.QueueChangeRemove(items, -1);
      }
    }

    public void Move(int oldIndex, int newIndex) {
      lock(this.@lock) {
        var item = this[oldIndex];
        this.items.RemoveAt(oldIndex);
        this.items.Insert(newIndex, item);
        this.QueueChangeMove(item, newIndex, oldIndex);
      }
    }
  }
}