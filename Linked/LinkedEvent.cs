using HAF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Telerik.Windows.Data;

namespace HAF {

  /// <summary>
  /// declares an event of a linked object
  /// </summary>
  /// <remarks>
  /// event names must always start with On...
  /// </remarks>
  public class LinkedEvent<T> {
    public List<WeakAction<T>> Listeners = new List<WeakAction<T>>();

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
      Console.WriteLine($"{this.Name}()");
#endif
    }

    /// <summary>
    /// register event listener
    /// </summary>
    public void Register(Action<T> listener) {
      this.Listeners.Add(new WeakAction<T>(listener));
    }

    public LinkedEvent(string name) {
#if DEBUG
      this.Name = name;
#endif
    }
  }

  /// <summary>
  /// declares an event of a linked object
  /// </summary>
  /// <remarks>
  /// event names must always start with On...
  /// </remarks>
  public class LinkedEvent {
    public List<WeakAction> Listeners = new List<WeakAction>();

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

    public LinkedEvent() {
    }
  }

}
