using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace HAF {
  public static class Messenger {
    private static readonly object @lock = new object();

    private static Dictionary<Type, List<WeakAction<object>>> routes = new Dictionary<Type, List<WeakAction<object>>>();

    public static void Register<T>(Action<T> action) {
      lock (Messenger.@lock) {
        var messageType = typeof(T);
        if (!Messenger.routes.TryGetValue(messageType, out var routes)) {
          routes = new List<WeakAction<object>>();
          Messenger.routes.Add(messageType, routes);
        }
        routes.Add(new WeakAction<object>(o => action((T)o)));
      }
    }

    public static void Unregister<T>(Action<T> action) {
      lock (Messenger.@lock) {
        var messageType = typeof(T);
        if (Messenger.routes.TryGetValue(messageType, out var routes)) {
          routes.RemoveAll(r => r.MethodName == action.Method.Name && r.Target == action.Target);
        }
      }
    }

    public static void Send<T>(T message) {
      if (Messenger.routes.TryGetValue(typeof(T), out var routes)) {
        foreach (var action in routes) {
          action.Execute(message);
        }
      }
    }
  }
}
