using System;

namespace HAF {
  public interface ISetting<T>: ISettingsValueBase {
    /// <summary>
    /// Current setting value.
    /// </summary>
    T Value { get; set; }

    /// <summary>
    /// Default value of the setting.
    /// </summary>
    T DefaultValue { get; }
  }
}