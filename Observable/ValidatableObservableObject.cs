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
  public abstract class ValidatableObservableObject: ObservableObject, INotifyDataErrorInfo {

    private ValidationBatch validationBatch = new ValidationBatch();
    private readonly object validationLock = new object();
    private readonly ConcurrentDictionary<string, List<string>> validationErrors = new ConcurrentDictionary<string, List<string>>();

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

    protected bool SetValueAndValidate<T>(ref T property, T value, Action<ValidationBatch> batch, bool subscribe = false, [CallerMemberName] string propertyName = "") {
      if (!this.SetValue(ref property, value, subscribe, propertyName)) {
        return false;
      }
      this.ValidateBatch(propertyName, batch);
      return true;
    }

    protected bool SetValueAndValidate<T>(ref T property, T value, Action<ValidationBatch> batch, Expression<Func<T>> expression, bool subscribe = false) {
      return this.SetValueAndValidate(ref property, value, batch, subscribe, this.GetExpressionMemberName(expression));
    }

    protected bool SetValueAndValidateObject<T>(ref T property, T value, bool subscribe = false, [CallerMemberName] string propertyName = "") {
      if(!this.SetValue(ref property, value, subscribe, propertyName)) {
        return false;
      }
      this.ValidateBatch("", this.ValidateObject);
      return true;
    }

    protected bool SetValueAndValidateObject<T>(ref T property, T value, Expression<Func<T>> expression, bool subscribe = false) {
      return this.SetValueAndValidateObject(ref property, value, subscribe, this.GetExpressionMemberName(expression));
    }

    protected virtual void ValidateObject(ValidationBatch batch) {
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
