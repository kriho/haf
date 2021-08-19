using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HAF.Ftp {
  // the ftp interaction is realized using the FtpWebRequest class
  // http://www.codeproject.com/Tips/443588/Simple-Csharp-FTP-Class

  public class FtpClient: ObservableObject {
    #region messages

    public enum Messages {
      // session starts
      StartDownloadFile,
      StartDownloadStream,
      StartUploadFile,
      StartRenameFile,
      StartListDirectories,
      StartDirectoryExists,
      StartCreateDirectory,
      StartDeleteFile,
      StartRemoveDirectory,
      StartCalculatingCRC,

      // error messages
      InvalidFtpRequest,
      DownloadStreamFailed,
      DownloadFileFailed,
      CalculatingCrcFailed,
      LocalFileNotFound,
      UploadFileFailed,
      ListDirectoriesFailed,
      CreateDirectoryFailed,
      RemoveDirectoryFailed,
      DeleteFileFailed,
      RenameFileFailed
    }

    public string GetMessageText(Messages message) {
      switch (message) {
        case Messages.StartDownloadFile: return "Downloading remote file...";
        case Messages.StartDownloadStream: return "Downloading remote file to stream...";
        case Messages.StartUploadFile: return "Uploading local file...";
        case Messages.StartRenameFile: return "Renaming remote file...";
        case Messages.StartListDirectories: return "Listing remote directories...";
        case Messages.StartCreateDirectory: return "Creating remote directory...";
        case Messages.StartDeleteFile: return "Deleting remote file...";
        case Messages.StartRemoveDirectory: return "Removong remote directory...";
        case Messages.StartCalculatingCRC: return "Calculating file CRC...";
        case Messages.InvalidFtpRequest: return "The FTP request URL is invalid!";
        case Messages.DownloadFileFailed: return "Remote file could not be donwloaded!";
        case Messages.DownloadStreamFailed: return "Remote file could not be donwloaded to stream!";
        case Messages.CalculatingCrcFailed: return "CRC calculation failed!";
        case Messages.LocalFileNotFound: return "Local file not found!";
        case Messages.UploadFileFailed: return "Uploading local file failed!";
        case Messages.ListDirectoriesFailed: return "Listing remote directories failed!";
        case Messages.CreateDirectoryFailed: return "Creating remote directory failed!";
        case Messages.RemoveDirectoryFailed: return "Removing remote directory failed!";
        case Messages.DeleteFileFailed: return "Deleting remote file failed!";
        case Messages.RenameFileFailed: return "Renaming remote file failed!";
        default: return "An unidentified error ocurred!";
      }
    }

    public ReportSeverity GetMessageType(Messages message) {
      switch (message) {
        case Messages.StartDownloadFile:
        case Messages.StartDownloadStream:
        case Messages.StartUploadFile:
        case Messages.StartRenameFile:
        case Messages.StartListDirectories:
        case Messages.StartDirectoryExists:
        case Messages.StartCreateDirectory:
        case Messages.StartDeleteFile:
        case Messages.StartRemoveDirectory:
          return ReportSeverity.Information;
        default:
          return ReportSeverity.Error;
      }
    }

    #endregion

    private string host, username, password;
    private MD5 md5 = MD5.Create();

    public Reporter<string, Messages> Reporter { get; set; }

    public FtpClient(string host, string username, string password) {
      // store connection information
      this.host = host;
      this.username = username;
      this.password = password;
      // initialize reporter
      this.Reporter = new Reporter<string, Messages>("FTP Client", this.GetMessageText, this.GetMessageType);
    }

    private FtpWebRequest CreateRequest(string path, string method) {
      try {
        // create request
        var ftpRequest = (FtpWebRequest)WebRequest.Create(CombinePath(this.host, path));
        // use provided credentials
        ftpRequest.Credentials = new NetworkCredential(this.username, this.password);
        // configure ftp request
        ftpRequest.UseBinary = true;
        ftpRequest.UsePassive = true;
        ftpRequest.KeepAlive = true;
        ftpRequest.Method = method;
        return ftpRequest;
      } catch {
        this.Reporter.Report(Messages.InvalidFtpRequest);
        return null;
      }
    }

    public string CalculateCRC(Stream stream) {
      try {
        byte[] md5;
        // gather all data in memory stream
        using (var mem = new MemoryStream()) {
          stream.CopyTo(mem);
          md5 = this.md5.ComputeHash(mem.ToArray());
        }
        return Convert.ToBase64String(md5); ;
      } catch {
        this.Reporter.Report(Messages.CalculatingCrcFailed);
        return null;
      }
    }

    public string CalculateCRC(MemoryStream stream) {
      try {
        return Convert.ToBase64String(this.md5.ComputeHash(stream.ToArray()));
      } catch {
        this.Reporter.Report(Messages.CalculatingCrcFailed);
        return null;
      }
    }

    public static string CombinePath(params string[] paths) {
      if (paths.Length > 0) {
        if (paths[0].StartsWith("ftp://"))
          return Path.Combine(paths);
        else {
          var path = String.Join("/", paths);
          path.TrimStart(new char[] { ' ', '/' });
          path.TrimEnd(new char[] { ' ' });

          return path;
        }
      } else {
        return "";
      }
    }

    public Task<MemoryStream> DownloadToStream(string remoteFile, CancellationToken cancellationToken, long fileSize = 0, ITaskProgress progress = null) {
      return Task.Run(() => {
        this.Reporter.StartSession(Messages.StartDownloadStream);
        var stream = new MemoryStream();
        // read head snapshot file
        var ftpRequest = this.CreateRequest(remoteFile, WebRequestMethods.Ftp.DownloadFile);
        if (this.Reporter.HasErrors) {
          return null;
        }
        // report initial progress
        progress?.ReportProgress(0, (int)fileSize, this.GetMessageText(Messages.StartDownloadStream));
        try {
          // get response from server
          using (var response = (FtpWebResponse)ftpRequest.GetResponse()) {
            // get response stream
            using (var ftpStream = response.GetResponseStream()) {
              var bytesRead = 0;
              var buffer = new byte[2048];
              // read stream into memory
              while (!cancellationToken.IsCancellationRequested) {
                bytesRead = ftpStream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) {
                  break;
                }
                stream.Write(buffer, 0, bytesRead);
                // report progress
                if (fileSize > 0) {
                  progress?.ReportProgress((int)stream.Position, (int)fileSize);
                }
              }
            }
          }
          return stream;
        } catch {
          this.Reporter.Report(Messages.DownloadStreamFailed);
          return null;
        }
      });
    }

    public Task DownloadToFile(string remoteFile, string localFile, CancellationToken cancellationToken, long fileSize = 0, ITaskProgress progress = null) {
      return Task.Run(() => {
        this.Reporter.StartSession(Messages.StartDownloadFile);
        // read head snapshot file
        var ftpRequest = this.CreateRequest(remoteFile, WebRequestMethods.Ftp.DownloadFile);
        if (this.Reporter.HasErrors) {
          return;
        }
        // report initial progress
        progress?.ReportProgress(0, (int)fileSize, this.GetMessageText(Messages.StartDownloadFile));
        try {
          // get response from server
          using (var response = (FtpWebResponse)ftpRequest.GetResponse()) {
            // get response stream
            using (var ftpStream = response.GetResponseStream()) {
              var bytesRead = 0;
              var buffer = new byte[2048];
              using (var stream = File.Create(localFile)) {
                // read stream into memory
                while (!cancellationToken.IsCancellationRequested) {
                  bytesRead = ftpStream.Read(buffer, 0, buffer.Length);
                  if (bytesRead == 0) {
                    break;
                  }
                  stream.Write(buffer, 0, bytesRead);
                  // report progress
                  if (fileSize > 0) {
                    progress?.ReportProgress((int)stream.Position, (int)fileSize);
                  }
                }
              }
            }
          }
        } catch {
          this.Reporter.Report(Messages.DownloadFileFailed);
        }
      });
    }

    public async Task<bool> DownloadToFileVerified(string remoteFile, string localFile, string md5, CancellationToken cancellationToken, long fileSize = 0, ITaskProgress progress = null) {
      await this.DownloadToFile(remoteFile, localFile, cancellationToken, fileSize, progress);
      if (this.Reporter.HasErrors) {
        return false;
      }
      try {
        using (var stream = File.OpenRead(localFile)) {
          return md5 == this.CalculateCRC(stream);
        }
      } catch {
        this.Reporter.Report(Messages.CalculatingCrcFailed);
        return false;
      }
    }

    public Task UploadFile(string remoteFile, string localFile, CancellationToken cancellationToken, long fileSize = 0, ITaskProgress progress = null) {
      return Task.Run(() => {
        this.Reporter.StartSession(Messages.StartUploadFile);
        // check if local file exists
        if (!File.Exists(localFile)) {
          this.Reporter.Report(Messages.LocalFileNotFound);
          return;
        }
        // read head snapshot file
        var ftpRequest = this.CreateRequest(remoteFile, WebRequestMethods.Ftp.UploadFile);
        if (this.Reporter.HasErrors) {
          return;
        }
        // report initial progress
        progress?.ReportProgress(0, (int)fileSize, this.GetMessageText(Messages.StartUploadFile));
        try {
          // get response stream
          using (var ftpStream = ftpRequest.GetRequestStream()) {
            var bytesRead = 0;
            var buffer = new byte[2048];
            using (var stream = File.OpenRead(localFile)) {
              // read stream into memory
              while (!cancellationToken.IsCancellationRequested) {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) {
                  break;
                }
                ftpStream.Write(buffer, 0, bytesRead);
                // report progress
                if (fileSize > 0) {
                  progress?.ReportProgress((int)stream.Position, (int)fileSize);
                }
              }
            }
          }
        } catch {
          this.Reporter.Report(Messages.UploadFileFailed);
        }
      });
    }

    public async Task<bool> UploadFileVerified(string remoteFile, string localFile, string md5, CancellationToken cancellationToken, long fileSize = 0, ITaskProgress progress = null) {
      await this.UploadFile(remoteFile, localFile, cancellationToken, fileSize, progress);
      if (this.Reporter.HasErrors) {
        return false;
      }
      using (var stream = await this.DownloadToStream(remoteFile, cancellationToken, fileSize, progress)) {
        if (this.Reporter.HasErrors) {
          return false;
        }
        return md5 == this.CalculateCRC(stream);
      }
    }

    public Task<List<string>> ListDirectories(string remoteDirectory) {
      return Task.Run(() => {
        var directories = new List<string>();
        this.Reporter.StartSession(Messages.StartListDirectories);
        // read head snapshot file
        var ftpRequest = this.CreateRequest(remoteDirectory, WebRequestMethods.Ftp.ListDirectory);
        if (this.Reporter.HasErrors) {
          return directories;
        }
        try {
          // get response from server
          using (var response = (FtpWebResponse)ftpRequest.GetResponse()) {
            // get response stream
            using (var streamReader = new StreamReader(response.GetResponseStream())) {
              string dir = streamReader.ReadLine();
              while (!string.IsNullOrEmpty(dir)) {
                directories.Add(dir);
                dir = streamReader.ReadLine();
              }
            }
          }
        } catch {
          this.Reporter.Report(Messages.ListDirectoriesFailed);
        }
        return directories;
      });
    }

    public async Task<bool> DirectoryExists(string remoteDirectoryParent, string remoteDirectory) {
      var directories = await this.ListDirectories(remoteDirectoryParent);
      if (this.Reporter.HasErrors) {
        return false;
      }
      return directories.Any(d => d.Substring(d.LastIndexOf('/') + 1) == remoteDirectory);
    }

    public Task CreateDirectory(string remoteDirectory) {
      return Task.Run(() => {
        this.Reporter.StartSession(Messages.StartCreateDirectory);
        // read head snapshot file
        var ftpRequest = this.CreateRequest(remoteDirectory, WebRequestMethods.Ftp.MakeDirectory);
        if (this.Reporter.HasErrors) {
          return;
        }
        try {
          // get response from server
          var response = (FtpWebResponse)ftpRequest.GetResponse();
          response.Close();
        } catch {
          this.Reporter.Report(Messages.CreateDirectoryFailed);
        }
      });
    }

    public Task RemoveDirectory(string remoteDirectory) {
      return Task.Run(() => {
        this.Reporter.StartSession(Messages.StartRemoveDirectory);
        // read head snapshot file
        var ftpRequest = this.CreateRequest(remoteDirectory, WebRequestMethods.Ftp.RemoveDirectory);
        if (this.Reporter.HasErrors) {
          return;
        }
        try {
          // get response from server
          var response = (FtpWebResponse)ftpRequest.GetResponse();
          response.Close();
        } catch {
          this.Reporter.Report(Messages.RemoveDirectoryFailed);
        }
      });
    }

    public Task DeleteFile(string remoteFile) {
      return Task.Run(() => {
        this.Reporter.StartSession(Messages.StartDeleteFile);
        // read head snapshot file
        var ftpRequest = this.CreateRequest(remoteFile, WebRequestMethods.Ftp.DeleteFile);
        if (this.Reporter.HasErrors) {
          return;
        }
        try {
          // get response from server
          var response = (FtpWebResponse)ftpRequest.GetResponse();
          response.Close();
        } catch {
          this.Reporter.Report(Messages.DeleteFileFailed);
        }
      });
    }

    public Task RenameFile(string remoteFile, string newRemoteFile) {
      return Task.Run(() => {
        this.Reporter.StartSession(Messages.StartRenameFile);
        // read head snapshot file
        var ftpRequest = this.CreateRequest(remoteFile, WebRequestMethods.Ftp.Rename);
        ftpRequest.RenameTo = newRemoteFile;
        if (this.Reporter.HasErrors) {
          return;
        }
        try {
          // get response from server
          var response = (FtpWebResponse)ftpRequest.GetResponse();
          response.Close();
        } catch {
          this.Reporter.Report(Messages.RenameFileFailed);
        }
      });
    }
  }
}
