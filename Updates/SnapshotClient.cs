﻿using HAF.Core;
using HAF.Ftp;
using HAF.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Xml.Serialization;

namespace HAF.Updates {
  public class SnapshotClient : ObservableObject {
    public const string UpdateControllerName = "UpdateController.exe";

    #region messages

    public enum Messages {
      // session starts
      StartDownloadHead,
      StartDownloadSnapshot,
      StartUploadSnapshot,
      StartCreateLocalSnapshot,
      StartReadLocalSnapshot,

      // error messages
      SnapshotDeserializationFailed,
      SnapshotSerializationFailed,
      InvalidSnapshot,
      VerifiedUploadImpossible,
      LocalDirectoryInvalid,
      FolderCreationFailed,
      VerifiedDownloadImpossible,

      // ftp client message
      FtpClientError,
      FtpClientWarning,
      FtpClientInformation
    }

    public string GetMessageText(Messages message) {
      switch (message) {
        case Messages.StartDownloadHead: return "Downloading head snapshot to stream...";
        case Messages.StartDownloadSnapshot: return "Downloading snapshot...";
        case Messages.StartUploadSnapshot: return "Uploading snapshot...";
        case Messages.StartCreateLocalSnapshot: return "Creating local snapshot...";
        case Messages.StartReadLocalSnapshot: return "Reading local snapshot...";

        case Messages.SnapshotDeserializationFailed: return "Deserializing the snapshot failed!";
        case Messages.SnapshotSerializationFailed: return "Serializing the snapshot failed!";
        case Messages.InvalidSnapshot: return "The snapshot is invalid!";
        case Messages.VerifiedUploadImpossible: return "Verified upload failed!";
        case Messages.LocalDirectoryInvalid: return "The local directory is invalid!";
        case Messages.FolderCreationFailed: return "Local folder creation failed!";
        case Messages.VerifiedDownloadImpossible: return "Verified download failed!";

        default: return "An unidentified error ocurred!";
      }
    }

    public ReportSeverity GetMessageType(Messages message) {
      switch (message) {
        case Messages.StartDownloadHead:
        case Messages.StartDownloadSnapshot:
        case Messages.StartUploadSnapshot:
        case Messages.StartCreateLocalSnapshot:
        case Messages.StartReadLocalSnapshot:
          return ReportSeverity.Information;
        default:
          return ReportSeverity.Error;
      }
    }

    #endregion

    public FtpClient FtpClient { get; private set; }
    public Reporter<string, Messages> Reporter { get; private set; }
    public readonly bool UpdateInstalled;
    public readonly bool UpdateSuccess;

    public SnapshotClient(string host, string username, string password) {
      this.Reporter = new Reporter<string, Messages>("SnapshotClient", this.GetMessageText, this.GetMessageType);
      this.FtpClient = new FtpClient(host, username, password);
      // pass up messages from the ftp client
      this.FtpClient.Reporter.MessageReported += (message) => {
        if (message.Type == ReportSeverity.Error) {
          this.Reporter.Report(message.Type, Messages.FtpClientError, message.Message);
        } else if (message.Type == ReportSeverity.Information) {
          this.Reporter.Report(message.Type, Messages.FtpClientInformation, message.Message);
        } else {
          this.Reporter.Report(message.Type, Messages.FtpClientWarning, message.Message);
        }
      };
      if (Utils.HasCommandLineArgument("update")) {
        this.UpdateInstalled = true;
        this.UpdateSuccess = Utils.GetCommandLineArgument<bool>("update");
      }
      // delete controller if update was successfull
      if (this.UpdateInstalled && this.UpdateSuccess) {
        this.RemoveLocalUpdateController();
      }
    }

    public async Task<Snapshot> DownloadHeadSnapshot(string applicationPath, CancellationToken cancellationToken, IObservableTaskProgress progress) {
      Snapshot snapshot = null;
      this.Reporter.StartSession(Messages.StartDownloadHead);
      progress.ReportIndeterminate(this.GetMessageText(Messages.StartDownloadHead));
      using (var stream = await this.FtpClient.DownloadToStream(FtpClient.CombinePath(applicationPath, Snapshot.FileName), cancellationToken, progress: progress)) {
        if (this.Reporter.HasErrors) {
          return null;
        }
        try {
          // memory stream position needs to be set to start of stream
          // http://stackoverflow.com/questions/15971928/deserialize-from-memorystream-issue
          stream.Seek(0, SeekOrigin.Begin);
          var serializer = new XmlSerializer(typeof(Snapshot));
          snapshot = (Snapshot)serializer.Deserialize(stream);
        } catch {
          this.Reporter.Report(Messages.SnapshotDeserializationFailed);
        }
        return snapshot;
      }
    }

