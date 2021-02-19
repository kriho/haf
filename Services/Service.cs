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

  public enum ServiceId {
    Backend = 1,
    Projects,
    MainWindow,
    Settings,
    WindowLayout,
    Updates,
    Themes,
    UserRange = 100,
  }

  public enum LogSeverity {
    Error,
    Warning,
    Info,
  }

  public class LogEventArgs: EventArgs {
    public LogSeverity Severity { get; set; }
    public int ServiceId { get; set; }
    public string Message { get; set; }
  }

  public abstract class Service : ObservableObject, IService {

    public abstract  int Id { get; }

    public virtual void Load(Settings storage) {
    }

    public virtual void Save(Settings storage) {
    }

    public virtual void Clear() {
    }

    public Service() {
      this.Clear();
    }

    private struct Link {
      public int ServiceId;
      public int EntryId;
    }

    private struct Condition {
      public bool Value;
      public List<Link> Links;
    }

    private struct Dependency {
      public Action OnChanged;
      public List<Link> Links;
    }

    private struct Event {
      public List<Action> Listeners;
    }

    private static Dictionary<int, Dictionary<int, Dependency>> dependencies = new Dictionary<int, Dictionary<int, Dependency>>();
    private static Dictionary<int, Dictionary<int, Condition>> conditions = new Dictionary<int, Dictionary<int, Condition>>();
    private static Dictionary<int, Dictionary<int, Event>> events = new Dictionary<int, Dictionary<int, Event>>();

    public static EventHandler<LogEventArgs> OnLog;

    /// <summary>
    /// evaluate the current value of a dependency
    /// </summary>
    protected bool GetDependency(Enum dependencyId) {
      return this.GetDependency((int)(object)dependencyId);
    }

    /// <summary>
    /// evaluate the current value of a dependency
    /// </summary>
    protected bool GetDependency(int dependencyId) {
      if (Service.dependencies.TryGetValue(this.Id, out var service)) {
        if (service.TryGetValue(dependencyId, out var dependency)) {
          return dependency.Links.All(link => Service.GetCondition(link.ServiceId, link.EntryId));
        }
      }
      return true;
    }

    /// <summary>
    /// register the changed handler of a dependency
    /// </summary>
    /// <remarks>
    /// only one callback is possible, schould be used in owning service
    /// </remarks>
    protected void RegisterDependency(Enum dependencyId, Action onChanged) {
      this.RegisterDependency((int)(object)dependencyId, onChanged);
    }

    /// <summary>
    /// register the changed handler of a dependency
    /// </summary>
    /// <remarks>
    /// only one callback is possible, schould be used in owning service
    /// </remarks>
    protected void RegisterDependency(int dependencyId, Action onChanged) {
      if (!Service.dependencies.TryGetValue(this.Id, out var service)) {
        service = new Dictionary<int, Dependency>();
        Service.dependencies.Add(this.Id, service);
      }
      if (!service.TryGetValue(dependencyId, out var dependency)) {
        dependency = new Dependency() {
          Links = new List<Link>(),
        };
        service.Add(dependencyId, dependency);
      }
      dependency.OnChanged = onChanged;
    }

    /// <summary>
    /// declare a contraint for a dependency by adding a condition 
    /// </summary>
    /// <remarks>
    /// dependencies are true if all their conditions are true
    /// </remarks>
    public static void DeclareDependency(int dependentServiceId, int dependencyId, int serviceId, int conditionId) {
      if (!Service.dependencies.TryGetValue(dependentServiceId, out var service)) {
        service = new Dictionary<int, Dependency>();
        Service.dependencies.Add(dependentServiceId, service);
      }
      if (service.TryGetValue(dependencyId, out var dependency)) {
        dependency.Links.Add(new Link() {
          ServiceId = serviceId,
          EntryId = conditionId,
        });
      } else {
        service.Add(dependencyId, new Dependency() {
          Links = new List<Link>() {
        new Link() {
          ServiceId = serviceId,
          EntryId = conditionId,
        }
      },
        });
      }
    }

    /// <summary>
    /// get the current value of a condition
    /// </summary>
    private static bool GetCondition(int serviceId, int conditionId) {
      if (Service.conditions.TryGetValue(serviceId, out var service)) {
        return service[conditionId].Value;
      }
      return true;
    }

    /// <summary>
    /// update the value of a condition and notify all relevant dependencies
    /// </summary>
    protected void SetCondition(int conditionId, bool value) {
      if (!Service.conditions.TryGetValue(this.Id, out var service)) {
        service = new Dictionary<int, Condition>();
        Service.conditions.Add(this.Id, service);
      }
      if (!service.TryGetValue(conditionId, out var condition)) {
        condition = new Condition() {
          Value = value,
          Links = new List<Link>(),
        };
        service.Add(conditionId, condition);
      }
      condition.Value = value;
      foreach (var link in condition.Links) {
        if (Service.dependencies.TryGetValue(link.ServiceId, out var dependentService)) {
          if (dependentService.TryGetValue(link.EntryId, out var dependency)) {
            dependency.OnChanged?.Invoke();
          }
        }
      }
    }

    protected void FireEvent(Enum eventId) {
      this.FireEvent((int)(object)eventId);
    }

    protected void FireEvent(int eventId) {
      if (Service.events.TryGetValue(this.Id, out var service)) {
        if (service.TryGetValue(eventId, out var @event)) {
          foreach (var listener in @event.Listeners) {
            listener.Invoke();
          }
        }
      }
    }

    protected void Log(LogSeverity severity, string message) {
      Service.OnLog?.Invoke(this, new LogEventArgs() {
        Message = message,
        Severity = severity,
        ServiceId = this.Id,
      });
    }

    public static void RegisterEvent(Enum serviceId, Enum eventId, Action listener) {
      Service.RegisterEvent((int)(object)serviceId, (int)(object)eventId, listener);
    }

    public static void RegisterEvent(int serviceId, int eventId, Action listener) {
      if (!Service.events.TryGetValue(serviceId, out var service)) {
        service = new Dictionary<int, Event>();
        Service.events.Add(serviceId, service);
      }
      if (!service.TryGetValue(eventId, out var @event)) {
        @event = new Event() {
          Listeners = new List<Action>(),
        };
        service.Add((int)(object)eventId, @event);
      }
      @event.Listeners.Add(listener);
    }
  }
}
