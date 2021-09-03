using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HAF {
  public class IntegerExpressionToBooleanConverter: ValueConverter<int, bool> {
    public enum IntegerExpression {
      IsEqual,
      IsLargerThen,
      IsSmallerThen
    }

    public IntegerExpression Expression { get; set; } = IntegerExpression.IsEqual;
    public int Comparator { get; set; } = 0;
    public bool Inverted { get; set; } = false;

    protected override bool convert(int value) {
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
      return result;
    }
  }
}
