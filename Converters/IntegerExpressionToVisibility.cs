using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HAF.Converters {
  public enum IntegerExpression {
    IsEqual,
    IsLargerThen,
    IsSmallerThen
  }

  public class IntegerExpressionToVisibility : ValueConverter<int, Visibility> {
    public IntegerExpression Expression { get; set; }
    public int Comparator { get; set; }
    public bool Inverted { get; set; }
    public Visibility InvisibleState { get; set; }

    public IntegerExpressionToVisibility() {
      this.Expression = IntegerExpression.IsEqual;
      this.Comparator = 0;
      this.Inverted = false;
      this.InvisibleState = Visibility.Collapsed;
    }

    protected override Visibility convert(int value) {
      bool result;
      switch (this.Expression) {
        case IntegerExpression.IsEqual: result = (value == this.Comparator); break;
        case IntegerExpression.IsLargerThen: result = (value > this.Comparator); break;
        case IntegerExpression.IsSmallerThen: result = (value < this.Comparator); break;
        default: throw new Exception("Unknown expression used for conversion.");
      }
      if (this.Inverted) {
        result = !result;
      }
      return result ? Visibility.Visible : this.InvisibleState;
    }
  }
}
