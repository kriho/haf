using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public static partial class Utils {
    public static T GetExportMetadata<T>(object export, string name) {
      return (T)export.GetType().GetCustomAttributes(typeof(ExportMetadataAttribute), true).OfType<ExportMetadataAttribute>().First(a => a.Name == name).Value;
    }
  }
}
