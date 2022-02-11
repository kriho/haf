using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public interface ILocalizationProvider {
    /// <summary>
    /// Get localized text from id.
    /// </summary>
    /// <param name="id">Text id, usually the english translations.</param>
    /// <returns>The localized text.</returns>
    string GetText(string id);

    /// <summary>
    /// Get localized text from id within context.
    /// </summary>
    /// <param name="contextId">The id used to specify the context in which the localized string is used.</param>
    /// <param name="id">Text id, usually the english translations.</param>
    /// <returns>The localized text.</returns>
    string GetText(string contextId, string id);


    /// <summary>
    /// Get localized text from id within context.
    /// </summary>
    /// <param name="id">Text id, usually the english translations.</param>
    /// <param name="pluralId">Pluralized text id, usually the english translations.</param>
    /// <param name="count">Used to determine whether the plural id must be used. The behavior is language specific.</param>
    /// <returns>The localized text.</returns>
    string GetText(string id, string pluralId, int count);

    /// <summary>
    /// Get localized text from id within context
    /// </summary>
    /// <param name="contextId">The id used to specify the context in which the localized string is used.</param>
    /// <param name="id">Text id, usually the english translations.</param>
    /// <param name="pluralId">Pluralized text id, usually the english translations.</param>
    /// <param name="count">Used to determine whether the plural id must be used. The behavior is language specific.</param>
    /// <returns>The localized text.</returns>
    string GetText(string contextId, string id, string pluralId, int count);

    /// <summary>
    /// Load localized texts from the source of the localization provider.
    /// </summary>
    void Load();

    /// <summary>
    /// Unload all localized texts.
    /// </summary>
    void Unload();
  }
}
