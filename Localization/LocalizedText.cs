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
      this.Register();
    }

    public LocalizedText(string contextId, string id) {
      this.ContextId = contextId;
      this.Id = id;
      this.Register();
    }

    public LocalizedText(string id, string pluralId, int count) {
      this.Id = id;
      this.PluralId = pluralId;
      this.Count = count;
      this.Register();
    }

    public LocalizedText(string contextId, string id, string pluralId, int count) {
      this.ContextId = contextId;
      this.Id = id;
      this.PluralId = pluralId;
      this.Count = count;
      this.Register();
    }

    private void Register() {
      LocalizeExtension.LocalizationService.OnSelectedCultureChanged.Register(() => {
        this.NotifyPropertyChanged(() => this.Value);
      });
    }

    public override string ToString() {
      return this.ContextId == null
        ? this.PluralId == null
          ? LocalizeExtension.LocalizationService.GetText(this.Id)
          : LocalizeExtension.LocalizationService.GetText(this.Id, this.PluralId, this.Count)
        : this.PluralId == null
          ? LocalizeExtension.LocalizationService.GetText(this.ContextId, this.Id)
          : LocalizeExtension.LocalizationService.GetText(this.ContextId, this.Id, this.PluralId, this.Count);
    }

    public static implicit operator string(LocalizedText text) {
      return text.ToString();
    }
  }
}
