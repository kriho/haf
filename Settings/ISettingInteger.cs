using System;

namespace HAF {
  public interface ISettingInteger: ISetting<int> {
    int? Min { get; }
    int? Max { get; }
    string Unit { get; }
  }
}