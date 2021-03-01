﻿using HAF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Telerik.Windows.Data;

namespace HAF {

  public class ServiceEvent<T> {
    public List<WeakAction<T>> Listeners;

#if DEBUG
    public string Name;
#endif

    /// <summary>
    /// fire event
    /// </summary>
    public void Fire(T args) {
      foreach (var listener in this.Listeners) {
        if (listener.IsAlive) {
          listener.Execute(args);
        } else {
          // TODO remove from list
        }
      }
#if DEBUG
      Console.WriteLine($"service event <{this.Name}> fired");
#endif
    }

    /// <summary>
    /// register event listener
    /// </summary>
    public void Register(Action<T> listener) {
      this.Listeners.Add(new WeakAction<T>(listener));
    }

    public ServiceEvent(string name) {
#if DEBUG
      this.Name = name;
#endif
    }
  }

  public class ServiceEvent {
    public List<WeakAction> Listeners;

#if DEBUG
    public string Name;
#endif

    /// <summary>
    /// fire event
    /// </summary>
    public void Fire() {
      foreach (var listener in this.Listeners) {
        if (listener.IsAlive) {
          listener.Execute();
        } else {
          // TODO remove from list
        }
      }
#if DEBUG
      Console.WriteLine($"service event <{this.Name}> fired");
#endif
    }

    /// <summary>
    /// register event listener
    /// </summary>
    public void Register(Action listener) {
      this.Listeners.Add(new WeakAction(listener));
    }

    public ServiceEvent(string name) {
#if DEBUG
      this.Name = name;
#endif
    }
  }

}
