using System.Globalization;

namespace HAF {
  public interface ILocalizationService: IService {
    string this[string compoundId] { get; }

    /// <summary>
    /// The available cultures with registered localization providers.
    /// </summary>
    IReadOnlyObservableCollection<CultureInfo> AvailableCultures { get; }

    /// <summary>
    /// The selected culture.
    /// </summary>
    CultureInfo SelectedCulture { get; set; }

    /// <summary>
    /// Register a localization provider for the provided culture.
    /// </summary>
    void RegisterProvider(CultureInfo culture, ILocalizationProvider provider);

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
    /// Event that is fired when the selected culture changes.
    /// </summary>
    IReadOnlyEvent OnSelectedCultureChanged { get; }
  }
}