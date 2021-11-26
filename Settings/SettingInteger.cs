using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public class SettingInteger: Setting<int>, ISettingInteger {
    public int? Min { get; set; }

    public int? Max { get; set; }

    public string Unit { get; set; }

    public static implicit operator int(SettingInteger setting) {
      return setting.Value;
    }
  }
}
