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

    /// <summary>
    /// Get localized text from id.
    /// </summary>
    /// <param name="id">Text id, usually the english translations.</param>
    /// <returns>The localized text.</returns>
    public static string GetText(string id) {
      return Localization.LocalizationService.GetText(id);
    }

    /// <summary>
    /// Get localized text from id within context.
    /// </summary>
    /// <param name="contextId">The id used to specify the context in which the localized string is used.</param>
    /// <param name="id">Text id, usually the english translations.</param>
    /// <returns>The localized text.</returns>
    public static string GetText(string contextId, string id) {
      return Localization.LocalizationService.GetText(contextId, id);
    }

    /// <summary>
    /// Get localized text from id within context.
    /// </summary>
    /// <param name="id">Text id, usually the english translations.</param>
    /// <param name="pluralId">Pluralized text id, usually the english translations.</param>
    /// <param name="count">Used to determine whether the plural id must be used. The behavior is language specific.</param>
    /// <returns>The localized text.</returns>
    public static string GetText(string id, string pluralId, int count) {
      return Localization.LocalizationService.GetText(id, pluralId, count);
    }

    /// <summary>
    /// Get localized text from id within context
    /// </summary>
    /// <param name="contextId">The id used to specify the context in which the localized string is used.</param>
    /// <param name="id">Text id, usually the english translations.</param>
    /// <param name="pluralId">Pluralized text id, usually the english translations.</param>
    /// <param name="count">Used to determine whether the plural id must be used. The behavior is language specific.</param>
    /// <returns>The localized text.</returns>
    public static string GetText(string contextId, string id, string pluralId, int count) {
      return Localization.LocalizationService.GetText(contextId, id, pluralId, count);
    }
  }
}
