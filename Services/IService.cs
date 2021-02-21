namespace HAF {
  public interface IService {
    void LoadConfiguration(Configuration configuration);
    void SaveConfiguration(Configuration configuration);
    void ClearConfiguration();
  }
}