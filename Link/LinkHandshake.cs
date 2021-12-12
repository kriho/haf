using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  [DataContract]
  public class LinkHandshake {
    [DataMember]
    public int ProtocolVersion { get; set; }
    [DataMember]
    public string ReportedName { get; set; }
  }
}
