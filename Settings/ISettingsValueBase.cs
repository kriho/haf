using System;
using System.ComponentModel;
using System.Windows;

namespace HAF {
  public interface ISettingsValueBase: INotifyPropertyChanged {
    /// <summary>
    /// The display name of the settings value.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// The description of the settings value.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// A validation action that is used to validate the current value. No validation is performed when NULL.
    /// </summary>
    Action<ValidationBatch> Validation { get; }
  }
}