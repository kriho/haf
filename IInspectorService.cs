namespace HAF {
  public interface IInspectorService: IService {
    IRelayCommand DoClearSelectedItem { get; }
    IRelayCommand<object> DoSetSelectedItem { get; }
    object SelectedItem { get; set; }
    object SelectedSection { get; set; }
    IReadOnlyEvent<object> OnSelectedItemChanged { get; }
  }
}