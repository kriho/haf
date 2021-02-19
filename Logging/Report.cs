using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {

  public enum ReportSeverity {
    Information,
    Warning,
    Error
  }

  public class Report<Tmessage, Tid> {
    public ReportSeverity Type { get; private set; }
    public DateTime Timestamp { get; private set; }
    public Tid ID { get; private set; }
    public Tmessage Message { get; private set; }
    public string Source { get; private set; }

    public bool IsError {
      get {
        return this.Type == ReportSeverity.Error;
      }
    }

    public Report(ReportSeverity type, string source, Tid id, Tmessage message) {
      this.Type = type;
      this.Timestamp = DateTime.Now;
      this.Source = source;
      this.Message = message;
      this.ID = id;
    }
  }
}
