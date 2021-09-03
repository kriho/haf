using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HAF {
  public class DoubleExpressionToVisibilityConverter: ValueConverter<double, Visibility> {
    public enum DoubleExpression {
      IsEqual,
      IsLargerThen,
      IsSmallerThen,
      IsNAN
    }

    public DoubleExpression Expression { get; set; } = DoubleExpression.IsEqual;
    public int Comparator { get; set; } = 0;
    public bool Inverted { get; set; } = false;
    public Visibility InvisibleState { get; set; } = Visibility.Collapsed;

    protected override Visibility convert(double value) {
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
      return result ? Visibility.Visible : this.InvisibleState;
    }
  }
}
