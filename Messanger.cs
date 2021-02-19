using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace HAF {
  public class Messenger {
    private static readonly object creationLock = new object();

    private static Messenger defaultInstance;

    public static Messenger Default {
      get {
        if (defaultInstance == null) {
          lock (creationLock) {
            if (defaultInstance == null) {
              defaultInstance = new Messenger();
            }
          }
        }
        return defaultInstance;
      }
    }

    public bool Debug = false;

    private struct Route {
      public WeakAction Action;
      public object Token;
    }

    private object registerLock = new object();
    private bool isCleanupRegistered;
    private Dictionary<Type, List<Route>> routes;
    private Dictionary<Type, List<Route>> routesSubclasses;

    public Messenger() {
      this.routes = new Dictionary<Type, List<Route>>();
      this.routesSubclasses = new Dictionary<Type, List<Route>>();
    }

    #region register

    public virtual void Register<TMessage>(object recipient, Action<TMessage> action) {
      this.Register(recipient, null, false, action);
    }

    public virtual void Register<TMessage>(object recipient, bool receiveDerivedMessagesToo, Action<TMessage> action) {
      this.Register(recipient, null, receiveDerivedMessagesToo, action);
    }

    public virtual void Register<TMessage>(object recipient, object token, Action<TMessage> action) {
      this.Register(recipient, token, false, action);
    }

    public virtual void Register<TMessage>(object recipient, object token, bool receiveDerivedMessagesToo, Action<TMessage> action) {
      lock (this.registerLock) {
        // get message type
        var messageType = typeof(TMessage);

        // get routes
        Dictionary<Type, List<Route>> routes;
        if (receiveDerivedMessagesToo)
          routes = this.routesSubclasses;
        else
          routes = this.routes;

        lock (routes) {
          // get list of recipients
          List<Route> list;
          if (!routes.ContainsKey(messageType)) {
            list = new List<Route>();
            routes.Add(messageType, list);
          } else
            list = routes[messageType];

          // create new route
          var weakAction = new WeakAction<TMessage>(recipient, action);
          var item = new Route {
            Action = weakAction,
            Token = token
          };

          // add route to recipients
          list.Add(item);
        }
      }

      // remove all routes that have no valid action left
      this.RequestCleanup();
    }

    #endregion

    #region unregister

    public virtual void Unregister<TMessage>(object recipient) {
      this.Unregister<TMessage>(recipient, null, null);
    }

    public virtual void Unregister<TMessage>(object recipient, object token) {
      this.Unregister<TMessage>(recipient, token, null);
    }

    public virtual void Unregister<TMessage>(object recipient, Action<TMessage> action) {
      this.Unregister(recipient, null, action);
    }

    public virtual void Unregister<TMessage>(object recipient, object token, Action<TMessage> action) {
      // remove routes for recipient from lists
      this.QueryDeletionFromLists(recipient, token, action, this.routes);
      this.QueryDeletionFromLists(recipient, token, action, this.routesSubclasses);

      // remove all routes that have no valid action left
      this.RequestCleanup();
    }

    private void QueryDeletionFromLists<TMessage>(object recipient, object token, Action<TMessage> action, Dictionary<Type, List<Route>> lists) {
      // get message type
      var messageType = typeof(TMessage);

      // invalid recipient or lists
      if (recipient == null || lists == null || lists.Count == 0 || !lists.ContainsKey(messageType))
        return;

      lock (lists) {
        foreach (var route in lists[messageType]) {
          // cast action to desired generic
          var castedAction = route.Action as WeakAction<TMessage>;

          // query for deletion if all parameters fit
          if (castedAction != null && recipient == castedAction.Target && (action == null || action.Method.Name == castedAction.MethodName) && (token == null || token.Equals(route.Token)))
            route.Action.QueryDeletion();
        }
      }
    }

    public virtual void Unregister(object recipient) {
      // remove recipient from lists
      this.QueryDeletionFromLists(recipient, this.routes);
      this.QueryDeletionFromLists(recipient, this.routesSubclasses);

      // remove all routes that have no valid action left
      this.RequestCleanup();
    }

    private void QueryDeletionFromLists(object recipient, Dictionary<Type, List<Route>> lists) {
      // invalid recipient or lists
      if (recipient == null || lists == null || lists.Count == 0)
        return;

      lock (lists) {
        // query deletion for all routes to the specified recipient
        foreach (var messageType in lists.Keys)
          foreach (var route in lists[messageType])
            if (route.Action != null && recipient == route.Action.Target)
              route.Action.QueryDeletion();
      }
    }


    #endregion

    #region cleanup

    public void RequestCleanup() {
      if (!this.isCleanupRegistered) {
        // cleanup lists when application is idle
        Dispatcher.CurrentDispatcher.BeginInvoke((Action)this.Cleanup, DispatcherPriority.ApplicationIdle, null);

        this.isCleanupRegistered = true;
      }
    }

    private void Cleanup() {
      // remove all routes that have no valid action left from the lists
      this.CleanupList(this.routes);
      this.CleanupList(this.routesSubclasses);

      isCleanupRegistered = false;
    }


    private void CleanupList(IDictionary<Type, List<Route>> routes) {
      // invalid routes
      if (routes == null)
        return;

      lock (routes) {
        var removedKeys = new List<Type>();

        // clean lists and store keys to remove
        foreach (var route in routes) {
          route.Value.RemoveAll(item => item.Action == null || !item.Action.IsAlive);

          if (route.Value.Count == 0)
            removedKeys.Add(route.Key);
        }

        // remove keys
        foreach (var key in removedKeys)
          routes.Remove(key);
      }
    }

    #endregion

    #region send

    public virtual void Send<TMessage>(TMessage message) {
      this.SendToTargetOrType(message, null, null);
    }

    public virtual void Send<TMessage, TTarget>(TMessage message) {
      this.SendToTargetOrType(message, typeof(TTarget), null);
    }

    public virtual void Send<TMessage>(TMessage message, object token) {
      this.SendToTargetOrType(message, null, token);
    }

    private void SendToTargetOrType<TMessage>(TMessage message, Type messageTargetType, object token) {
#if TRACE
      Console.WriteLine("message " + typeof(TMessage).Name);
#endif
      // get message type
      var messageType = typeof(TMessage);

      if (this.routesSubclasses != null && this.routesSubclasses.Count > 0) {
        var types = this.routesSubclasses.Keys.Take(this.routesSubclasses.Keys.Count());

        foreach (var type in types) {
          // get all lists linked to a type that is or derives from the message target type
          if (messageType == type || messageType.IsSubclassOf(type) || type.IsAssignableFrom(messageType)) {
            List<Route> list;
            lock (this.routesSubclasses) {
              list = this.routesSubclasses[type].Take(this.routesSubclasses[type].Count()).ToList();
            }
            this.SendToList(message, list, messageTargetType, token);
          }
        }
      }

      if (this.routes != null && this.routes.Count > 0) {
        List<Route> list = null;

        lock (this.routes) {
          // get list linked to message target type
          if (this.routes.ContainsKey(messageType))
            list = this.routes[messageType].Take(this.routes[messageType].Count()).ToList();
        }

        if (list != null) {
          this.SendToList(message, list, messageTargetType, token);
        }
      }

      // remove all routes that have no valid action left
      this.RequestCleanup();
    }

    private void SendToList<TMessage>(TMessage message, List<Route> routes, Type messageTargetType, object token) {
      if (routes == null)
        return;

      foreach (var route in routes) {
        // get casted action
        var castedAction = route.Action as WeakAction<TMessage>;

        // validate route
        if (castedAction != null && castedAction.IsAlive && castedAction.Target != null
          && (messageTargetType == null || castedAction.Target.GetType() == messageTargetType || messageTargetType.IsAssignableFrom(castedAction.Target.GetType()))
           && ((route.Token == null && token == null) || route.Token != null && route.Token.Equals(token)))
          castedAction.Execute(message);
      }
    }

    #endregion
  }
}
