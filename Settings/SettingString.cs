using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public class SettingString: Setting<string>, ISettingString {
    public string Mask { get; set; }

    public static implicit operator string(SettingString setting) {
      return setting.Value;
    }
  }
}
