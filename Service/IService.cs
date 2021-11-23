using System.ComponentModel;
using System.Threading.Tasks;

namespace HAF {
  public interface IService: INotifyPropertyChanged {
    Task LoadConfiguration(ServiceConfiguration configuration);
    Task SaveConfiguration(ServiceConfiguration configuration);
    Task Reset();
  }
}