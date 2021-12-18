using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace HAF {
  public interface ILinkService {
    IReadOnlyState IsConnected { get; }
    IReadOnlyState IsServerInstalled { get; }
    string ServerFilePath { get; }
    IRelayCommand DoConnect { get; }

    void ConnectToService();
    void HostService(Type serviceType, Type serviceInterfaceType, string endpoint);
  }
}