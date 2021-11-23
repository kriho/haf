using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {

  public class ValidationBatch {
    internal List<string> Errors = new List<string>();
    internal string PropertyName;

    public void Assert(bool assertion, string error) {
      if(!assertion) {
        this.Errors.Add(error);
      }
    }

    public void Throw(string error) {
      this.Errors.Add(error);
    }
  }
}
