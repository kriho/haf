using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace HAF.Validation {

  public class IsNumberValidationRule : ValidationRule {
    private bool digits;
    private string message;

    public bool Digits {
      get { return this.digits; }
      set { this.digits = value; }
    }

    public string Message {
      get {
        // default message
        if (this.message == null) {
          this.message = Localization.GetText("The value is not a number.");
        }
        return this.message;
      }
      set { this.message = value; }
    }

    public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
      // start valid
      var result = new ValidationResult(true, null);
      // invalidate if needed
      if (this.digits) {
        if (!double.TryParse(value.ToString(), out _)) {
          result = new ValidationResult(false, this.Message);
        }
      } else {
        if (!int.TryParse(value.ToString(), out _)) {
          result = new ValidationResult(false, this.Message);
        }
      }
      return result;
    }
  }
}
