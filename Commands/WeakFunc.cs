using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public class WeakFunc<T> {
    private Func<T> staticFunc;

    protected MethodInfo Method { get; set; }
    protected WeakReference FuncReference { get; set; }
    protected WeakReference Reference { get; set; }

    public virtual bool IsStatic {
      get {
        return this.staticFunc != null;
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

    protected object FuncTarget {
      get {
        return this.FuncReference?.Target;
      }
    }

    public virtual string MethodName {
      get {
        return this.IsStatic ? this.staticFunc.Method.Name : this.Method.Name;
      }
    }

    protected WeakFunc() { }

    public WeakFunc(Func<T> func) : this(func?.Target, func) { }

    public WeakFunc(object target, Func<T> func) {
      if (func.Method.IsStatic) {
        this.staticFunc = func;
        if (target != null) {
          this.Reference = new WeakReference(target);
        }
      } else {
        this.Method = func.Method;
        this.FuncReference = new WeakReference(func.Target);
        this.Reference = new WeakReference(target);
      }
    }

    public T Execute() {
      if (this.IsStatic) {
        return this.staticFunc();
      }
      if (this.IsAlive) {
        if (this.Method != null && this.FuncReference != null && this.FuncTarget != null) {
          return (T)Method.Invoke(this.FuncTarget, null);
        }
      }
      return default;
    }

    public void QueryDeletion() {
      this.Reference = null;
      this.FuncReference = null;
      this.Method = null;
      this.staticFunc = null;
    }
  }

  public class WeakFunc<Tin, Tout> : WeakFunc<Tout> {
    private Func<Tin, Tout> staticFunc;

    public override bool IsStatic {
      get {
        return this.staticFunc != null;
      }
    }

    public override string MethodName {
      get {
        return this.IsStatic ? this.staticFunc.Method.Name : this.Method.Name;
      }
    }

    public WeakFunc(Func<Tin, Tout> func) : this(func?.Target, func) { }

    public WeakFunc(object target, Func<Tin, Tout> func) {
      if (func.Method.IsStatic) {
        this.staticFunc = func;
        if (target != null) {
          this.Reference = new WeakReference(target);
        }
      } else {
        this.Method = func.Method;
        this.FuncReference = new WeakReference(func.Target);
        this.Reference = new WeakReference(target);
      }
    }

    public new Tout Execute() {
      return this.Execute(default);
    }

    public Tout Execute(Tin parameter) {
      if (this.IsStatic) {
        return this.staticFunc(parameter);
      }
      if (this.IsAlive) {
        if (this.Method != null && this.FuncReference != null && this.FuncTarget != null) {
          return (Tout)Method.Invoke(this.FuncTarget, new object[] { parameter });
        }
      }
      return default;
    }

    public new void QueryDeletion() {
      this.staticFunc = null;
      base.QueryDeletion();
    }
  }
}