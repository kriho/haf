using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HAF {
  public class StringExpressionToBooleanConverter: ValueConverter<string, bool> {
    public enum StringExpression {
      IsEqual,
      Contains,
    }

    public StringExpression Expression { get; set; } = StringExpression.IsEqual;
    public string Comparator { get; set; } = string.Empty;
    public bool Inverted { get; set; } = false;

    protected override bool convert(string value) {
      bool result;
      switch (this.Expression) {
        case StringExpression.IsEqual:
          result = this.Comparator == string.Empty ? string.IsNullOrEmpty(value) : value == this.Comparator;
          break;
        case StringExpression.Contains:
          result = (value.Contains(this.Comparator)); break;
        default:
          throw new Exception("Unknown expression used for conversion.");
      }
      if (this.Inverted) {
        result = !result;
      }
      return result;
    }
  }
}
