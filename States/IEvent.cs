using System;

namespace HAF {
  public interface IEvent<T> {
    void Fire(T args);
    void Register(Action<T> listener);
    void RegisterWeak(Action<T> listener);
  }

  public interface IEvent {
    void Fire();
    void Register(Action listener);
    void RegisterWeak(Action listener);
  }
}