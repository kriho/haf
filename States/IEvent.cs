using System;

namespace HAF {
  public interface IReadOnlyEvent<T> {
    void Register(Action<T> listener);
    void RegisterWeak(Action<T> listener);
  }

  public interface IEvent<T>: IReadOnlyEvent<T> {
    void Fire(T args);
  }

  public interface IReadOnlyEvent {
    void Register(Action listener);
    void RegisterWeak(Action listener);
  }

  public interface IEvent: IReadOnlyEvent {
    void Fire();
  }
}