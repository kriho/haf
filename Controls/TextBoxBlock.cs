using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HAF.Controls {
  public class TextBoxBlock: TextBox {

    static TextBoxBlock() {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBoxBlock), new FrameworkPropertyMetadata(typeof(TextBoxBlock)));
    }
  }
}
