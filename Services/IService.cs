namespace HAF {
  public interface IService {
    void LoadConfiguration(ServiceConfiguration configuration);
    void SaveConfiguration(ServiceConfiguration configuration);
    void ClearConfiguration();
  }
}