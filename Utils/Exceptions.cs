using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HAF {
  public static partial class Utils {
    public static string GetExceptionDescription(Exception e) {
      var description = new StringBuilder();
      description.Append(e.Message);
      while(e.InnerException != null) {
        description.Append(" / " + e.InnerException.Message);
        e = e.InnerException;
      }
      return description.ToString();
    }
  }
}
