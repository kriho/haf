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

    protected bool SetValueAndValidate<T>(ref T property, T value, Func<IEnumerable<string>> validation, bool subscribe = false, [CallerMemberName] string propertyName = "") {
      if (!this.SetValue(ref property, value, subscribe, propertyName)) {
        return false;
      }
      // perform validation
      lock(this.validationLock) {
        var validationErrors = validation.Invoke().ToList();
        if(this.errors.TryGetValue(propertyName, out var list)) {
          if(validationErrors.Count == 0) {
            if(list.Count > 0) {
              list.Clear();
              this.OnErrorsChanged(propertyName);
              this.NotifyPropertyChanged(() => this.HasErrors);
            }
          } else {
            list.Clear();
            list.AddRange(validationErrors);
            this.OnErrorsChanged(propertyName);
            this.NotifyPropertyChanged(() => this.HasErrors);
          }
        } else if(validationErrors.Count > 0) {
          _ = this.errors.TryAdd(propertyName, validationErrors);
          this.OnErrorsChanged(propertyName);
          this.NotifyPropertyChanged(() => this.HasErrors);
        }
      }
      return true;
    }

    protected bool SetValueAndValidate<T>(ref T property, T value, Func<IEnumerable<string>> validation, Expression<Func<T>> expression, bool subscribe = false) {
      return this.SetValueAndValidate(ref property, value, validation, subscribe, this.GetExpressionMemberName(expression));
    }

    private readonly ConcurrentDictionary<string, List<string>> errors = new ConcurrentDictionary<string, List<string>>();
    private readonly object validationLock = new object();

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    public bool HasErrors {
      get {
        return this.errors.Any(e => e.Value != null && e.Value.Count > 0);
      }
    }

    public IEnumerable GetErrors(string propertyName) {
      return this.errors.TryGetValue(propertyName, out var errorsForProperty) ? errorsForProperty : Enumerable.Empty<string>();
    }

    public void OnErrorsChanged(string propertyName) {
      ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }
  }
}
