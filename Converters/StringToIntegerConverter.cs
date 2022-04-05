using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF.Converters {
  public class StringToIntegerConverter: ValueConverter<string, int> {
    public int Base { get; set; } = 10;

    protected override int convert(string value) {
      return System.Convert.ToInt32(value, this.Base);
    }

    protected override string convertBack(int value) {
      return System.Convert.ToString(value, this.Base);
    }
  }
}
