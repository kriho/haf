using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public interface ILocalizationProvider {
    string GetText(string id);
    string GetText(string contextId, string id);
    string GetText(string id, string pluralId, int count);
    string GetText(string contextId, string id, string pluralId, int count);
    void Load();
    void Unload();
  }
}
