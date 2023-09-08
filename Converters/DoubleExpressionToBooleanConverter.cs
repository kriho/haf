using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HAF.Converters {
  public class DoubleExpressionToBooleanConverter: ValueConverter<double, bool> {
    public enum DoubleExpression {
      IsEqual,
      IsLargerThen,
      IsSmallerThen,
      IsNAN
    }

    public DoubleExpression Expression { get; set; } = DoubleExpression.IsEqual;
    public int Comparator { get; set; } = 0;
    public bool Inverted { get; set; } = false;

    protected override bool convert(double value) {
      bool result;
      switch (this.Expression) {
        case DoubleExpression.IsEqual: result = (value == this.Comparator); break;
        case DoubleExpression.IsLargerThen: result = (value > this.Comparator); break;
        case DoubleExpression.IsSmallerThen: result = (value < this.Comparator); break;
        case DoubleExpression.IsNAN: result = double.IsNaN(value); break;
        default: throw new Exception("Unknown expression used for conversion.");
      }
      if (this.Inverted) {
        result = !result;
      }
      return result;
    }

    protected override double convertBack(bool value) {
      if(this.Expression != DoubleExpression.IsEqual) {
        throw new InvalidOperationException("Back-conversion is only supported when the expression is 'IsEquel'.");
      }
      if(value) {
        return this.Comparator;
      }
      return 0.0;
    }
  }
}