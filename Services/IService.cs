namespace HAF {
  public interface IService {
    int Id { get; }
    void Clear();
    void Load(Settings storage);
    void Save(Settings storage);
  }
}