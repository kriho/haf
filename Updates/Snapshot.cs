using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HAF.Updates {
  public class Snapshot {
    public enum DirectoryOrder {
      ShallowToDeep,
      DeepToShallow
    }

    public const string FileName = "snapshot.xml";

    [XmlElement("versionId")]
    public int VersionId { get; set; }

    [XmlElement("versionPath")]
    public string VersionPath { get; set; }

    [XmlElement("applicationId")]
    public int ApplicationId { get; set; }

    [XmlElement("applicationPath")]
    public string ApplicationPath { get; set; }

    [XmlElement("description")]
    public string Description { get; set; }

    [XmlArray("files")]
    [XmlArrayItem("file")]
    public List<SnapshotFile> Files { get; set; }

    [XmlArray("tasks")]
    [XmlArrayItem("task")]
    public List<SnapshotTask> Tasks { get; set; }

    public Snapshot() {
      this.Files = new List<SnapshotFile>();
      this.Tasks = new List<SnapshotTask>();
    }

    private void SearchSubDirectories(string directory, char seperator, ref List<string> subs) {
      if (directory.Contains(seperator)) {
        var sub = directory.Substring(0, directory.LastIndexOf(seperator));
        this.SearchSubDirectories(sub, seperator, ref subs);
        if (!subs.Contains(sub)) {
          subs.Add(sub);
        }
      }
    }

    public List<string> GetRemoteDirectories(DirectoryOrder order = DirectoryOrder.ShallowToDeep) {
      var subs = new List<string>();
      foreach (var file in this.Files) {
        this.SearchSubDirectories(file.RemotePath, '/', ref subs);
      }
      // sort by depth
      if (order == DirectoryOrder.ShallowToDeep) {
        subs = subs.OrderBy(s => s.Count(c => c == '/')).ToList();
      } else {
        subs = subs.OrderByDescending(s => s.Count(c => c == '/')).ToList();
      }
      return subs;
    }

    public List<string> GetLocalDirectories(DirectoryOrder order = DirectoryOrder.ShallowToDeep) {
      var subs = new List<string>();
      foreach (var file in this.Files) {
        this.SearchSubDirectories(file.LocalPath, Path.DirectorySeparatorChar, ref subs);
      }
      // sort by depth
      if (order == DirectoryOrder.ShallowToDeep) {
        subs = subs.OrderBy(s => s.Count(c => c == Path.DirectorySeparatorChar)).ToList();
      } else {
        subs = subs.OrderByDescending(s => s.Count(c => c == Path.DirectorySeparatorChar)).ToList();
      }
      return subs;
    }

    public static Snapshot FromFile(string file) {
      using (var fileStream = File.OpenRead(file)) {
        var serializer = new XmlSerializer(typeof(Snapshot));
        return (Snapshot)serializer.Deserialize(fileStream);
      }
    }

    public bool ShouldSerializeTasks() {
      return this.Tasks.Count > 0;
    }
  }
}
