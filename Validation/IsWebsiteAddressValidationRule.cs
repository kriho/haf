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
  public class IsWebsiteAddressValidationRule : BasePatternValidationRule {
    public IsWebsiteAddressValidationRule() {
      this.pattern = @"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$";
      this.options = RegexOptions.IgnoreCase;
    }

    public override string Message {
      get {
        // default message
        if (this.message == null) {
          this.message = Strings.Validation_IsWebsiteAddress;
        }
        return this.message;
      }
    }
  }
}
