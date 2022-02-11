using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace HAF.Controls {
  public class FastBindableTextBlock: TextBlock {
    public IEnumerable<Inline> BindableInlines {
      get { return (IEnumerable<Inline>)GetValue(BindableInlinesProperty); }
      set { SetValue(BindableInlinesProperty, value); }
    }

    public static readonly DependencyProperty BindableInlinesProperty = DependencyProperty.Register("BindableInlines", typeof(IEnumerable<Inline>), typeof(FastBindableTextBlock), new UIPropertyMetadata(null, OnBindableInlinesChanged));

    private static void OnBindableInlinesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
      if(sender is FastBindableTextBlock bindableTextBlock) {
        if(e.NewValue is IEnumerable<Inline> inlines) {
          bindableTextBlock.Inlines.Clear();
          bindableTextBlock.Inlines.AddRange(inlines);
        }
      }
    }
  }
}
