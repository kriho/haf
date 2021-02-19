using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HAF.Updates {
  public class SnapshotFile {
    [XmlElement("path")]
    public string RemotePath { get; set; }

    [XmlAttribute("checksum")]
    public string MD5 { get; set; }

    [XmlAttribute("size")]
    public long Size { get; set; }

    public string LocalPath {
      get {
        return this.RemotePath.Replace('/', System.IO.Path.DirectorySeparatorChar);
      }
    }
  }
}
