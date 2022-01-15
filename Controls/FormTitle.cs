using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HAF.Controls {
  public class FormTitle: Control {

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(FormTitle), new PropertyMetadata("Title"));

    public string Title {
      get => (string)GetValue(FormTitle.TitleProperty);
      set => this.SetValue(FormTitle.TitleProperty, value);
    }

    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(FormTitle), new PropertyMetadata(null));

    public string Description {
      get => (string)this.GetValue(FormTitle.DescriptionProperty); 
      set => this.SetValue(FormTitle.DescriptionProperty, value); 
    }

    static FormTitle() {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(FormTitle), new FrameworkPropertyMetadata(typeof(FormTitle)));

    }
  }
}