    private void SearchDirectories(string directory, ref List<string> subs, char seperator) {
      if (directory.Contains(seperator)) {
        var sub = directory.Substring(0, directory.LastIndexOf(seperator));
        this.SearchDirectories(sub, ref subs, seperator);
        if (!subs.Contains(sub)) {
          subs.Add(sub);
        }
      }
    }

    public async Task UploadSnapshot(Snapshot snapshot, DirectoryInfo directory, CancellationToken cancellationToken, IObservableTaskProgress progress) {
      this.Reporter.StartSession(Messages.StartUploadSnapshot);
      // check snapshot
      if (snapshot == null) {
        this.Reporter.Report(Messages.InvalidSnapshot);
        return;
      }
      // search all needed directories
      var subs = new List<string>();
      foreach (var file in snapshot.Files) {
        this.SearchDirectories(FtpClient.CombinePath(snapshot.ApplicationPath, snapshot.VersionPath, file.RemotePath), ref subs, '/');
      }
      // sort by depth
      subs = subs.OrderBy(s => s.Count(c => c == '/')).ToList();
      // task count is now known
      progress?.ReportProgress(0, snapshot.Files.Count + subs.Count, this.GetMessageText(Messages.StartUploadSnapshot));
      // create subdirectories
      foreach (var sub in subs) {
        if (sub.Contains("/")) {
          // check if directory exists
          var directoryExists = await this.FtpClient.DirectoryExists(sub.Substring(0, sub.LastIndexOf('/')), sub.Substring(sub.LastIndexOf('/') + 1));
          if (!directoryExists) {
            await this.FtpClient.CreateDirectory(FtpClient.CombinePath(sub));
          }
        } else {
          // check if directory exists
          var directoryExists = await this.FtpClient.DirectoryExists("", sub);
          if (!directoryExists) {
            await this.FtpClient.CreateDirectory(sub);
          }
        }
        progress?.ReportProgress();
      }
      if (this.Reporter.HasErrors) {
        return;
      }
      foreach (var file in snapshot.Files) {
        var tries = 0;
        while (true) {
          if (this.Reporter.HasErrors) {
            return;
          }
          progress?.ReportProgress("Uploading " + file.RemotePath);
          var success = await this.FtpClient.UploadFileVerified(FtpClient.CombinePath(snapshot.ApplicationPath, snapshot.VersionPath, file.RemotePath), Path.Combine(directory.FullName, file.RemotePath), file.MD5, cancellationToken, file.Size, progress);
          if (success) {
            progress?.ReportProgress();
            break;
          } else {
            tries++;
            if (tries == 3) {
              this.Reporter.Report(Messages.VerifiedUploadImpossible);
              return;
            }
          }
        }
      }
      var snapshotFileInfo = new FileInfo(Path.Combine(directory.FullName, Snapshot.FileName));
      progress?.ReportProgress("Uploading the snapshot file");
      await this.FtpClient.UploadFile(FtpClient.CombinePath(snapshot.ApplicationPath, snapshot.VersionPath, Snapshot.FileName), snapshotFileInfo.FullName, cancellationToken, snapshotFileInfo.Length, progress);
      progress?.ReportProgress();
      progress?.ReportProgress("Uploading the snapshot file as head");
      await this.FtpClient.UploadFile(FtpClient.CombinePath(snapshot.ApplicationPath, Snapshot.FileName), snapshotFileInfo.FullName, cancellationToken, snapshotFileInfo.Length, progress);
      progress?.ReportProgress();
    }

    public Task<Snapshot> CreateLocalSnapshot(DirectoryInfo localDirectory, int applicationId, string applicationPath, int versionId, string versionPath, string description, List<SnapshotTask> tasks, IObservableTaskProgress progress) {
      return Task.Run(() => {
        Snapshot snapshot = null;
        this.Reporter.StartSession(Messages.StartCreateLocalSnapshot);
        // report initial progress
        progress?.ReportIndeterminate(this.GetMessageText(Messages.StartCreateLocalSnapshot));
        if (localDirectory == null || !localDirectory.Exists) {
          this.Reporter.Report(Messages.LocalDirectoryInvalid);
          return snapshot;
        }
        snapshot = new Snapshot() {
          ApplicationId = applicationId,
          ApplicationPath = applicationPath,
          VersionId = versionId,
          VersionPath = versionPath,
          Description = description,
          Tasks = tasks
        };
        try {
          var snapshotFileName = Path.Combine(localDirectory.FullName, Snapshot.FileName);
          // remove existing snapshot file
          if (File.Exists(snapshotFileName)) {
            File.Delete(snapshotFileName);
          }
          // add files
          foreach (var path in Directory.GetFiles(localDirectory.FullName, "*.*", SearchOption.AllDirectories)) {
            var file = new FileInfo(path);
            string md5;
            using (var stream = File.OpenRead(file.FullName)) {
              md5 = this.FtpClient.CalculateCRC(stream);
            }
            // create file entry
            var entry = new SnapshotFile() {
              RemotePath = file.FullName.Replace(localDirectory.FullName + Path.DirectorySeparatorChar, "").Replace(Path.DirectorySeparatorChar, '/'),
              MD5 = md5,
              Size = file.Length
            };
            // add entry to snapshot
            snapshot.Files.Add(entry);
          }
          // add default task
          foreach (var file in snapshot.Files) {
            if (file.RemotePath.Contains(".exe")) {
              snapshot.Tasks.Add(new SnapshotTask() {
                Id = SnapshotTask.TaskId.LaunchApplication,
                Execution = SnapshotTask.TaskExecution.Allways,
                Files = new List<string>(new string[] {
                                  file.RemotePath
                              })
              });
              break;
            }
          }
          using (var fileStream = File.Create(snapshotFileName)) {
            var serializer = new XmlSerializer(typeof(Snapshot));
            serializer.Serialize(fileStream, snapshot);
          }
        } catch {
          this.Reporter.Report(Messages.SnapshotSerializationFailed);
        }
        return snapshot;
      });
    }

