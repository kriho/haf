using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HAF {
  public class Setting<T>: ObservableObject, ISetting<T> {
    public string DisplayName { get; set; }

    public string Description { get; set; }

    public Action<ValidationBatch> Validation { get; set; }

    public Action<ServiceConfigurationEntry> SaveSetting { get; set; }

    public Action<ServiceConfigurationEntry> LoadSetting { get; set; }

    public FrameworkElement Drawer { get; set; }

    private T value;
    public T Value {
      get { return this.value; }
      set { this.SetValue(ref this.value, value); }
    }

    public T DefaultValue { get; set; }
  }
}
