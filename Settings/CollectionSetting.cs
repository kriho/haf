using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public class CollectionSetting<T>: ICollectionSetting<T> {
    public string DisplayName { get; set; }

    public string Description { get; set; }

    public IObservableCollection<T> Collection { get; set; } = new ObservableCollection<T>();

    public Action<ValidationBatch> Validation { get; set; }
  }
}
