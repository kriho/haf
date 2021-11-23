using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public class Setting<T>: ObservableObject, ISetting<T> {
    public string DisplayName { get; set; }

    public string Description { get; set; }

    private T value;
    public T Value {
      get { return this.value; }
      set { this.SetValue(ref this.value, value); }
    }

    public Action<ValidationBatch> Validation { get; set; }
  }
}
