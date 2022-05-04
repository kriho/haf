using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace HAF {
  public interface ILinkService: IService{
    /// <summary>
    /// Is the application connected to the target service.
    /// </summary>
    IReadOnlyState IsConnected { get; }

    /// <summary>
    /// Has the application successfully installed the local service host.
    /// </summary>
    IReadOnlyState IsServerInstalled { get; }

    /// <summary>
    /// Path the the executable of the target service.
    /// </summary>
    string ServerFilePath { get; }

    /// <summary>
    /// Connect to the target service.
    /// </summary>
    IRelayCommand DoConnect { get; }

    /// <summary>
    /// Connect to the target service.
    /// </summary>
    void ConnectToService();

    /// <summary>
    /// Install the local service host.
    /// </summary>
    void HostService(Type serviceType, Type serviceInterfaceType, string endpoint);
  }
}