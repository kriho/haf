using System;

namespace HAF {
  public interface IReadOnlyEvent<T> {
    /// <summary>
    /// Register callback that is invoked when the event fires.
    /// </summary>
    void Register(Action<T> listener);

    /// <summary>
    /// Register weak callback that is invoked when the event fires.
    /// </summary>
    void RegisterWeak(Action<T> listener);
  }

  public interface IEvent<T>: IReadOnlyEvent<T> {
    /// <summary>
    /// Fire the event. Whis will invoke all registered callbacks.
    /// </summary>
    void Fire(T args);
  }

  public interface IReadOnlyEvent {
    /// <summary>
    /// Register callback that is invoked when the event fires.
    /// </summary>
    void Register(Action listener);

    /// <summary>
    /// Register weak callback that is invoked when the event fires.
    /// </summary>
    void RegisterWeak(Action listener);
  }

  public interface IEvent: IReadOnlyEvent {
    /// <summary>
    /// Fire the event. Whis will invoke all registered callbacks.
    /// </summary>
    void Fire();
  }
}