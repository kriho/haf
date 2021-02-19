using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;

namespace HAF {

  public class SelectedItemsBehavior : Behavior<RadComboBox> {

    private RadComboBox ComboBox {
      get {
        return AssociatedObject as RadComboBox;
      }
    }

    public INotifyCollectionChanged SelectedItems {
      get { return (INotifyCollectionChanged)GetValue(SelectedItemsProperty); }
      set { SetValue(SelectedItemsProperty, value); }
    }

    // Using a DependencyProperty as the backing store for SelectedItemsProperty.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SelectedItemsProperty =
        DependencyProperty.Register("SelectedItems", typeof(INotifyCollectionChanged), typeof(SelectedItemsBehavior), new PropertyMetadata(OnSelectedItemsPropertyChanged));


    private static void OnSelectedItemsPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs args) {
      var collection = args.NewValue as INotifyCollectionChanged;
      if (collection != null) {
        ((SelectedItemsBehavior)target).UpdateTransfer(args.NewValue);
        collection.CollectionChanged += ((SelectedItemsBehavior)target).ContextSelectedItems_CollectionChanged;
      }
    }

    private void UpdateTransfer(object items) {
      if (this.ComboBox == null) {
        return;
      }
      Transfer(items as IList, ComboBox.SelectedItems);
      ComboBox.SelectionChanged += ComboSelectionChanged;
    }

    protected override void OnAttached() {
      base.OnAttached();
    }

    void ContextSelectedItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
      UnsubscribeFromEvents();
      Transfer(SelectedItems as IList, ComboBox.SelectedItems);
      SubscribeToEvents();
    }

    private void ComboSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
      UnsubscribeFromEvents();
      if (e.AddedItems.Count == 0 && e.RemovedItems.Count == 1) {
        (SelectedItems as IList).Remove(e.RemovedItems[0]);
      } else {
        Transfer(ComboBox.SelectedItems, SelectedItems as IList);

      }
      SubscribeToEvents();
    }

    private void SubscribeToEvents() {
      ComboBox.SelectionChanged += ComboSelectionChanged;
      if (SelectedItems != null) {
        SelectedItems.CollectionChanged += ContextSelectedItems_CollectionChanged;
      }
    }

    private void UnsubscribeFromEvents() {
      ComboBox.SelectionChanged -= ComboSelectionChanged;
      if (SelectedItems != null) {
        SelectedItems.CollectionChanged -= ContextSelectedItems_CollectionChanged;
      }
    }

    public static void Transfer(IList source, IList target) {
      if (source == null || target == null)
        return;
      if (source.Count == 0) {
        foreach (var o in target) {
          source.Add(o);
        }
      } else {
        target.Clear();
        foreach (var o in source) {
          target.Add(o);
        }
      }
    }
  }
}