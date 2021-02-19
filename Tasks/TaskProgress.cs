using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public class TaskProgress : ObservableObject {

    private int maximum;
    public int Maximum {
      get {
        if (this.normalizer.HasValue) {
          return this.normalizer.Value;
        } else {
          return this.maximum;
        }
      }
      set { this.SetValue(ref this.maximum, value); }
    }

    private int value;
    public int Value {
      get {
        if (this.normalizer.HasValue) {
          if (this.maximum == 0) {
            return 0;
          } else {
            return this.value * this.normalizer.Value / this.maximum;
          }
        } else {
          return this.value;
        }
      }
      set { this.SetValue(ref this.value, value); }
    }

    private bool isIndeterminate;
    public bool IsIndeterminate {
      get { return this.isIndeterminate; }
      set { this.SetValue(ref this.isIndeterminate, value); }
    }

    private string description;
    public string Description {
      get { return this.description; }
      set { this.SetValue(ref this.description, value); }
    }

    private int? normalizer;
    public int? Normalizer {
      get { return this.normalizer; }
      set { this.SetValue(ref this.normalizer, value); }
    }

    public TaskProgress() {
      this.Description = "";
      this.IsIndeterminate = true;
      this.Maximum = 0;
      this.Value = 0;
      this.normalizer = null;
    }

    public TaskProgress(string description, int maximum = 0, int value = 0) {
      this.description = description;
      this.isIndeterminate = (maximum == value);
      this.maximum = maximum;
      this.value = value;
      this.normalizer = null;
    }

    public void IncreaseValue(int value) {
      this.value += value;
      this.NotifyPropertyChanged(() => this.Value);
    }
  }
}
