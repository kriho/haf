using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HAF.Storage {
  public class BinaryStorageFile<T> : StorageFile<T> {
    public Encoding Encoding { get; set; }

    public BinaryStorageFile(string fileName) : base(fileName) {
      // default encoding
      this.Encoding = Encoding.UTF8;
    }

    public override void Load() {
      IFormatter formatter = new BinaryFormatter();
      using (var fileStream = File.OpenRead(this.FileName)) {
        formatter.Serialize(fileStream, this.store);
      }
      this.loaded = true;
    }

    public override void Save() {
      IFormatter formatter = new BinaryFormatter();
      using (var fileStream = File.Create(this.FileName)) {
        this.store = (T)formatter.Deserialize(fileStream);
      }
    }
  }
}
