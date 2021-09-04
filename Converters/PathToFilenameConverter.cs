using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF.Converters {
  public class PathToFilenameConverter: ValueConverter<string, string> {
    public bool TrimExtension { get; set; } = true;

    protected override string convert(string value) {
      if (this.TrimExtension) {
        return Path.GetFileNameWithoutExtension(value);
      } else {
        return Path.GetFileName(value);
      }

    }
  }
}
