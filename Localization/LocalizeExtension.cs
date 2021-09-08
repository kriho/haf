using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HAF {
  public class LocalizeExtension: Binding {
    internal static readonly ILocalizationService LocalizationService;

    static LocalizeExtension() {
      LocalizeExtension.LocalizationService = Configuration.Container.GetExportedValue<ILocalizationService>();
    }

    public LocalizeExtension(string id) : base("[" + id + "]") {
      this.Mode = BindingMode.OneWay;
      this.Source = LocalizeExtension.LocalizationService;
    }
  }
}
