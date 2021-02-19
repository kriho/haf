using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF.Converters {
  public class InvertNullableBoolean : ValueConverter<bool?, bool?> {

    protected override bool? convert(bool? value) {
      if (value == null) {
        return null;
      }
      return !value;
    }

    protected override bool? convertBack(bool? value) {
      return this.convert(value);
    }
  }
}
