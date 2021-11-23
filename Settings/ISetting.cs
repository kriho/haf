using System;

namespace HAF {
  public interface ISetting<T>: ISettingMeta {
    T Value { get; set; }
  }
}