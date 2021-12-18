using HAF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HAF {
  public interface IWindowLayoutService : IService {
    IWindowLayout ActiveWindowLayout { get; set; }
    IReadOnlyObservableCollection<IPaneMeta> AvailablePanes { get; }
    ICompoundState CanChangeWindowLayout { get; }
    IWindowLayout DefaultWindowLayout { get; set; }
    IReadOnlyCollection<IWindowLayout> DefaultWindowLayouts { get; }
    IReadOnlyEvent OnActiveWindowLayoutChanged { get; }
    IRelayCommand<IWindowLayout> DoDeleteWindowLayout { get; }
    IRelayCommand<IWindowLayout> DoLoadWindowLayout { get; }
    IRelayCommand<IWindowLayout> DoSaveWindowLayout { get; }
    IRelayCommand<IWindowLayout> DoSetDefaultWindowLayout { get; }
    IRelayCommand<IPaneMeta> DoShowPane { get; }
    IRelayCommand DoAddWindowLayout { get; }
    IReadOnlyObservableCollection<IWindowLayout> WindowLayouts { get; }
    string EditName { get; set; }
    void AddWindowLayout(string name);
    void AddDefaultWindowLayout(string name, string layout);
    void RegisterAvailablePane(string name, Type type, bool canUserClose = true);
  }
}