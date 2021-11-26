using System;

namespace HAF {
  public interface ISettingString: ISetting<string> {
    string Mask { get; }
  }
}