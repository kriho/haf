using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HAF.Logging {
  public delegate void MessageLoggedEventHandler<Tmessage, Tid>(Report<Tmessage, Tid> message);

  public class Logger<Tmessage, Tid>: ObservableObject {
    public event MessageLoggedEventHandler<Tmessage, Tid> MessageLogged;
    public ObservableCollection<Report<Tmessage, Tid>> reports = new ObservableCollection<Report<Tmessage, Tid>>();

    public Logger() {
    }

    public void SubscribeReporter(Reporter<Tmessage, Tid> reporter) {
      reporter.MessageReported += Reporter_MessageReported;
    }

    public void UnsubscribeReporter(Reporter<Tmessage, Tid> reporter) {
      reporter.MessageReported -= Reporter_MessageReported;
    }

    private void Reporter_MessageReported(Report<Tmessage, Tid> message) {
      Application.Current.Dispatcher.BeginInvoke(new Action(() => {
        this.reports.Add(message);
        this.MessageLogged?.Invoke(message);
      }));
    }
  }
}
