using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HAF.Converters {
  public class DoubleExpressionToVisibility : ValueConverter<double, Visibility> {
    public enum DoubleExpression {
      IsEqual,
      IsLargerThen,
      IsSmallerThen,
      IsNAN
    }

    public DoubleExpression Expression { get; set; }
    public int Comparator { get; set; }
    public bool Inverted { get; set; }
    public Visibility InvisibleState { get; set; }

    public DoubleExpressionToVisibility() {
      this.Expression = DoubleExpression.IsEqual;
      this.Comparator = 0;
      this.Inverted = false;
      this.InvisibleState = Visibility.Collapsed;
    }

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
