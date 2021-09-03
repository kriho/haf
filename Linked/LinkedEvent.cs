using HAF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HAF {

  /// <summary>
  /// declares an event of a linked object
  /// </summary>
  /// <remarks>
  /// event names must always start with On...
  /// </remarks>
  public class LinkedEvent<T> {
    public List<Action<T>> Listeners = new List<Action<T>>();

#if DEBUG
    public string Name;
#endif

    /// <summary>
    /// fire event
    /// </summary>
    public void Fire(T args) {
      foreach (var listener in this.Listeners) {
        listener.Invoke(args);
      }
#if DEBUG
      Console.WriteLine($"{this.Name}()");
#endif
    }

    /// <summary>
    /// register event listener
    /// </summary>
    public void Register(Action<T> listener) {
      this.Listeners.Add(listener);
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
    public List<Action> Listeners = new List<Action>();

#if DEBUG
    public string Name;
#endif

    /// <summary>
    /// fire event
    /// </summary>
    public void Fire() {
      foreach (var listener in this.Listeners) {
        listener.Invoke();
      }
#if DEBUG
      Console.WriteLine($"service event <{this.Name}> fired");
#endif
    }

    /// <summary>
    /// register event listener
    /// </summary>
    public void Register(Action listener) {
      this.Listeners.Add(listener);
    }

    public LinkedEvent(string name) {
#if DEBUG
      this.Name = name;
#endif
    }
  }

}
