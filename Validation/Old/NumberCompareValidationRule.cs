using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HAF.Validation {
  public enum NumberCompareOperator {
    Larger,
    LargerOrEqual,
    Smaller,
    SmallerOrEqual
  }

  public class NumberCompareValidationRule : ValidationRule {
    private NumberCompareOperator comparison = NumberCompareOperator.Larger;
    private int refrence = 0;
    private string message;

    public NumberCompareOperator Comparison {
      get { return this.comparison; }
      set { this.comparison = value; }
    }

    public int Refrence {
      get { return this.refrence; }
      set { this.refrence = value; }
    }

    public string Message {
      get {
        // default message
        if (this.message == null) {
          var compareMessage = new string[] { "larger then", "larger then or equal to", "smaller then", "smaller then or equal to" };
          this.message = string.Format(Localization.GetText("Validation Rule", "The value has to be {0} {1}."), compareMessage[(int)this.comparison], this.refrence);
        }
        return this.message;
      }
      set { this.message = value; }
    }

    public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
      // return valid cases
      var input = double.Parse(value.ToString());
      if (this.comparison == NumberCompareOperator.Larger && input > this.refrence) {
        return new ValidationResult(true, null);
      } else if (this.comparison == NumberCompareOperator.LargerOrEqual && input >= this.refrence) {
        return new ValidationResult(true, null);
      } else if (this.comparison == NumberCompareOperator.Smaller && input < this.refrence) {
        return new ValidationResult(true, null);
      } else if (this.comparison == NumberCompareOperator.SmallerOrEqual && input <= this.refrence) {
        return new ValidationResult(true, null);
      }
      // return invalid
      return new ValidationResult(false, this.Message); ;
    }
  }
}
