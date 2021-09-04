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
  public abstract class ValidatableView: View, INotifyDataErrorInfo {

    protected bool SetValueAndValidate<T>(ref T property, T value, bool subscribe = false, [CallerMemberName] string propertyName = "") {
      if (!this.SetValue(ref property, value, subscribe, propertyName)) {
        return false;
      }
      // perform validation
      this.ValidateProperty(value, propertyName);
      return true;
    }

    protected bool SetValueAndValidate<T>(ref T property, T value, Expression<Func<T>> expression, bool subscribe = false) {
      return this.SetValueAndValidate(ref property, value, subscribe, this.GetExpressionMemberName(expression));
    }

    private ConcurrentDictionary<string, List<string>> errors = new ConcurrentDictionary<string, List<string>>();
    private object validationLock = new object();

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    public bool HasErrors {
      get {
        return this.errors.Any(e => e.Value != null && e.Value.Count > 0);
      }
    }

    public IEnumerable GetErrors(string propertyName) {
      this.errors.TryGetValue(propertyName, out List<string> errorsForProperty);
      return errorsForProperty;
    }

    public void OnErrorsChanged(string propertyName) {
      ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    protected void ValidateProperty(object value, [CallerMemberName] string propertyName = "") {
      lock (this.validationLock) {
        // remove old validation errors
        if (this.errors.ContainsKey(propertyName)) {
          this.errors.TryRemove(propertyName, out List<string> list);
        }
        // get error description
        var propertyInfo = this.GetType().GetProperty(propertyName);
        var validationErrors = (from validationAttribute in propertyInfo.GetCustomAttributes(true).OfType<ValidationAttribute>()
                                where !validationAttribute.IsValid(value)
                                select validationAttribute.FormatErrorMessage(string.Empty)).ToList();
        // add to errors
        this.errors.TryAdd(propertyName, validationErrors);
        this.OnErrorsChanged(propertyName);
        this.NotifyPropertyChanged("HasErrors");
      }
    }

    protected Task ValidatePropertyAsync(object value, [CallerMemberName] string propertyName = "") {
      return Task.Run(() => this.ValidateProperty(value, propertyName));
    }

    protected void Validate() {
      lock (this.validationLock) {
        var validationContext = new ValidationContext(this, null, null);
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(this, validationContext, validationResults, true);
        // clear old validation errors
        foreach (var error in this.errors.ToList()) {
          if (validationResults.All(result => result.MemberNames.All(member => member != error.Key))) {
            this.errors.TryRemove(error.Key, out var outLi);
            this.OnErrorsChanged(error.Key);
          }
        }
        // group results by membery
        var q = from result in validationResults
                from member in result.MemberNames
                group result by member into groups
                select groups;
        foreach (var prop in q) {
          var messages = prop.Select(r => r.ErrorMessage).ToList();
          if (this.errors.ContainsKey(prop.Key)) {
            this.errors.TryRemove(prop.Key, out var outLi);
          }
          this.errors.TryAdd(prop.Key, messages);
          OnErrorsChanged(prop.Key);
        }
      }
    }

    protected Task ValidateAsync() {
      return Task.Run(() => Validate());
    }
  }
}
