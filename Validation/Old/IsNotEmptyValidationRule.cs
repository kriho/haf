using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HAF.Validation {

  public class IsNotEmptyValidationRule : ValidationRule {
    private string message;

    public string Message {
      get {
        // default message
        if (this.message == null) {
          this.message = Localization.GetText("This field is mandatory.");
        }
        return this.message;
      }
      set { this.message = value; }
    }

    public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
      // valid
      var result = new ValidationResult(true, null);
      // invalidate if needed
      if (string.IsNullOrWhiteSpace(value.ToString())) {
        result = new ValidationResult(false, this.Message);
      }
      return result;
    }
  }
}
