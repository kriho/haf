using System;

namespace HAF {
  public interface IStringSetting: ISetting<string> {
    string Mask { get; }
  }
}