using HAF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public class PatchNotesEntryType: Enumeration {

    public static PatchNotesEntryType Feature = new PatchNotesEntryType(1, "feature");
    public static PatchNotesEntryType Improvement = new PatchNotesEntryType(2, "improvement");
    public static PatchNotesEntryType Fix = new PatchNotesEntryType(3, "fix");
    public static PatchNotesEntryType CriticalFix = new PatchNotesEntryType(4, "critical fix");
    public static PatchNotesEntryType Backlog = new PatchNotesEntryType(5, "backlog");

    public PatchNotesEntryType(int id, string name): base(id, name) {
    }
  }
}
