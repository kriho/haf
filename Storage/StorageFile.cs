using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HAF.Storage {
  public abstract class StorageFile<T> {
    protected bool loaded = false;

    public bool Loaded {
      get { return this.loaded; }
    }

    public string FileName { get; private set; }

    protected T store;
    public T Store {
      get {
        // load if needed
        if (!this.loaded) {
          this.Load();
        }
        return this.store;
      }
    }

    private string FilterFileName(string fileName) {
      return fileName
          .Trim()
          .Replace("\\", "")
          .Replace("/", "")
          .Replace("|", "")
          .Replace(":", "")
          .Replace("*", "")
          .Replace("?", "")
          .Replace("\"", "")
          .Replace("<", "")
          .Replace(">", "")
          .Replace(" ", "");
    }

    public StorageFile(string fileName) {
      this.FileName = this.FilterFileName(fileName);
    }

    public abstract void Load();

    public Task LoadAsync() {
      return Task.Run(() => {
        this.Load();
      });
    }

    public abstract void Save();

    public Task SaveAsync() {
      return Task.Run(() => {
        this.Save();
      });
    }
  }
}
