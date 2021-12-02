using System;
using System.ComponentModel;
using System.Windows;

namespace HAF {
  public interface ISettingsValueBase: INotifyPropertyChanged {
    string DisplayName { get; }
    string Description { get; }
    Action<ValidationBatch> Validation { get; }
    Action<ServiceConfigurationEntry> SaveSetting { get; }
    Action<ServiceConfigurationEntry> LoadSetting { get; }
  }
}