using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public class PropertyValidationBatch {
    internal List<string> Errors = new List<string>();

    /// <summary>
    /// Throw a validation error.
    /// </summary>
    /// <param name="error">Validation error description.</param>
    public void Throw(string error) {
      this.Errors.Add(error);
    }
  }
}
