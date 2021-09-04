using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public class WeakAction {
    private Action staticAction;

    protected MethodInfo Method { get; set; }
    protected WeakReference ActionReference { get; set; }
    protected WeakReference Reference { get; set; }

    public virtual bool IsStatic {
      get {
        return this.staticAction != null;
      }
    }

    public bool IsAlive {
      get {
        if (this.IsStatic) {
          return this.Reference == null || this.Reference.IsAlive;
        } else {
          return this.Reference != null && this.Reference.IsAlive;
        }
      }
    }

    public object Target {
      get {
        return this.Reference?.Target;
      }
    }

    protected object ActionTarget {
      get {
        return this.ActionReference?.Target;
      }
    }

    public virtual string MethodName {
      get {
        return this.IsStatic ? this.staticAction.Method.Name : this.Method.Name;
      }
    }

    protected WeakAction() { }

    public WeakAction(Action action) : this(action?.Target, action) { }

    public WeakAction(object target, Action action) {
      if (action.Method.IsStatic) {
        this.staticAction = action;
        if (target != null) {
          this.Reference = new WeakReference(target);
        }
      } else {
        this.Method = action.Method;
        this.ActionReference = new WeakReference(action.Target);
        this.Reference = new WeakReference(target);
      }
    }

    public void Execute() {
      if (this.IsStatic) {
        this.staticAction();
      } else if (this.IsAlive) {
        if (this.Method != null && this.ActionReference != null && this.ActionTarget != null) {
          Method.Invoke(this.ActionTarget, null);
        }
      }
    }

    public void QueryDeletion() {
      this.Reference = null;
      this.ActionReference = null;
      this.Method = null;
      this.staticAction = null;
    }
  }

  public class WeakAction<T>: WeakAction {
    private Action<T> staticAction;

    public override bool IsStatic {
      get {
        return this.staticAction != null;
      }
    }

    public override string MethodName {
      get {
        return this.IsStatic ? this.staticAction.Method.Name : this.Method.Name;
      }
    }

    public WeakAction(Action<T> action) : this(action?.Target, action) { }

    public WeakAction(object target, Action<T> action) {
      if (action.Method.IsStatic) {
        this.staticAction = action;
        if (target != null) {
          Reference = new WeakReference(target);
        }
      } else {
        this.Method = action.Method;
        this.ActionReference = new WeakReference(action.Target);
        this.Reference = new WeakReference(target);
      }
    }

    public new void Execute() {
      this.Execute(default);
    }

    public void Execute(T parameter) {
      if (this.IsStatic) {
        this.staticAction(parameter);
      } else if (this.IsAlive) {
        if (this.Method != null && this.ActionReference != null && this.ActionTarget != null) {
          Method.Invoke(this.ActionTarget, new object[] { parameter });
        }
      }
    }

    public new void QueryDeletion() {
      this.staticAction = null;
      base.QueryDeletion();
    }
  }
}