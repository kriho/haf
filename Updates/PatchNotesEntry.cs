using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public class PatchNotesEntry: IPatchNotesEntry {
    public PatchNotesEntryType Type { get; private set; }

    public string Description { get; private set; }

    public IPatchNotes Parent { get; private set; }

    public PatchNotesEntry(IPatchNotes parent, PatchNotesEntryType type, string description) {
      this.Parent = parent;
      this.Type = type; 
      this.Description = description;
    }
  }
}
