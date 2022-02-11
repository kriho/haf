using System;

namespace HAF {
  public interface ISettingInteger: ISetting<int> {
    /// <summary>
    /// Minimal allowed value (inclusive) of the setting. Can be used by the drawer to improve visualization and validation.
    /// </summary>
    int Min { get; }

    /// <summary>
    /// Maximal allowed value (inclusive) of the setting. Can be used by the drawer to improve visualization and validation.
    /// </summary>
    int Max { get; }

    /// <summary>
    /// Unit of the setting. Can be used by the drawer to improve visualization.
    /// </summary>
    string Unit { get; }
  }
}