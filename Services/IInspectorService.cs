namespace HAF {
  public interface IInspectorService {
    RelayCommand DoClearSelectedItem { get; }
    RelayCommand<object> DoSetSelectedItem { get; }
    object SelectedItem { get; set; }
  }
}