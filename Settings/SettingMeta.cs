using System;

namespace HAF {
  public class SettingMeta: ISettingMeta {
    public string Description { get; set; }
    public string DisplayName { get; set; }
    public Action<ValidationBatch> Validation { get; set; }
  }
}