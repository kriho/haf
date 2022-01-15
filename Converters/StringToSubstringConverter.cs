using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF.Converters {
  public class StringToSubstringConverter: ValueConverter<string, string> {
    public int Index { get; set; }

    public int Length { get; set; }

    protected override string convert(string value) {
      if(value == null) {
        return null;
      }
      return value.Substring(this.Index, this.Length);
    }
  }
}
