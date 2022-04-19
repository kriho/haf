using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HAF {

  [Export(typeof(IInspectorService)), PartCreationPolicy(CreationPolicy.Shared)]
  public class InspectorService: Service, IInspectorService {

    public IRelayCommand<object> DoSetSelectedItem { get; private set; }

    public IRelayCommand DoClearSelectedItem { get; private set; }

    private Event<object> onSelectedItemAssigned = new Event<object>(nameof(OnSelectedItemAssigned));
    public IReadOnlyEvent<object> OnSelectedItemAssigned => this.onSelectedItemAssigned;

    private object selectedItem;
    public object SelectedItem {
      get => this.selectedItem;
      set {
        this.SetValue(ref this.selectedItem, value);
        this.onSelectedItemAssigned.Fire(value);
      }
    }

    private object selectedSection;
    public object SelectedSection {
      get => this.selectedSection;
      set {
        this.SetValue(ref this.selectedSection, value);
      }
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