    public async Task DownloadSnapshot(Snapshot snapshot, DirectoryInfo directory, CancellationToken cancellationToken, IObservableTaskProgress progress) {
      this.Reporter.StartSession(Messages.StartDownloadSnapshot);
      // report initial progress
      progress?.ReportIndeterminate(this.GetMessageText(Messages.StartDownloadSnapshot));
      // check snapshot
      if (snapshot == null) {
        this.Reporter.Report(Messages.InvalidSnapshot);
        return;
      }
      try {
        // search all needed directories
        var subs = new List<string>();
        foreach (var file in snapshot.Files) {
          this.SearchDirectories(Path.Combine(directory.FullName, file.LocalPath), ref subs, Path.DirectorySeparatorChar);
        }
        // sort by depth
        subs = subs.OrderBy(s => s.Count(c => c == Path.DirectorySeparatorChar)).ToList();
        // create subdirectories
        foreach (var sub in subs) {
          if (!Directory.Exists(sub)) {
            Directory.CreateDirectory(sub);
          }
        }
      } catch {
        this.Reporter.Report(Messages.FolderCreationFailed);
      }
      if (this.Reporter.HasErrors) {
        return;
      }
      progress?.ReportProgress(0, snapshot.Files.Count + 2);
      foreach (var file in snapshot.Files) {
        var tries = 0;
        while (true) {
          if (this.Reporter.HasErrors) {
            return;
          }
          var success = await this.FtpClient.DownloadToFileVerified(FtpClient.CombinePath(snapshot.ApplicationPath, snapshot.VersionPath, file.RemotePath), Path.Combine(directory.FullName, file.LocalPath), file.MD5, cancellationToken, file.Size, progress);
          if (success) {
            progress?.ReportProgress();
            break;
          } else {
            tries++;
            if (tries == 3) {
              this.Reporter.Report(Messages.VerifiedDownloadImpossible);
              return;
            }
          }
        }
      }
      // download snapshot
      await this.FtpClient.DownloadToFile(FtpClient.CombinePath(snapshot.ApplicationPath, snapshot.VersionPath, Snapshot.FileName), Path.Combine(directory.FullName, Snapshot.FileName), cancellationToken, progress: progress);
      progress?.ReportProgress();
      // download update controller
      if (File.Exists(SnapshotClient.UpdateControllerName)) {
        File.Delete(SnapshotClient.UpdateControllerName);
      }
      await this.FtpClient.DownloadToFile(SnapshotClient.UpdateControllerName, SnapshotClient.UpdateControllerName, cancellationToken, progress: progress);
      progress?.ReportProgress();
    }

    public Task<Snapshot> ReadLocalSnapshot(string localDirectory, IObservableTaskProgress progress) {
      return Task.Run(() => {
        progress?.ReportIndeterminate();
        this.Reporter.StartSession(Messages.StartReadLocalSnapshot);
        try {
          using (var fileStream = File.OpenRead(Path.Combine(localDirectory, Snapshot.FileName))) {
            var serializer = new XmlSerializer(typeof(Snapshot));
            return (Snapshot)serializer.Deserialize(fileStream);
          }
        } catch {
          this.Reporter.Report(Messages.SnapshotSerializationFailed);
          return null;
        }
      });
    }

    public void ExecuteUpdateController(string updateDir) {
      var process = new System.Diagnostics.Process();
      process.StartInfo.Arguments = "\"" + updateDir + "\"";
      process.StartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SnapshotClient.UpdateControllerName);
      process.Start();
    }

    public void RemoveLocalUpdateController() {
      var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SnapshotClient.UpdateControllerName);
      if (File.Exists(file)) {
        File.Delete(file);
      }
    }
  }
}
