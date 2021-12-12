using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public abstract class LinkService<T, S>: Service, ILinkService<T, S> where T : ClientBase<S>, IRequestHandshake where S : class {
    private ILogService logService;

    private readonly string applicationFamily;

    private readonly string serverEndpoint;

    private readonly string serverName;

    private readonly int protocolVersion;

    private ServiceHost serviceHost;

    public IRelayCommand DoConnect { get; private set; }

    private State isConnected = new State(false);
    public IReadOnlyState IsConnected => this.isConnected;

    private PredicateState isServerInstalled;
    public IReadOnlyState IsServerInstalled => this.isServerInstalled;

    private string serverFilePath;
    public string ServerFilePath {
      get => this.serverFilePath;
      private set {
        if(this.SetValue(ref this.serverFilePath, value)) {
          this.isServerInstalled.UpdateValue();
        }
      }
    }

    private string reportedServerName;
    public string ReportedServerName {
      get => this.reportedServerName;
      private set {
        if(this.SetValue(ref this.reportedServerName, value)) {
          this.isServerInstalled.UpdateValue();
        }
      }
    }

    private T proxy;
    public T Proxy {
      get => this.proxy;
      set {
        if(this.SetValue(ref this.proxy, value)) {
        }
      }
    }

    public LinkService(ILogService logService, string applicationFamily, string serverEndpoint, string serverName, int protocolVersion) {
      this.logService = logService;
      this.applicationFamily = applicationFamily;
      this.serverEndpoint = serverEndpoint;
      this.serverName = serverName;
      this.protocolVersion = protocolVersion;
      this.isServerInstalled = new PredicateState(() => File.Exists(this.serverFilePath));
      this.DoConnect = new RelayCommand(() => {
        this.ConnectToService();
      }, this.isConnected.Negated);
    }

    public void HostService(Type serviceType, Type serviceInterfaceType, string endpoint) {
      this.serviceHost = new ServiceHost(serviceType, new Uri[] {
        new Uri("net.pipe://localhost/" + this.applicationFamily + endpoint),
      });
      serviceHost.AddServiceEndpoint(serviceInterfaceType, new NetNamedPipeBinding(), "index");
      serviceHost.Open();
    }

    public void ConnectToService() {
      if(this.isConnected.Value) {
        // already connected
        return;
      }
      try {
        if(this.proxy != null) {
          this.proxy.Abort();
        }
        this.Proxy = Activator.CreateInstance(typeof(T)) as T;
        this.proxy.InnerChannel.Opened += (s, e) => {
          this.isConnected.Value = true;
        };
        this.proxy.InnerChannel.Closed += (s, e) => {
          this.isConnected.Value = false;
        };
        var handshake = this.proxy.RequestHandshake();
        if(handshake.ProtocolVersion != this.protocolVersion) {
          this.proxy.Close();
          return;
        }
        this.ReportedServerName = handshake.ReportedName;
      } catch(EndpointNotFoundException) {
        this.isConnected.Value = false;
      } catch(Exception ex) {
        this.isConnected.Value = false;
        this.logService.Error($"failed to link with application: {Utils.GetExceptionDescription(ex)}", "application");
      }
    }

    public override Task LoadConfiguration(ServiceConfiguration configuration) {
      this.ServerFilePath = configuration.ReadValue(this.applicationFamily + this.serverEndpoint + "FilePath", null);
      if(this.serverFilePath == null) {
        var application = HAF.Utils.GetInstalledApplications().FirstOrDefault(a => a.Name == this.serverName);
        if(application != null) {
          this.ServerFilePath = application.Directory;
        }
      }
      return Task.CompletedTask;
    }

    public override Task SaveConfiguration(ServiceConfiguration configuration) {
      configuration.WriteValue(this.applicationFamily + this.serverEndpoint + "FilePath", this.serverFilePath);
      return Task.CompletedTask;
    }
  }
}
