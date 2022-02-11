namespace HAF {
  public interface IInspectorService: IService {
    /// <summary>
    /// Clear the selected item.
    /// </summary>
    IRelayCommand DoClearSelectedItem { get; }

    /// <summary>
    /// Set the selected item to the provided value.
    /// </summary>
    IRelayCommand<object> DoSetSelectedItem { get; }

    /// <summary>
    /// Currently selected item.
    /// </summary>
    object SelectedItem { get; set; }

    /// <summary>
    /// Currently selected section within the selected item.
    /// </summary>
    object SelectedSection { get; set; }

    /// <summary>
    /// Event that fires when the selected item changes.
    /// </summary>
    IReadOnlyEvent<object> OnSelectedItemChanged { get; }
  }
}