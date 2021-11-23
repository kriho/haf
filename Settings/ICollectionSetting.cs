namespace HAF {
  public interface ICollectionSetting<T>: ISettingMeta {
    IObservableCollection<T> Collection { get; }
  }
}