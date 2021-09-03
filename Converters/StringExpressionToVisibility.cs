using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HAF {
  public class StringExpressionToVisibilityConverter: ValueConverter<string, Visibility> {
    public enum StringExpression {
      IsEqual,
      Contains,
      IsEmpty
    }

    public StringExpression Expression { get; set; } = StringExpression.IsEqual;
    public string Comparator { get; set; } = string.Empty;
    public bool Inverted { get; set; } = false;
    public Visibility InvisibleState { get; set; } = Visibility.Collapsed;

    protected override Visibility convert(string value) {
      bool result;
      switch (this.Expression) {
        case StringExpression.IsEqual:
          result = this.Comparator == string.Empty ? string.IsNullOrEmpty(value) : value == this.Comparator;
          break;
        case StringExpression.Contains:
          result = (value.Contains(this.Comparator)); break;
        case StringExpression.IsEmpty:
          result = String.IsNullOrWhiteSpace(value); break;
        default:
          throw new Exception("Unknown expression used for conversion.");
      }
      if (this.Inverted) {
        result = !result;
      }
      return result ? Visibility.Visible : this.InvisibleState;
    }
  }
}
