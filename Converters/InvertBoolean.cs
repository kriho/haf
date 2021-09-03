using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public class InvertBooleanConverter: ValueConverter<bool, bool> {

    protected override bool convert(bool value) {
      return !value;
    }
  }
}
