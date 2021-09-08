using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace HAF.Validation {
  internal class StringPatternValidationRule : BasePatternValidationRule {
    public string Pattern {
      get { return this.pattern; }
      set { this.pattern = value; }
    }

    public RegexOptions Options {
      get { return this.options; }
      set { this.options = value; }
    }

    public override string Message {
      get {
        // default message
        if (this.message == null) {
          this.message = Configuration.LocalizationService.GetText("The value is has an incorrect format.");
        }
        return this.message;
      }
    }
  }
}
