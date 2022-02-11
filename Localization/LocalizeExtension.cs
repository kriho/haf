using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HAF {
  public class LocalizeExtension: Binding {
    public LocalizeExtension(string id) : base("[" + id + "]") {
      this.Mode = BindingMode.OneWay;
      this.Source = Localization.LocalizationService;
    }
  }
}
