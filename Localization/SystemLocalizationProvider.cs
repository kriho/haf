using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public class SystemLocalizationProvider: ILocalizationProvider {
    public string GetText(string id) {
      return id;
    }

    public string GetText(string contextId, string id) {
      return id;
    }

    public string GetText(string contextId, string id, string pluralId, int count) {
      return count == 1 ? id : pluralId;
    }

    public string GetText(string id, string pluralId, int count) {
      return count == 1 ? id : pluralId;
    }

    public void Load() {
    }

    public void Unload() {
    }
  }
}
