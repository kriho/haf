using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HAF {
  public abstract class ValidatableObject: ObservableObject, INotifyDataErrorInfo {
    private ValidationBatch validationBatch = new ValidationBatch();

    private readonly object validationLock = new object();

    private readonly ConcurrentDictionary<string, List<string>> validationErrors = new ConcurrentDictionary<string, List<string>>();

    /// <summary>
    /// Add validation errors that are gathered with a validation batch to the error list of the object.
    /// </summary>
    /// <param name="propertyName">property name that the errors are associated with, use "" for object scoped validation errors</param>
    /// <param name="batch">the validation batch, used to gather validation errors</param>
    private void ValidateBatch(string propertyName, Action<ValidationBatch> batch) {
      lock(this.validationLock) {
        // prepare batch
        this.validationBatch.PropertyName = propertyName;
        this.validationBatch.Errors.Clear();
        // execute assertions
        batch.Invoke(this.validationBatch);
        // apply batch
        if(this.validationErrors.TryGetValue(this.validationBatch.PropertyName, out var oldErrors)) {
          if(this.validationBatch.Errors.Count == 0) {
            if(oldErrors.Count > 0) {
              oldErrors.Clear();
              this.OnErrorsChanged(this.validationBatch.PropertyName);
              this.NotifyPropertyChanged(() => this.HasErrors);
            }
          } else {
            oldErrors.Clear();
            oldErrors.AddRange(this.validationBatch.Errors);
            this.OnErrorsChanged(this.validationBatch.PropertyName);
            this.NotifyPropertyChanged(() => this.HasErrors);
          }
        } else if(this.validationBatch.Errors.Count > 0) {
          _ = this.validationErrors.TryAdd(this.validationBatch.PropertyName, new List<string>(this.validationBatch.Errors));
          this.OnErrorsChanged(this.validationBatch.PropertyName);
          this.NotifyPropertyChanged(() => this.HasErrors);
        }
      }
    }

    /// <summary>
    /// Set the property value, perform validation and send a change notification when the value changed.
    /// </summary>
    /// <typeparam name="T">the value type</typeparam>
    /// <param name="property">reference to the property, used to update the property value</param>
    /// <param name="value">the new value</param>
    /// <param name="propertyValidation">the validation batch, used to gather validation errors for this property</param>
    /// <param name="subscribe">subsicibe to changes in the new property value, can be used to redirect change notification from the value object to this object</param>
    /// <param name="propertyName">the name of the property, provided by the compiler when called from within the property</param>
    /// <returns>true if the value has changed</returns>
    protected bool SetValueAndValidate<T>(ref T property, T value, Action<ValidationBatch> propertyValidation, bool subscribe = false, [CallerMemberName] string propertyName = "") {
      if (!this.SetValue(ref property, value, subscribe, propertyName)) {
        return false;
      }
      this.ValidateBatch(propertyName, propertyValidation);
      return true;
    }

    /// <summary>
    /// Set the property value, perform validation and send a change notification when the value changed.
    /// </summary>
    /// <typeparam name="T">the value type</typeparam>
    /// <param name="property">reference to the property, used to update the property value</param>
    /// <param name="value">the new value</param>
    /// <param name="propertyValidation">the validation batch, used to gather validation errors for this property</param>
    /// <param name="subscribe">subsicibe to changes in the new property value, can be used to redirect change notification from the value object to this object</param>
    /// <param name="expression">a member expression that is used to resolve the property name</param>
    /// <returns>true if the value has changed</returns>
    protected bool SetValueAndValidate<T>(ref T property, T value, Action<ValidationBatch> propertyValidation, Expression<Func<T>> expression, bool subscribe = false) {
      return this.SetValueAndValidate(ref property, value, propertyValidation, subscribe, this.GetExpressionMemberName(expression));
    }

    /// <summary>
    /// Set the property value, perform object validation using Validate() and send a change notification when the value changed.
    /// </summary>
    /// <typeparam name="T">the value type</typeparam>
    /// <param name="property">reference to the property, used to update the property value</param>
    /// <param name="value">the new value</param>
    /// <param name="subscribe">subsicibe to changes in the new property value, can be used to redirect change notification from the value object to this object</param>
    /// <param name="propertyName">the name of the property, provided by the compiler when called from within the property</param>
    /// <returns>true if the value has changed</returns>
    protected bool SetValueAndValidate<T>(ref T property, T value, bool subscribe = false, [CallerMemberName] string propertyName = "") {
      if(!this.SetValue(ref property, value, subscribe, propertyName)) {
        return false;
      }
      this.ValidateBatch("", this.Validate);
      return true;
    }

    /// <summary>
    /// Set the property value, perform object validation using Validate() and send a change notification when the value changed.
    /// </summary>
    /// <typeparam name="T">the value type</typeparam>
    /// <param name="property">reference to the property, used to update the property value</param>
    /// <param name="value">the new value</param>
    /// <param name="subscribe">subsicibe to changes in the new property value, can be used to redirect change notification from the value object to this object</param>
    /// <param name="expression">a member expression that is used to resolve the property name</param>
    /// <returns>true if the value has changed</returns>
    protected bool SetValueAndValidate<T>(ref T property, T value, Expression<Func<T>> expression, bool subscribe = false) {
      return this.SetValueAndValidate(ref property, value, subscribe, this.GetExpressionMemberName(expression));
    }

    /// <summary>
    /// Perform validation of the object and associate all validation errors with the object.
    /// </summary>
    /// <param name="objectValidation">the validation batch, used to gather validation errors for this object</param>
    protected virtual void Validate(ValidationBatch objectValidation) {
    }

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    public bool HasErrors {
      get {
        return this.validationErrors.Any(e => e.Value != null && e.Value.Count > 0);
      }
    }

    public IEnumerable GetErrors(string propertyName) {
      // https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifydataerrorinfo?view=net-5.0
      // empty propertyName or null indicates that entity level errors shell be retrieved
      this.validationErrors.TryGetValue(propertyName  ?? "", out var errorsForProperty);
      return errorsForProperty ?? Enumerable.Empty<string>();
    }

    public void OnErrorsChanged(string propertyName) {
      ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }
  }
}
