using System;

namespace HAF {
  public interface ISetting<T>: ISettingsValueBase {
    T Value { get; set; }
    T DefaultValue { get; }
  }
}