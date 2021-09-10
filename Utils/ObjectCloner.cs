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
    public static T Clone<T>(T source) {
      var xmlSerializer = new XmlSerializer(typeof(T));
      using(Stream stream = new MemoryStream()) {
        xmlSerializer.Serialize(stream, source);
        _ = stream.Seek(0, SeekOrigin.Begin);
        return (T)xmlSerializer.Deserialize(stream);
      }
    }
  }
}
