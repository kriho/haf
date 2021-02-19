using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using HAF.Localization;

namespace HAF.Validation {
  public class NumberRangeValidationRule : ValidationRule {
    private int minimum = 0;
    private int maximum = 0;
    private string message;

    public int Minimum {
      get { return this.minimum; }
      set { this.maximum = value; }
    }

    public int Maximum {
      get { return this.maximum; }
      set { this.maximum = value; }
    }

    public string Message {
      get {
        // default message
        if (this.message == null) {
          this.message = String.Format(Strings.Validation_NumberRange, this.minimum, this.maximum);
        }
        return this.message;
      }
      set { this.message = value; }
    }

    public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
      // valid
      var result = new ValidationResult(true, null);
      // invalidate if needed
      var input = double.Parse(value.ToString());
      if (input < this.minimum || (this.maximum > this.minimum && input > this.maximum)) {
        result = new ValidationResult(false, this.Message);
      }
      return result;
    }
  }
}
