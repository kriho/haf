using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HAF.Storage {
  public class XmlStorageFile<T> : StorageFile<T> {
    public override void Load() {
      try {
        using (var fileStream = File.OpenRead(this.FileName)) {
          var serializer = new XmlSerializer(typeof(T));
          this.store = (T)serializer.Deserialize(fileStream);
        }
      } catch {
        this.store = Activator.CreateInstance<T>();
      }
      this.loaded = true;
    }

    public override void Save() {
      using (var fileStream = File.Create(this.FileName)) {
        var serializer = new XmlSerializer(typeof(T));
        serializer.Serialize(fileStream, this.store);
      }
    }

    public XmlStorageFile(string fileName) : base(fileName) { }
  }
}
