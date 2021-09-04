using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace HAF.Controls.Controls {
  // add a dependency property for a bindable document
  // http://stackoverflow.com/questions/30821339/load-rtf-in-bindable-richtexbox-mvvm-wpf

  public class BindableRichTextBox: RichTextBox {
    public static readonly DependencyProperty DocumentProperty =
        DependencyProperty.Register("Document", typeof(FlowDocument),
            typeof(BindableRichTextBox),
            new FrameworkPropertyMetadata(null) {
              BindsTwoWayByDefault = true,
              DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
              PropertyChangedCallback = new PropertyChangedCallback(OnDocumentChanged)
            }
        );

    public static readonly DependencyProperty TextChangedCommandProperty =
        DependencyProperty.Register("TextChangedCommand", typeof(RelayCommand<TextChangedEventArgs>), typeof(BindableRichTextBox));

    public new FlowDocument Document {
      get {
        return (FlowDocument)this.GetValue(DocumentProperty);
      }
      set {
        this.SetValue(DocumentProperty, value);
      }
    }

    public RelayCommand<TextChangedEventArgs> TextChangedCommand {
      get {
        return (RelayCommand<TextChangedEventArgs>)this.GetValue(TextChangedCommandProperty);
      }
      set {
        this.SetValue(TextChangedCommandProperty, value);
      }
    }

    public static void OnDocumentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
      var rtb = (RichTextBox)obj;
      if (args.NewValue is FlowDocument doc) {
        rtb.Document = doc;
      } else {
        rtb.Document = new FlowDocument();
      }
    }

    public BindableRichTextBox() {
      this.TextChanged += (sender, e) => {
        if (e.Changes.Count > 0) {
          if (this.TextChangedCommand != null) {
            this.TextChangedCommand.Execute(e);
          }
        }
      };
    }
  }
}
