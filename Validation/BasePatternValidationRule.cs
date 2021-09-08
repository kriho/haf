using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace HAF.Validation {

  public abstract class BasePatternValidationRule: ValidationRule {
    protected string pattern = string.Empty;
    protected RegexOptions options = RegexOptions.None;
    protected string message;

    public virtual string Message {
      get { return this.message; }
      set { this.message = value; }
    }

    public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
      // start valid
      var result = new ValidationResult(true, null);
      // invalidate if needed
      var input = (value ?? string.Empty).ToString();
      var expression = new Regex(this.pattern, this.options);
      if(!expression.IsMatch(input)) {
        result = new ValidationResult(false, this.Message);
      }
      return result;
    }
  }
}
