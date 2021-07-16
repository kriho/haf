using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {

  [Export(typeof(IInspectorService)), PartCreationPolicy(CreationPolicy.Shared)]
  public class InspectorService: Service, IInspectorService {

    public RelayCommand<object> DoSetSelectedItem { get; private set; }

    public RelayCommand DoClearSelectedItem { get; private set; }

    private object selectedItem;
    public object SelectedItem {
      get { return this.selectedItem; }
      set { this.SetValue(ref this.selectedItem, value); }
    }

    public InspectorService() {
      this.DoSetSelectedItem = new RelayCommand<object>(obj => {
        this.SelectedItem = obj;
      });
      this.DoClearSelectedItem = new RelayCommand(() => {
        this.SelectedItem = null;
      });
    }
  }
}
