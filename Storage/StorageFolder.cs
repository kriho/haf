using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF.Core.Storage
{
    public abstract class StorageFolder<T>
    {
        public ObservableCollection<T> Stores;

        protected bool loaded = false;
        public bool Loaded
        {
            get { return this.loaded; }
        }

        public string Path { get; private set; }

        public StorageFolder(string path)
        {
            this.Path = path;
            this.Stores = new ObservableCollection<T>();
        }

        public abstract void Load();

        public Task LoadAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                this.Load();
            });
        }

        public abstract void Save();

        public Task DaveAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                this.Save();
            });
        }
    }
}
