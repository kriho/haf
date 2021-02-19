using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HAF.Updates {
  public class SnapshotTask {
    public enum TaskId {
      LaunchApplication = 1,
      ExecuteFile
    }

    public enum TaskExecution {
      Success = 1,
      Failure,
      Allways
    }

    [XmlAttribute("id")]
    public TaskId Id { get; set; }

    [XmlAttribute("execution")]
    public TaskExecution Execution { get; set; }

    [XmlArray("files")]
    [XmlArrayItem("file")]
    public List<string> Files { get; set; }

    [XmlAttribute("option")]
    public string Option { get; set; }

    public bool ShouldSerializeFiles() {
      return this.Files.Count > 0;
    }

    public bool ShouldSerializeOption() {
      return !String.IsNullOrWhiteSpace(this.Option);
    }

    public bool ShouldSerializeExecution() {
      return this.Execution != TaskExecution.Success;
    }

    public SnapshotTask() {
      this.Files = new List<string>();
      this.Option = String.Empty;
      this.Execution = TaskExecution.Success;
    }
  }
}
