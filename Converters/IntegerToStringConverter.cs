using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF.Converters {
  public class IntegerToStringConverter: ValueConverter<int, string> {
    public int Base { get; set; } = 10;

    protected override string convert(int value) {
      return System.Convert.ToString(value, this.Base);
    }

    protected override int convertBack(string value) {
      return System.Convert.ToInt32(value, this.Base); 
    }
  }
}
