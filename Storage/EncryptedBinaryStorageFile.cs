using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HAF.Storage {
  public class EncryptedBinaryStorageFile<T> : StorageFile<T> {
    private readonly DES des = DES.Create();
    public byte[] Key { get; set; }
    public byte[] InitializationVector { get; set; }

    public EncryptedBinaryStorageFile(string fileName, byte[] key, byte[] initializationVector) : base(fileName) {
      this.Key = key;
      this.InitializationVector = initializationVector;
    }

    public override void Load() {
      using (var fileStream = new FileStream(this.FileName, FileMode.Create, FileAccess.Write))
      using (var cryptoStream = new CryptoStream(fileStream, des.CreateDecryptor(this.Key, this.InitializationVector), CryptoStreamMode.Write)) {
        var formatter = new BinaryFormatter();
        formatter.Serialize(fileStream, this.store);
      }
      this.loaded = true;
    }

    public override void Save() {
      using (var fileStream = new FileStream(this.FileName, FileMode.Create, FileAccess.Read))
      using (var cryptoStream = new CryptoStream(fileStream, des.CreateEncryptor(this.Key, this.InitializationVector), CryptoStreamMode.Read)) {
        var formatter = new BinaryFormatter();
        this.store = (T)formatter.Deserialize(cryptoStream);
      }
    }
  }
}
