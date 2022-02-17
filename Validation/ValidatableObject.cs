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
    private readonly object validationLock = new object();

    private readonly ConcurrentDictionary<string, List<string>> validationErrors = new ConcurrentDictionary<string, List<string>>();

    private State isValid = new State(true);
    /// <summary>
    /// Has the object passed validation.
    /// </summary>
    public IReadOnlyState IsValid => this.isValid;

    /// <summary>
    /// Update validation errors for property with provided name.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="errors">Updated list of errors for the property.</param>
    private void UpdateValidationErrors(string propertyName, List<string> errors) {
      if(this.validationErrors.TryGetValue(propertyName, out var oldErrors)) {
        if(errors.Count == 0) {
          if(oldErrors.Count > 0) {
            oldErrors.Clear();
            this.OnErrorsChanged(propertyName);
          }
        } else {
          oldErrors.Clear();
          oldErrors.AddRange(errors);
          this.OnErrorsChanged(propertyName);
        }
      } else if(errors.Count > 0) {
        _ = this.validationErrors.TryAdd(propertyName, new List<string>(errors));
        this.OnErrorsChanged(propertyName);
      }
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
      lock(this.validationLock) {
        // prepare batch
        var validationBatch = new ObjectValidationBatch(propertyName);
        // execute validation
        this.Validate(validationBatch);
        // apply batch
        foreach(var errorPropertyName in validationBatch.Properties.Keys) {
          this.UpdateValidationErrors(errorPropertyName, validationBatch.Properties[errorPropertyName].Errors);
        }
        this.UpdateValidationErrors("", validationBatch.Errors);
      }
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
    protected virtual void Validate(ObjectValidationBatch validationBatch) {
    }

    /// <summary>
    /// Request validation of the object and all validatable properties.
    /// </summary>
    public void RequestValidation() {
      lock(this.validationLock) {
        // prepare batch
        var validationBatch = new ObjectValidationBatch("");
        // execute validation
        this.Validate(validationBatch);
        // apply batch
        foreach(var errorPropertyName in validationBatch.Properties.Keys) {
          this.UpdateValidationErrors(errorPropertyName, validationBatch.Properties[errorPropertyName].Errors);
        }
        this.UpdateValidationErrors("", validationBatch.Errors);
      }
    }

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    public bool HasErrors {
      get {
        return this.validationErrors.Any(e => e.Value != null && e.Value.Count > 0);
      }
    }

    /// <summary>
    /// Current validation errors of the object excluding errors that are associated with the objects properties.
    /// </summary>
    public IEnumerable<string> ObjectErrors => this.validationErrors.TryGetValue("", out var errors) ? errors : Enumerable.Empty<string>();

    public IEnumerable GetErrors(string propertyName) {
      // https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifydataerrorinfo?view=net-5.0
      // empty propertyName or null indicates that entity level errors shell be retrieved
      this.validationErrors.TryGetValue(propertyName  ?? "", out var errorsForProperty);
      return errorsForProperty ?? Enumerable.Empty<string>();
    }

    private void OnErrorsChanged(string propertyName) {
      ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
      this.NotifyPropertyChanged(() => this.ObjectErrors);
      this.isValid.Value = !this.HasErrors;

    }
  }
}
