using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HAF.Converters {
  public class UnsignedIntegerExpressionToVisibilityConverter: ValueConverter<uint, Visibility> {
    public IntegerExpression Expression { get; set; } = IntegerExpression.IsEqual;
    public uint Comparator { get; set; } = 0;
    public bool Inverted { get; set; } = false;
    public Visibility InvisibleState { get; set; } = Visibility.Collapsed;

    protected override Visibility convert(uint value) {
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