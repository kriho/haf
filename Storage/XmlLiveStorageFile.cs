using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF.Storage {
  public class XmlLiveStorageFile<T> : XmlStorageFile<T> where T : INotifyPropertyChanged {
    public XmlLiveStorageFile(string fileName) : base(fileName) {
      this.Store.PropertyChanged += Store_PropertyChanged;
    }

    void Store_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      this.Save();
    }
  }
}
