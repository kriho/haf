using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HAF.Controls {
  public class Form: ItemsControl {

    static Form() {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(Form), new FrameworkPropertyMetadata(typeof(Form)));
    }

    public ScrollBarVisibility HorizontalScrollBarVisibility {
      get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); }
      set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
    }

    public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty = DependencyProperty.Register("HorizontalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(Form), new PropertyMetadata(ScrollBarVisibility.Disabled));

    public ScrollBarVisibility VerticalScrollBarVisibility {
      get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
      set { SetValue(VerticalScrollBarVisibilityProperty, value); }
    }

    public static readonly DependencyProperty VerticalScrollBarVisibilityProperty = DependencyProperty.Register("VerticalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(Form), new PropertyMetadata(ScrollBarVisibility.Visible));
  }
}
