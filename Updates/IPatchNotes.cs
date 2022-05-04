using System.Collections.Generic;

namespace HAF {
  public interface IPatchNotes {
    string Version { get; }
    List<IPatchNotesEntry> Entries { get; }
    IPatchNotes Fix(string description);
    IPatchNotes CriticalFix(string description);
    IPatchNotes Feature(string description);
    IPatchNotes Improvement(string description);
    IPatchNotes Backlog(string description);
  }
}