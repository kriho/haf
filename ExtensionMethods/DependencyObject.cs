using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HAF {
  public static class DependencyObjectExtensions {
    public static T GetChildOfType<T>(this DependencyObject depObj) where T : DependencyObject {
      if(depObj == null) {
        return null;
      }
      if(depObj is FrameworkElement element) {
        element.ApplyTemplate();
      }
      for(int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++) {
        var child = VisualTreeHelper.GetChild(depObj, i);
        var result = (child as T) ?? GetChildOfType<T>(child);
        if(result != null) {
          return result;
        }
      }
      return null;
    }
  }
}
