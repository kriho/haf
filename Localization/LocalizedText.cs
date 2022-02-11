using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  /// <summary>
  /// Provides a localized text that can be used for dynamically created objects that persist and must therefore be updated when the selected culture changes.
  /// </summary>
  public class LocalizedText: ObservableObject {
    /// <summary>
    /// Text id, usually the english translations.
    /// </summary>
    public readonly string Id;

    /// <summary>
    /// The id used to specify the context in which the localized string is used.
    /// </summary>
    public readonly string ContextId;

    /// <summary>
    /// Pluralized text id, usually the english translations.
    /// </summary>
    public readonly string PluralId;

    /// <summary>
    /// Used to determine whether the plural id must be used. The behavior is language specific.
    /// </summary>
    public readonly int Count;

    /// <summary>
    /// The translated text.
    /// </summary>
    public string Value => this.ToString();

    /// <summary>
    /// Create a localized text.
    /// </summary>
    /// <param name="id">Text id, usually the english translations.</param>
    public LocalizedText(string id) {
      this.Id = id;
      Localization.LocalizationService.OnSelectedCultureChanged.RegisterWeak(this.NotifyValueChanged);
    }

    /// <summary>
    /// Create a localized text.
    /// </summary>
    /// <param name="contextId">The id used to specify the context in which the localized string is used.</param>
    /// <param name="id">Text id, usually the english translations.</param>
    public LocalizedText(string contextId, string id) {
      this.ContextId = contextId;
      this.Id = id;
      Localization.LocalizationService.OnSelectedCultureChanged.RegisterWeak(this.NotifyValueChanged);
    }

    /// <summary>
    /// Create a localized text.
    /// </summary>
    /// <param name="id">Text id, usually the english translations.</param>
    /// <param name="pluralId">Pluralized text id, usually the english translations.</param>
    /// <param name="count">Used to determine whether the plural id must be used. The behavior is language specific.</param>
    public LocalizedText(string id, string pluralId, int count) {
      this.Id = id;
      this.PluralId = pluralId;
      this.Count = count;
      Localization.LocalizationService.OnSelectedCultureChanged.RegisterWeak(this.NotifyValueChanged);
    }

    /// <summary>
    /// Create a localized text.
    /// </summary>
    /// <param name="contextId">The id used to specify the context in which the localized string is used.</param>
    /// <param name="id">Text id, usually the english translations.</param>
    /// <param name="pluralId">Pluralized text id, usually the english translations.</param>
    /// <param name="count">Used to determine whether the plural id must be used. The behavior is language specific.</param>
    public LocalizedText(string contextId, string id, string pluralId, int count) {
      this.ContextId = contextId;
      this.Id = id;
      this.PluralId = pluralId;
      this.Count = count;
      Localization.LocalizationService.OnSelectedCultureChanged.RegisterWeak(this.NotifyValueChanged);
    }

    private void NotifyValueChanged() {
      this.NotifyPropertyChanged(() => this.Value);
    }

    public override string ToString() {
    #if DEBUG
      if(this.IsInDesignMode) {
        return $"${this.Id}$";
      }
    #endif
      return this.ContextId == null
        ? this.PluralId == null
          ? Localization.GetText(this.Id)
          : Localization.GetText(this.Id, this.PluralId, this.Count)
        : this.PluralId == null
          ? Localization.GetText(this.ContextId, this.Id)
          : Localization.GetText(this.ContextId, this.Id, this.PluralId, this.Count);
    }

    public static implicit operator string(LocalizedText text) {
      return text.ToString();
    }
  }
}
