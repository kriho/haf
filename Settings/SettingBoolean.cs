using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public class SettingBoolean: Setting<bool>, ISettingBoolean {
    public static implicit operator bool(SettingBoolean setting) {
      return setting.Value;
    }
  }
}
