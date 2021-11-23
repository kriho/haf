using System;

namespace HAF {
  public interface ISettingMeta {
    string Description { get; }
    string DisplayName { get; }
    Action<ValidationBatch> Validation { get; }
  }
}