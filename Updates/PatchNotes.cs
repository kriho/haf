using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public class PatchNotes: IPatchNotes {
    public string Version { get; }

    public List<IPatchNotesEntry> Entries { get; set; } = new List<IPatchNotesEntry>();

    public PatchNotes(string version) {
      this.Version = version;
    }

    public IPatchNotes Fix(string description) {
      this.Entries.Add(new PatchNotesEntry(this, PatchNotesEntryType.Fix, description));
      return this;
    }

    public IPatchNotes CriticalFix(string description) {
      this.Entries.Add(new PatchNotesEntry(this, PatchNotesEntryType.CriticalFix, description));
      return this;
    }

    public IPatchNotes Feature(string description) {
      this.Entries.Add(new PatchNotesEntry(this, PatchNotesEntryType.Feature, description));
      return this;
    }

    public IPatchNotes Improvement(string description) {
      this.Entries.Add(new PatchNotesEntry(this, PatchNotesEntryType.Improvement, description));
      return this;
    }

    public IPatchNotes Backlog(string description) {
      this.Entries.Add(new PatchNotesEntry(this, PatchNotesEntryType.Backlog, description));
      return this;
    }
  }
}
