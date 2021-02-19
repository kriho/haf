using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using HAF.Localization;

namespace HAF.Validation {
  public class StringRangeValidationRule : ValidationRule {
    private int minimum = -1;
    private int maximum = -1;
    private string message;

    public int MinimumLength {
      get { return this.minimum; }
      set { this.maximum = value; }
    }

    public int MaximumLength {
      get { return this.maximum; }
      set { this.maximum = value; }
    }

    public string Message {
      get {
        // default message
        if (this.message == null) {
          this.message = String.Format(Strings.Validation_StringRange, this.minimum, this.maximum);
        }
        return this.message;
      }
      set { this.message = value; }
    }

    public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
      // start valid
      var result = new ValidationResult(true, null);
      // invalidate if needed
      var input = (value ?? string.Empty).ToString();
      if (input.Length < this.minimum || (this.maximum > this.minimum && input.Length > this.maximum)) {
        result = new ValidationResult(false, this.Message);
      }
      return result;
    }
  }
}
