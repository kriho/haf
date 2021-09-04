using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HAF {
  public class LocalizeExtension: Binding {
    private static readonly ILocalizationService localizationService;

    static LocalizeExtension() {
      LocalizeExtension.localizationService = Configuration.Container.GetExportedValue<ILocalizationService>();
    }

    public LocalizeExtension(string name) : base("[" + name + "]") {
      this.Mode = BindingMode.OneWay;
      this.Source = LocalizeExtension.localizationService;
    }
  }
}
