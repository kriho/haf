using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HAF {
  public class ObservableObject: INotifyPropertyChanged {
    public event PropertyChangedEventHandler PropertyChanged;

    [Conditional("DEBUG")]
    [DebuggerStepThrough]
    public void VerifyPropertyName(string propertyName) {
      var myType = GetType();
      if (!string.IsNullOrEmpty(propertyName) && myType.GetProperty(propertyName) == null) {
        if (this is ICustomTypeDescriptor descriptor) {
          if (descriptor.GetProperties().Cast<PropertyDescriptor>().Any(property => property.Name == propertyName)) {
            return;
          }
        }
        if (propertyName == "Item[]") {
          // TODO check if indexer property exists
          return;
        }
        throw new ArgumentException("failed to notify changed for unknown property", propertyName);
      }
    }

    protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
      // validate property name
      this.VerifyPropertyName(propertyName);
      // update value using dispatcher if needed
      if (Application.Current.Dispatcher.CheckAccess()) {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      } else {
#if DEBUG
        Console.WriteLine($"dispatching change notification for {propertyName}");
#endif
        if (this.PropertyChanged != null) {
          Application.Current.Dispatcher.Invoke(new Action(() => {
            this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
          }));
        }
      }
    }

    protected void NotifyPropertyChanged<T>(Expression<Func<T>> expression) {
      this.NotifyPropertyChanged(this.GetExpressionMemberName(expression));
    }

    protected string GetExpressionMemberName<T>(Expression<Func<T>> expression) {
      if (expression.Body is MemberExpression memberExpression) {
        if (memberExpression.Member != null) {
          return memberExpression.Member.Name;
        }
      }
      throw new Exception("resolving the property name from a member expression has failed");
    }

    protected bool SetValue<T>(ref T property, T value, bool subscribe = false, [CallerMemberName] string propertyName = "") {
      if (Object.Equals(property, value)) {
        // no changes
        return false;
      }
      // assign new value
      property = value;
      if (subscribe && value is INotifyPropertyChanged notify) {
        notify.PropertyChanged += (s, e) => {
          this.NotifyPropertyChanged(propertyName);
        };
      }
      this.NotifyPropertyChanged(propertyName);
      return true;
    }

    protected bool SetValue<T>(ref T property, T value, Expression<Func<T>> expression, bool subscribe = false) {
      return this.SetValue(ref property, value, subscribe, this.GetExpressionMemberName(expression));
    }

    private static bool? isInDesignMode;

    public bool IsInDesignMode {
      get {
        return ObservableObject.IsInDesignModeStatic;
      }
    }

    public static bool IsInDesignModeStatic {
      get {
        if (!ObservableObject.isInDesignMode.HasValue) {
          ObservableObject.isInDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty, typeof(FrameworkElement)).Metadata.DefaultValue;
        }
        return ObservableObject.isInDesignMode.Value;
      }
    }
  }
}
