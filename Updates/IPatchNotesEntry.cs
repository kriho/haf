namespace HAF {
  public interface IPatchNotesEntry {
    PatchNotesEntryType Type { get; }
    string Description { get; }
    IPatchNotes Parent { get; }
  }
}