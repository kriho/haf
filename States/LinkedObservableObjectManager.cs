using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  internal static class LinkedObservableObjectManager {
    private static List<WeakReference<LinkedObservableObject>> references = new List<WeakReference<LinkedObservableObject>>();

    public static void Register(LinkedObservableObject obj) {
      references.Add(new WeakReference<LinkedObservableObject>(obj));
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
