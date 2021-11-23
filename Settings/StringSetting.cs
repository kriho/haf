using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public class StringSetting: Setting<string>, IStringSetting {
    public string Mask { get; set; }
  }
}
