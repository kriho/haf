using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public class StringTruncationConverter: ValueConverter<string, string> {
    public int Length { get; set; } = 50;
    public string Indicator { get; set; } = "...";

    protected override string convert(string value) {
      return value.Length <= this.Length ? value : value.Substring(0, this.Length) + this.Indicator;
    }
  }
}
