using System;

namespace HAF {
  public interface ISettingString: ISetting<string> {
    /// <summary>
    /// Mask for describing allowed strings. Can be used by the drawer to improve visualization and validation.
    /// </summary>
    string Mask { get; }
  }
}