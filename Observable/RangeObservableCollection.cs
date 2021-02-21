
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace HAF {
  public class RangeObservableCollection<T> : ObservableCollection<T> {

    [NonSerialized]
    private DeferredEventsCollection deferredEvents;

    public RangeObservableCollection() { }

    public RangeObservableCollection(IEnumerable<T> collection) : base(collection) { }

    public RangeObservableCollection(List<T> list) : base(list) { }

    public bool AllowDuplicates { get; set; } = true;

    public void AddRange(IEnumerable<T> collection) {
      this.InsertRange(this.Count, collection);
    }

    public void InsertRange(int index, IEnumerable<T> collection) {
      if (collection == null) {
        throw new ArgumentNullException(nameof(collection));
      }
      if (index < 0) {
        throw new ArgumentOutOfRangeException(nameof(index));
      }
      if (index > Count) {
        throw new ArgumentOutOfRangeException(nameof(index));
      }
      if (!this.AllowDuplicates) {
        collection = collection.Distinct().Where(item => !Items.Contains(item)).ToList();
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
      this.OnEssentialPropertiesChanged();
      if (!(collection is IList list)) {
        list = new List<T>(collection);
      }
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list, index));
    }

    public void RemoveRange(IEnumerable<T> collection) {
      if (collection == null) {
        throw new ArgumentNullException(nameof(collection));
      }
      if (Count == 0) {
        return;
      } else if (collection is ICollection<T> countable) {
        if (countable.Count == 0) {
          return;
        } else if (countable.Count == 1) {
          using (var enumerator = countable.GetEnumerator()) {
            enumerator.MoveNext();
            Remove(enumerator.Current);
            return;
          }
        }
      } else if (!collection.Any()) {
        return;
      }
      this.CheckReentrancy();
      var clusters = new Dictionary<int, List<T>>();
      var lastIndex = -1;
      List<T> lastCluster = null;
      foreach (var item in collection) {
        var index = IndexOf(item);
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
      this.OnEssentialPropertiesChanged();
      if (this.Count == 0) {
        OnCollectionReset();
      } else {
        foreach (KeyValuePair<int, List<T>> cluster in clusters) {
          this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, cluster.Value, cluster.Key));
        }
      }
    }

    public int RemoveAll(Predicate<T> match) {
      return RemoveAll(0, Count, match);
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
      if (Count == 0) {
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
        this.OnEssentialPropertiesChanged();
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
      this.OnEssentialPropertiesChanged();
      if (Count == 0) {
        this.OnCollectionReset();
      } else {
        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItems, index));
      }
    }

    public void ReplaceRange(IEnumerable<T> collection) {
      this.ReplaceRange(0, Count, collection);
    }

    public void ReplaceRange(int index, int count, IEnumerable<T> collection) {
      if (index < 0) {
        throw new ArgumentOutOfRangeException(nameof(index));
      }
      if (count < 0) {
        throw new ArgumentOutOfRangeException(nameof(count));
      }
      if (index + count > Count) {
        throw new ArgumentOutOfRangeException(nameof(index));
      }
      if (collection == null) {
        throw new ArgumentNullException(nameof(collection));
      }
      if (!AllowDuplicates) {
        collection = collection.Distinct().ToList();
      }
      if (collection is ICollection<T> countable) {
        if (countable.Count == 0) {
          this.RemoveRange(index, count);
          return;
        }
      } else if (!collection.Any()) {
        this.RemoveRange(index, count);
        return;
      }
      if (index + count == 0) {
        this.InsertRange(0, collection);
        return;
      }
      if (!(collection is IList<T> list)) {
        list = new List<T>(collection);
      }
      using (this.BlockReentrancy()) {
        using (this.DeferEvents()) {
          var rangeCount = index + count;
          var addedCount = list.Count;
          var changesMade = false;
          List<T> newCluster = null, oldCluster = null;
          var i = index;
          for (; i < rangeCount && i - index < addedCount; i++) {
            //parallel position
            T old = this[i], @new = list[i - index];
            if (Comparer.Equals(old, @new)) {
              this.OnRangeReplaced(i, newCluster, oldCluster);
              continue;
            } else {
              this.Items[i] = @new;
              if (newCluster == null) {
                Debug.Assert(oldCluster == null);
                newCluster = new List<T> { @new };
                oldCluster = new List<T> { old };
              } else {
                newCluster.Add(@new);
                oldCluster.Add(old);
              }
              changesMade = true;
            }
          }
          this.OnRangeReplaced(i, newCluster, oldCluster);
          //exceeding position
          if (count != addedCount) {
            var items = (List<T>)Items;
            if (count > addedCount) {
              var removedCount = rangeCount - addedCount;
              var removed = new T[removedCount];
              items.CopyTo(i, removed, 0, removed.Length);
              items.RemoveRange(i, removedCount);
              this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed, i));
            } else {
              var k = i - index;
              var added = new T[addedCount - k];
              for (int j = k; j < addedCount; j++) {
                var @new = list[j];
                added[j - k] = @new;
              }
              items.InsertRange(i, added);
              this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, added, i));
            }
            this.OnEssentialPropertiesChanged();
          } else if (changesMade) {
            this.OnIndexerPropertyChanged();
          }
        }
      }
    }

    protected override void ClearItems() {
      if (this.Count == 0) {
        return;
      }
      this.CheckReentrancy();
      base.ClearItems();
      this.OnEssentialPropertiesChanged();
      this.OnCollectionReset();
    }

    protected override void InsertItem(int index, T item) {
      if (!this.AllowDuplicates && this.Items.Contains(item)) {
        return;
      }
      base.InsertItem(index, item);
    }

    protected override void SetItem(int index, T item) {
      if (this.AllowDuplicates) {
        if (this[index].Equals(item)) {
          return;
        }
      } else {
        if (this.Items.Contains(item)) {
          return;
        }
      }
      this.CheckReentrancy();
      var oldItem = this[index];
      base.SetItem(index, item);
      this.OnIndexerPropertyChanged();
      this.OnCollectionChanged(NotifyCollectionChangedAction.Replace, oldItem, item, index);
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
      if (this.deferredEvents != null) {
        this.deferredEvents.Add(e);
        return;
      }
      base.OnCollectionChanged(e);
    }

    protected virtual IDisposable DeferEvents() => new DeferredEventsCollection(this);

    private void OnEssentialPropertiesChanged() {
      this.OnPropertyChanged(EventArgsCache.CountPropertyChanged);
      this.OnIndexerPropertyChanged();
    }

    private void OnIndexerPropertyChanged() =>
     OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);

    private void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int index) =>
     OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));

    private void OnCollectionReset() =>
     OnCollectionChanged(EventArgsCache.ResetCollectionChanged);

    private void OnRangeReplaced(int followingItemIndex, ICollection<T> newCluster, ICollection<T> oldCluster) {
      if (oldCluster == null || oldCluster.Count == 0) {
        Debug.Assert(newCluster == null || newCluster.Count == 0);
        return;
      }
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, new List<T>(newCluster), new List<T>(oldCluster), followingItemIndex - oldCluster.Count));
      oldCluster.Clear();
      newCluster.Clear();
    }

    sealed class DeferredEventsCollection : List<NotifyCollectionChangedEventArgs>, IDisposable {

      readonly RangeObservableCollection<T> collection;

      public DeferredEventsCollection(RangeObservableCollection<T> collection) {
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

  internal static class EventArgsCache {
    internal static readonly PropertyChangedEventArgs CountPropertyChanged = new PropertyChangedEventArgs("Count");
    internal static readonly PropertyChangedEventArgs IndexerPropertyChanged = new PropertyChangedEventArgs("Item[]");
    internal static readonly NotifyCollectionChangedEventArgs ResetCollectionChanged = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
  }
}
