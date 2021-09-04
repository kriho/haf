using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public interface ILocalizationProvider {
    string ProvideString(string english);
    void Load();
    void Unload();
  }
}
