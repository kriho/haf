using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace HAF.Splash {
  public class SplashManager<T> where T : SplashWindow {
    private ManualResetEvent wait = new ManualResetEvent(false);
    private Thread worker;
    private T window;

    public SplashManager(string description = "", string status = "") {
      // setup splash thread
      this.worker = new Thread(() => {
        // show window
        this.window = Activator.CreateInstance<T>();
        this.window.Description = description;
        this.window.Status = status;
        this.window.Show();
        // indicate that window is open
        this.wait.Set();
        // start dispatcher
        Dispatcher.Run();
      });
      this.worker.SetApartmentState(ApartmentState.STA);
      this.worker.IsBackground = true;
#if DEBUG
      // disable splash in designe time
      if (ObservableObject.IsInDesignModeStatic) {
        return;
      }
#endif
      this.worker.Start();
    }

    public void SetStatus(string status) {
      this.wait.WaitOne();
      this.window.Dispatcher.Invoke(() => this.window.Status = status);
    }

    public void SetDescription(string description) {
      this.wait.WaitOne();
      this.window.Dispatcher.Invoke(() => this.window.Description = description);
    }

    public void SetProgress(int progress) {
      this.wait.WaitOne();
      this.window.Dispatcher.Invoke(() => this.window.Progress = progress);
    }

    public void Close() {
      this.wait.WaitOne();
      this.window.Dispatcher.Invoke(() => {
        this.window.Close();
        Dispatcher.ExitAllFrames();
      });

      this.worker.Abort();
    }
  }
}
