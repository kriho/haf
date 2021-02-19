using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using HAF.Localization;
using System.Text.RegularExpressions;

namespace HAF.Validation {

  public class IsPhoneNumberValidationRule : BasePatternValidationRule {
    public IsPhoneNumberValidationRule() {
      this.pattern = @"^+?[0-9/.\- ]{8,}$";
      this.options = RegexOptions.None;
    }

    public override string Message {
      get {
        // default message
        if (this.message == null) {
          this.message = Strings.Validation_IsPhoneNumber;
        }
        return this.message;
      }
    }
  }
}
