using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace HAF {
  public partial class Behavior {
    public static readonly DependencyProperty HighlightTextProperty = DependencyProperty.RegisterAttached("HighlightText", typeof(string), typeof(Behavior), new UIPropertyMetadata(null, OnHighlightTextPropertyChanged));

    public static string GetHighlightText(DependencyObject obj) {
      return (string)obj.GetValue(HighlightTextProperty);
    }
    public static void SetHighlightText(DependencyObject obj, string value) {
      obj.SetValue(HighlightTextProperty, value);
    }

    private static void OnHighlightTextPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
      if(obj is TextBlock textBlock && !string.IsNullOrEmpty(textBlock.Text)) {
        if(e.NewValue is string highlightText && !string.IsNullOrEmpty(highlightText)) {
          string text = textBlock.Text;
          var index = text.IndexOf(highlightText, StringComparison.CurrentCultureIgnoreCase);
          if(index < 0) {
            // no match
            return;
          }
          var highlightBrush = (Brush)obj.GetValue(HighlightBrushProperty);
          textBlock.Inlines.Clear();
          while(true) {
            textBlock.Inlines.AddRange(new Inline[] {
                 new Run(text.Substring(0, index)),
                 new Run(text.Substring(index, highlightText.Length)) {
                     Foreground = highlightBrush
                 }
            });
            text = text.Substring(index + highlightText.Length);
            index = text.IndexOf(highlightText, StringComparison.CurrentCultureIgnoreCase);
            if(index < 0) {
              textBlock.Inlines.Add(new Run(text));
              break;
            }
          }
        }
      }
    }

    /*
    private static void OnHighlightTextPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
      if(obj is TextBlock textBlock && (!string.IsNullOrEmpty(textBlock.Text) || textBlock.Inlines.Count > 0)) {
        if(e.NewValue is string highlightText && !string.IsNullOrEmpty(highlightText)) {
          var highlightBrush = (Brush)obj.GetValue(HighlightBrushProperty);
          for(var inlineIndex = 0; inlineIndex < textBlock.Inlines.Count; inlineIndex++) {
            if(textBlock.Inlines.ElementAt(inlineIndex) is Run run) {
              var runSpan = 1;
              string text = run.Text;
              while(textBlock.Inlines.ElementAt(inlineIndex + runSpan) is Run subsequentRun) {
                runSpan++;
                text += subsequentRun.Text;
              }
              var index = text.IndexOf(highlightText, StringComparison.CurrentCultureIgnoreCase);
              if(index < 0) {
                // no match in run
                continue;
              }
              while(true) {
                textBlock.Inlines.InsertBefore(run, new Run(text.Substring(0, index)));
                textBlock.Inlines.InsertBefore(run, new Run(text.Substring(index, highlightText.Length)) {
                  Foreground = highlightBrush
                });
                inlineIndex += 2;
                index = text.IndexOf(highlightText, StringComparison.CurrentCultureIgnoreCase);
                if(index < 0) {
                  break;
                }
              }
              textBlock.Inlines.InsertBefore(run, new Run(text));
              while(runSpan > 0) {
                runSpan--;
                textBlock.Inlines.Remove(textBlock.Inlines.ElementAt(inlineIndex)); 
              }
            }
          }
        }
      }
    }*/
  }
}
