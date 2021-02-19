using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF.Converters {
  public class PathToDirectoryName : ValueConverter<string, string> {
    protected override string convert(string value) {
      return Path.GetDirectoryName(value);
    }
  }
}
