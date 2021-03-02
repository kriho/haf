﻿using HAF.Models;
using System.Collections.ObjectModel;

namespace HAF {
  public interface IWindowLayoutService : IService {
    WindowLayout ActiveWindowLayout { get; set; }
    ObservableCollection<PaneMeta> AvailablePanes { get; }
    LinkedDependency MayChangeWindowLayout { get; }
    WindowLayout DefaultWindowLayout { get; set; }
    RangeObservableCollection<WindowLayout> DefaultWindowLayouts { get; }
    RelayCommand<WindowLayout> DeleteCommand { get; }
    RelayCommand<WindowLayout> LoadCommand { get; }
    LinkedEvent OnActiveWindowLayoutChanged { get; }
    RelayCommand<WindowLayout> SaveCommand { get; }
    RelayCommand<WindowLayout> SetDefaultCommand { get; }
    RelayCommand<PaneMeta> ShowPaneCommand { get; }
    RangeObservableCollection<WindowLayout> WindowLayouts { get; }

    void AddWindowLayout(string name);
  }
}