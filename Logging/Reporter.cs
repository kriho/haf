using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace HAF.Logging {
  public delegate void MessageReportedEventHandler<Tmessage, Tid>(Report<Tmessage, Tid> message);

  public class Reporter<Tmessage, Tid> {
    public event MessageReportedEventHandler<Tmessage, Tid> MessageReported;

    public Func<Tid, Tmessage> GetMessageById { get; private set; }
    public Func<Tid, ReportSeverity> GetReportTypeById { get; private set; }

    public string Name { get; private set; }
    public bool HasErrors { get; private set; }

    private Tid sessionFailureId;
    private bool reportSessionFailure = false;

    public Reporter(string name, Func<Tid, Tmessage> getMessageById, Func<Tid, ReportSeverity> getReportTypeById) {
      this.Name = name;
      this.GetMessageById = getMessageById;
      this.GetReportTypeById = getReportTypeById;
    }

    public void Report(ReportSeverity severity, Tid id, Tmessage message) {
      this.Report(new Report<Tmessage, Tid>(severity, this.Name, id, message));
    }

    public void Report(Report<Tmessage, Tid> report) {
      // indicate that an error was reported
      if (report.IsError) {
        this.HasErrors = true;
        if (this.reportSessionFailure) {
          this.Report(this.sessionFailureId);
        }
      }
      // notify that a message was reported
      this.MessageReported?.Invoke(report);
    }

    public void Report(Tid id) {
      var report = new Report<Tmessage, Tid>(this.GetReportTypeById(id), this.Name, id, this.GetMessageById(id));
      this.Report(report);
    }

    public void ReportAssert(bool assert, Tid id) {
      if (assert) {
        this.Report(id);
      }
    }

    public void StartSession() {
      this.HasErrors = false;
      this.reportSessionFailure = false;
    }

    public void StartSession(Tid id) {
      this.HasErrors = false;
      this.reportSessionFailure = false;
      this.Report(id);
    }

    public void StartSession(Tid startId, Tid failureId) {
      this.HasErrors = false;
      this.reportSessionFailure = true;
      this.sessionFailureId = failureId;
      this.Report(startId);
    }
  }
}
