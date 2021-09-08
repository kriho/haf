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

    private readonly string id;
    private readonly string contextId;
    private readonly string pluralId;
    private readonly int count;

    public string Value => this.ToString();

    public LocalizedText(string id) {
      this.id = id;
      this.Register();
    }

    public LocalizedText(string contextId, string id) {
      this.contextId = contextId;
      this.id = id;
      this.Register();
    }

    public LocalizedText(string id, string pluralId, int count) {
      this.id = id;
      this.pluralId = pluralId;
      this.count = count;
      this.Register();
    }

    public LocalizedText(string contextId, string id, string pluralId, int count) {
      this.contextId = contextId;
      this.id = id;
      this.pluralId = pluralId;
      this.count = count;
      this.Register();
    }

    private void Register() {
      LocalizeExtension.LocalizationService.OnSelectedCultureChanged.Register(() => {
        this.NotifyPropertyChanged(() => this.Value);
      });
    }

    public override string ToString() {
      return this.contextId == null
        ? this.pluralId == null
          ? LocalizeExtension.LocalizationService.GetText(this.id)
          : LocalizeExtension.LocalizationService.GetText(this.id, this.pluralId, this.count)
        : this.pluralId == null
          ? LocalizeExtension.LocalizationService.GetText(this.contextId, this.id)
          : LocalizeExtension.LocalizationService.GetText(this.contextId, this.id, this.pluralId, this.count);
    }

    public static implicit operator string(LocalizedText text) {
      return text.ToString();
    }
  }
}
