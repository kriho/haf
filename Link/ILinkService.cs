using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace HAF {
  public interface ILinkService<T, S>: IService where T : ClientBase<S> where S : class {
    IReadOnlyState IsConnected { get; }
    IReadOnlyState IsServerInstalled { get; }
    T Proxy { get; }
    string ServerFilePath { get; }
    IRelayCommand DoConnect { get; }

    void ConnectToService();
    void HostService(Type serviceType, Type serviceInterfaceType, string endpoint);
  }
}