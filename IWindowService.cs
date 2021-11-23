namespace HAF {
  public interface IWindowService : IService {
    Window Window { get; set; }
    void RegisterControl<T>(string name, T control);
    bool TryGetControl<T>(string name, out T control);
    T GetControl<T>(string name);
  }
}