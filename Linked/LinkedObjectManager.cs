using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  internal static class LinkedObjectManager {
    private static List<WeakReference<LinkedObject>> references = new List<WeakReference<LinkedObject>>();

    public static void Register(LinkedObject obj) {
      references.Add(new WeakReference<LinkedObject>(obj));
    }

    public static void AssignLinks() {
#if DEBUG
      foreach (var reference in references) {
        if(reference.TryGetTarget(out var obj)) {
          obj.AssignLinks();
        }
      }
#endif
    }
  }
}
