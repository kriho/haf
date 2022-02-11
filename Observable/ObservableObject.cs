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
using System.Windows.Data;

namespace HAF {
  public class ObservableObject: INotifyPropertyChanged {
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Verify existance of a property.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    [Conditional("DEBUG")]
    [DebuggerStepThrough]
    private void VerifyPropertyName(string propertyName) {
      var myType = GetType();
      if (!string.IsNullOrEmpty(propertyName) && myType.GetProperty(propertyName) == null) {
        if (this is ICustomTypeDescriptor descriptor) {
          if (descriptor.GetProperties().Cast<PropertyDescriptor>().Any(property => property.Name == propertyName)) {
            return;
          }
        }
        if (propertyName == Binding.IndexerName) {
          // TODO check if indexer property exists
          return;
        }
        throw new ArgumentException("failed to notify changed for unknown property", propertyName);
      }
    }

    /// <summary>
    /// Use the <c>PropertyChanged</c> event to notify listeners about a changed property value.
    /// </summary>
    /// <param name="propertyName">Name of the property, provided by the compiler when called from within the property.</param>
    protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
      // validate property name
      this.VerifyPropertyName(propertyName);
      // update value using dispatcher if needed
      if (Application.Current.Dispatcher.CheckAccess()) {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      } else {
        if (this.PropertyChanged != null) {
          Application.Current.Dispatcher.Invoke(() => {
            this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
          });
        }
      }
    }

    /// <summary>
    /// Use the <c>PropertyChanged</c> event to notify listeners about a changed property value.
    /// </summary>
    /// <param name="propertyName">An expresstion that resolves as the property.</param>
    protected void NotifyPropertyChanged<T>(Expression<Func<T>> expression) {
      this.NotifyPropertyChanged(this.GetExpressionMemberName(expression));
    }

    /// <summary>
    /// Get the name of a member expression.
    /// </summary>
    protected string GetExpressionMemberName<T>(Expression<Func<T>> expression) {
      if (expression.Body is MemberExpression memberExpression) {
        if (memberExpression.Member != null) {
          return memberExpression.Member.Name;
        }
      }
      throw new Exception("resolving the property name from a member expression has failed");
    }

    /// <summary>
    /// Set the property value and send a change notification when the value changed.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="property">Reference to the property, used to update the property value.</param>
    /// <param name="value">The new value.</param>
    /// <param name="subscribe">Subscribe to changes in the new property value. Can be used to redirect change notification from the value object to this object.</param>
    /// <param name="propertyName">The name of the property. Provided by the compiler when called from within the property.</param>
    /// <returns>True if the value has changed.</returns>
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

    /// <summary>
    /// Set the property value and send a change notification when the value changed.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="property">Reference to the property. Used to update the property value.</param>
    /// <param name="value">The new value.</param>
    /// <param name="expression">A member expression that is used to resolve the property.</param>
    /// <param name="subscribe">Subscribe to changes in the new property value. Can be used to redirect change notification from the value object to this object.</param>
    /// <returns>True if the value has changed.</returns>
    protected bool SetValue<T>(ref T property, T value, Expression<Func<T>> expression, bool subscribe = false) {
      return this.SetValue(ref property, value, subscribe, this.GetExpressionMemberName(expression));
    }

    private static bool? isInDesignMode;

    /// <summary>
    /// Check if the application running in design mode (Visual Studio or Expression Blend).
    /// </summary>
    public static bool IsInDesignModeStatic {
      get {
        if (!ObservableObject.isInDesignMode.HasValue) {
          ObservableObject.isInDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty, typeof(FrameworkElement)).Metadata.DefaultValue;
        }
        return ObservableObject.isInDesignMode.Value;
      }
    }

    /// <summary>
    /// Check if the application running in design mode (Visual Studio or Expression Blend).
    /// </summary>
    public bool IsInDesignMode {
      get {
        return ObservableObject.IsInDesignModeStatic;
      }
    }
  }
}
