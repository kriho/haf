using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HAF.Converters {
  public class StringDefaultConverter: ValueConverter<string, string> {
    public string Default { get; set; } = string.Empty;

    protected override string convert(string value) {
     return string.IsNullOrWhiteSpace(value) ? this.Default : value;
    }
  }
}
