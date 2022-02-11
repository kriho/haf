using System.ComponentModel;
using System.Threading.Tasks;

namespace HAF {
  public interface IService: INotifyPropertyChanged {
    /// <summary>
    /// Load the service configuration. Called automatically when the service is registered in the <c>Configuration</c> or the <c>ProjectService</c>.
    /// </summary>
    Task LoadConfiguration(ServiceConfiguration configuration);

    /// <summary>
    /// Save the service configuration. Called automatically when the service is registered in the <c>Configuration</c> or the <c>ProjectService</c>.
    /// </summary>
    Task SaveConfiguration(ServiceConfiguration configuration);

    /// <summary>
    /// Reset the service.
    /// </summary>
    Task Reset();
  }
}