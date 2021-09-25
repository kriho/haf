using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public static class Localization {
    internal static readonly ILocalizationService LocalizationService;

    static Localization() {
      Localization.LocalizationService = Configuration.Container.GetExportedValue<ILocalizationService>();
    }

    public static string GetText(string id) {
      return Localization.LocalizationService.GetText(id);
    }

    public static string GetText(string contextId, string id) {
      return Localization.LocalizationService.GetText(contextId, id);
    }

    public static string GetText(string id, string pluralId, int count) {
      return Localization.LocalizationService.GetText(id, pluralId, count);
    }

    public static string GetText(string contextId, string id, string pluralId, int count) {
      return Localization.LocalizationService.GetText(contextId, id, pluralId, count);
    }
  }
}
