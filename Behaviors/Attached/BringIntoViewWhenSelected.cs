using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HAF {
  public partial class Behaviors2 {
    public static readonly DependencyProperty BringIntoViewWhenSelectedProperty = DependencyProperty.RegisterAttached("BringIntoViewWhenSelected", typeof(bool), typeof(Behaviors2), new UIPropertyMetadata(false, OnBringIntoViewWhenSelectedPropertyChanged));

    public static bool GetBringIntoViewWhenSelected(DependencyObject obj) {
      return (bool)obj.GetValue(BringIntoViewWhenSelectedProperty);
    }

    public static void SetBringIntoViewWhenSelected(DependencyObject obj, bool value) {
      obj.SetValue(BringIntoViewWhenSelectedProperty, value);
    }

    private static void OnBringIntoViewWhenSelectedPropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e) {
      // not a tree view item
      if (!(depObj is TreeViewItem item)) {
        return;
      }
      // wrong value
      if (e.NewValue is bool == false) {
        return;
      }
      if ((bool)e.NewValue) {
        item.Selected += item_Selected;
      } else {
        item.Selected -= item_Selected;
      }
    }

    static void item_Selected(object sender, RoutedEventArgs e) {
      if (!ReferenceEquals(sender, e.OriginalSource)) {
        return;
      }
      if (e.OriginalSource is TreeViewItem item) {
        item.BringIntoView(new Rect(0, 0, 0, 150));
      }
    }
  }
}
