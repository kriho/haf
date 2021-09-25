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

    public readonly string Id;
    public readonly string ContextId;
    public readonly string PluralId;
    public readonly int Count;

    public string Value => this.ToString();

    public LocalizedText(string id) {
      this.Id = id;
      Localization.LocalizationService.OnSelectedCultureChanged.RegisterWeak(this.NotifyValueChanged);
    }

    public LocalizedText(string contextId, string id) {
      this.ContextId = contextId;
      this.Id = id;
      Localization.LocalizationService.OnSelectedCultureChanged.RegisterWeak(this.NotifyValueChanged);
    }

    public LocalizedText(string id, string pluralId, int count) {
      this.Id = id;
      this.PluralId = pluralId;
      this.Count = count;
      Localization.LocalizationService.OnSelectedCultureChanged.RegisterWeak(this.NotifyValueChanged);
    }

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
