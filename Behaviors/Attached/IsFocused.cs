﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HAF {
  public partial class Behavior {
    public static readonly DependencyProperty IsFocusedProperty = DependencyProperty.RegisterAttached("IsFocused", typeof(bool), typeof(Behavior), new UIPropertyMetadata(false, OnIsFocusedPropertyChanged));

    public static bool GetIsFocused(DependencyObject obj) {
      return (bool)obj.GetValue(IsFocusedProperty);
    }

    public static void SetIsFocused(DependencyObject obj, bool value) {
      obj.SetValue(IsFocusedProperty, value);
    }

    private static void OnIsFocusedPropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e) {
      var element = (UIElement)depObj;
      if ((bool)e.NewValue) {
        element.Focus();
      }
    }
  }
}
