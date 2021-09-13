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
    private readonly List<Action<T>> listeners = new List<Action<T>>();
    private readonly List<WeakAction<T>> weakListeners = new List<WeakAction<T>>();

#if DEBUG
    public string Name;
#endif

    /// <summary>
    /// fire event
    /// </summary>
    public void Fire(T args) {
      // invoke strong references
      foreach (var listener in this.listeners) {
        listener.Invoke(args);
      }
      // invoke weak references
      var index = 0;
      while(index < this.weakListeners.Count) {
        if(this.weakListeners[index].IsAlive) {
          this.weakListeners[index++].Execute(args);
        } else {
          this.weakListeners.RemoveAt(index);
        }
      }
#if DEBUG
      Console.WriteLine($"{this.Name}()");
#endif
    }

    /// <summary>
    /// register a strong referenced event listener
    /// </summary>
    public void Register(Action<T> listener) {
      this.listeners.Add(listener);
    }

    /// <summary>
    /// register a weak referenced event listener
    /// </summary>
    public void RegisterWeak(Action<T> listener) {
      this.weakListeners.Add(new WeakAction<T>(listener));
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
    private List<Action> listeners = new List<Action>();
    private readonly List<WeakAction> weakListeners = new List<WeakAction>();

#if DEBUG
    public string Name;
#endif

    /// <summary>
    /// fire event
    /// </summary>
    public void Fire() {
      // invoke strong references
      foreach(var listener in this.listeners) {
        listener.Invoke();
      }
      // invoke weak references
      var index = 0;
      while(index < this.weakListeners.Count) {
        if(this.weakListeners[index].IsAlive) {
          this.weakListeners[index++].Execute();
        } else {
          this.weakListeners.RemoveAt(index);
        }
      }
#if DEBUG
      Console.WriteLine($"service event <{this.Name}> fired");
#endif
    }

    /// <summary>
    /// register a strong referenced event listener
    /// </summary>
    public void Register(Action listener) {
      this.listeners.Add(listener);
    }

    /// <summary>
    /// register a weak referenced event listener
    /// </summary>
    public void RegisterWeak(Action listener) {
      this.weakListeners.Add(new WeakAction(listener));
    }

    public LinkedEvent(string name) {
#if DEBUG
      this.Name = name;
#endif
    }
  }

}
