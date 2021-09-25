using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace HAF.Validation {
  public class IsEmailAddressValidationRule : BasePatternValidationRule {
    public IsEmailAddressValidationRule() {
      this.pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
      this.options = RegexOptions.IgnoreCase;
    }

    public override string Message {
      get {
        // default message
        if (this.message == null) {
          this.message = Localization.GetText("The value is not a valid email address.");
        }
        return this.message;
      }
    }
  }
}
