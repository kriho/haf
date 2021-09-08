using System.ComponentModel;

namespace HAF {
  public interface IService: INotifyPropertyChanged {
    void LoadConfiguration(ServiceConfiguration configuration);
    void SaveConfiguration(ServiceConfiguration configuration);
    void ClearConfiguration();
  }
}