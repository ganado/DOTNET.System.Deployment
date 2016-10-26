// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.FileDownloader
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Collections;
using System.Deployment.Application.Manifest;
using System.Deployment.Internal.Isolation;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace System.Deployment.Application
{
  internal abstract class FileDownloader
  {
    protected DownloadOptions _options = new DownloadOptions();
    protected ComponentVerifier _componentVerifier = new ComponentVerifier();
    protected Queue _fileQueue;
    protected DownloadEventArgs _eventArgs;
    protected ArrayList _downloadResults;
    protected long _accumulatedBytesTotal;
    protected long _expectedBytesTotal;
    protected bool _fCancelPending;
    protected byte[] _buffer;

    public X509Certificate2 ClientCertificate { get; protected set; }

    public DownloadOptions Options
    {
      set
      {
        this._options = value;
      }
    }

    public ComponentVerifier ComponentVerifier
    {
      get
      {
        return this._componentVerifier;
      }
    }

    public DownloadResult[] DownloadResults
    {
      get
      {
        return (DownloadResult[]) this._downloadResults.ToArray(typeof (DownloadResult));
      }
    }

    public event FileDownloader.DownloadModifiedEventHandler DownloadModified;

    public event FileDownloader.DownloadCompletedEventHandler DownloadCompleted;

    protected FileDownloader()
    {
      this._fileQueue = new Queue();
      this._eventArgs = new DownloadEventArgs();
      this._downloadResults = new ArrayList();
      this._buffer = new byte[4096];
    }

    public static FileDownloader Create()
    {
      return (FileDownloader) new SystemNetDownloader();
    }

    public void AddNotification(IDownloadNotification notification)
    {
      this.DownloadCompleted += new FileDownloader.DownloadCompletedEventHandler(notification.DownloadCompleted);
      this.DownloadModified += new FileDownloader.DownloadModifiedEventHandler(notification.DownloadModified);
    }

    public void RemoveNotification(IDownloadNotification notification)
    {
      this.DownloadModified -= new FileDownloader.DownloadModifiedEventHandler(notification.DownloadModified);
      this.DownloadCompleted -= new FileDownloader.DownloadCompletedEventHandler(notification.DownloadCompleted);
    }

    protected void OnModified()
    {
      // ISSUE: reference to a compiler-generated field
      if (this.DownloadModified == null)
        return;
      // ISSUE: reference to a compiler-generated field
      this.DownloadModified((object) this, this._eventArgs);
    }

    protected void OnCompleted()
    {
      // ISSUE: reference to a compiler-generated field
      if (this.DownloadCompleted == null)
        return;
      // ISSUE: reference to a compiler-generated field
      this.DownloadCompleted((object) this, this._eventArgs);
    }

    public void AddFile(Uri sourceUri, string targetFilePath)
    {
      this.AddFile(sourceUri, targetFilePath, (object) null, (HashCollection) null);
    }

    public void AddFile(Uri sourceUri, string targetFilePath, int maxFileSize)
    {
      this.AddFile(sourceUri, targetFilePath, (object) null, (HashCollection) null, maxFileSize);
    }

    public void AddFile(Uri sourceUri, string targetFilePath, object cookie, HashCollection hashCollection)
    {
      this.AddFile(sourceUri, targetFilePath, cookie, hashCollection, -1);
    }

    public void AddFile(Uri sourceUri, string targetFilePath, object cookie, HashCollection hashCollection, int maxFileSize)
    {
      UriHelper.ValidateSupportedScheme(sourceUri);
      FileDownloader.DownloadQueueItem downloadQueueItem = new FileDownloader.DownloadQueueItem();
      downloadQueueItem._sourceUri = sourceUri;
      downloadQueueItem._targetPath = targetFilePath;
      downloadQueueItem._cookie = cookie;
      downloadQueueItem._hashCollection = hashCollection;
      downloadQueueItem._maxFileSize = maxFileSize;
      lock (this._fileQueue)
      {
        this._fileQueue.Enqueue((object) downloadQueueItem);
        ++this._eventArgs._filesTotal;
      }
    }

    private static FileStream GetPatchSourceStream(string filePath)
    {
      Logger.AddMethodCall("GetPatchSourceStream(" + filePath + ") called.");
      FileStream fileStream = (FileStream) null;
      try
      {
        fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
      }
      catch (IOException ex)
      {
        Logger.AddErrorInformation((Exception) ex, Resources.GetString("Ex_PatchSourceOpenFailed"), new object[1]
        {
          (object) Path.GetFileName(filePath)
        });
      }
      catch (UnauthorizedAccessException ex)
      {
        Logger.AddErrorInformation((Exception) ex, Resources.GetString("Ex_PatchSourceOpenFailed"), new object[1]
        {
          (object) Path.GetFileName(filePath)
        });
      }
      return fileStream;
    }

    private static FileStream GetPatchTargetStream(string filePath)
    {
      return new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
    }

    private static bool FileHashVerified(HashCollection hashCollection, string location)
    {
      try
      {
        ComponentVerifier.VerifyFileHash(location, hashCollection);
      }
      catch (InvalidDeploymentException ex)
      {
        if (ex.SubType == ExceptionTypes.HashValidation)
          return false;
        throw;
      }
      return true;
    }

    private static bool AddSingleFileInHashtable(Hashtable hashtable, HashCollection hashCollection, string location)
    {
      bool flag = false;
      if (System.IO.File.Exists(location))
      {
        foreach (Hash hash in hashCollection)
        {
          string compositString = hash.CompositString;
          if (!hashtable.Contains((object) compositString))
          {
            hashtable.Add((object) compositString, (object) location);
            flag = true;
          }
        }
      }
      return flag;
    }

    private static void AddFilesInHashtable(Hashtable hashtable, AssemblyManifest applicationManifest, string applicationFolder)
    {
      Logger.AddMethodCall("AddFilesInHashtable called.");
      Logger.AddInternalState("applicationFolder=" + applicationFolder);
      foreach (System.Deployment.Application.Manifest.File file in applicationManifest.Files)
      {
        string str = Path.Combine(applicationFolder, file.NameFS);
        try
        {
          FileDownloader.AddSingleFileInHashtable(hashtable, file.HashCollection, str);
        }
        catch (IOException ex)
        {
          Logger.AddErrorInformation((Exception) ex, Resources.GetString("Ex_PatchDependencyFailed"), new object[1]
          {
            (object) Path.GetFileName(str)
          });
          Logger.AddInternalState("Exception thrown : " + ex.GetType().ToString() + ":" + ex.Message);
        }
      }
      foreach (DependentAssembly dependentAssembly in applicationManifest.DependentAssemblies)
      {
        if (!dependentAssembly.IsPreRequisite)
        {
          string str = Path.Combine(applicationFolder, dependentAssembly.Codebase);
          try
          {
            if (FileDownloader.AddSingleFileInHashtable(hashtable, dependentAssembly.HashCollection, str))
            {
              System.Deployment.Application.Manifest.File[] files = new AssemblyManifest(str).Files;
              for (int index = 0; index < files.Length; ++index)
              {
                string location = Path.Combine(Path.GetDirectoryName(str), files[index].NameFS);
                FileDownloader.AddSingleFileInHashtable(hashtable, files[index].HashCollection, location);
              }
            }
          }
          catch (InvalidDeploymentException ex)
          {
            Logger.AddErrorInformation((Exception) ex, Resources.GetString("Ex_PatchDependencyFailed"), new object[1]
            {
              (object) Path.GetFileName(str)
            });
            Logger.AddInternalState("Exception thrown : " + ex.GetType().ToString() + ":" + ex.Message);
          }
          catch (IOException ex)
          {
            Logger.AddErrorInformation((Exception) ex, Resources.GetString("Ex_PatchDependencyFailed"), new object[1]
            {
              (object) Path.GetFileName(str)
            });
            Logger.AddInternalState("Exception thrown : " + ex.GetType().ToString() + ":" + ex.Message);
          }
        }
      }
    }

    private bool PatchSingleFile(FileDownloader.DownloadQueueItem item, Hashtable dependencyTable)
    {
      if (item._hashCollection == null)
        return false;
      string str = (string) null;
      foreach (Hash hash in item._hashCollection)
      {
        string compositString = hash.CompositString;
        if (dependencyTable.Contains((object) compositString))
        {
          str = (string) dependencyTable[(object) compositString];
          break;
        }
      }
      if (str == null || this._fCancelPending)
        return false;
      if (!FileDownloader.FileHashVerified(item._hashCollection, str))
      {
        Logger.AddInternalState("Hash verify failed for " + str + ", not using it for file patching.");
        return false;
      }
      FileStream fileStream1 = (FileStream) null;
      FileStream fileStream2 = (FileStream) null;
      try
      {
        fileStream1 = FileDownloader.GetPatchSourceStream(str);
        if (fileStream1 == null)
          return false;
        Directory.CreateDirectory(Path.GetDirectoryName(item._targetPath));
        fileStream2 = FileDownloader.GetPatchTargetStream(item._targetPath);
        if (fileStream2 == null)
          return false;
        this._eventArgs._fileSourceUri = item._sourceUri;
        this._eventArgs.FileLocalPath = item._targetPath;
        this._eventArgs.Cookie = (object) null;
        this._eventArgs._fileResponseUri = (Uri) null;
        this.CheckForSizeLimit((ulong) fileStream1.Length, true);
        this._accumulatedBytesTotal = this._accumulatedBytesTotal + fileStream1.Length;
        this.SetBytesTotal();
        this.OnModified();
        int tickCount = Environment.TickCount;
        fileStream2.SetLength(fileStream1.Length);
        fileStream2.Position = 0L;
        int count;
        do
        {
          if (this._fCancelPending)
            return false;
          count = fileStream1.Read(this._buffer, 0, this._buffer.Length);
          if (count > 0)
            fileStream2.Write(this._buffer, 0, count);
          this._eventArgs._bytesCompleted += (long) count;
          this._eventArgs._progress = (int) (this._eventArgs._bytesCompleted * 100L / this._eventArgs._bytesTotal);
          this.OnModifiedWithThrottle(ref tickCount);
        }
        while (count > 0);
      }
      finally
      {
        if (fileStream1 != null)
          fileStream1.Close();
        if (fileStream2 != null)
          fileStream2.Close();
      }
      this._eventArgs.Cookie = item._cookie;
      ++this._eventArgs._filesCompleted;
      this.OnModified();
      this._downloadResults.Add((object) new DownloadResult()
      {
        ResponseUri = (Uri) null
      });
      Logger.AddInternalState(item._targetPath + " is patched from store.");
      return true;
    }

    private void PatchFiles(SubscriptionState subState)
    {
      if (!subState.IsInstalled)
      {
        Logger.AddInternalState("Subscription is not installed. No patching.");
      }
      else
      {
        Store.IPathLock pathLock1 = (Store.IPathLock) null;
        Store.IPathLock pathLock2 = (Store.IPathLock) null;
        using (subState.SubscriptionStore.AcquireSubscriptionReaderLock(subState))
        {
          if (!subState.IsInstalled)
          {
            Logger.AddInternalState("Subscription is not installed. No patching.");
            return;
          }
          Hashtable hashtable = new Hashtable();
          try
          {
            pathLock1 = subState.SubscriptionStore.LockApplicationPath(subState.CurrentBind);
            FileDownloader.AddFilesInHashtable(hashtable, subState.CurrentApplicationManifest, pathLock1.Path);
            try
            {
              if (subState.PreviousBind != null)
              {
                pathLock2 = subState.SubscriptionStore.LockApplicationPath(subState.PreviousBind);
                FileDownloader.AddFilesInHashtable(hashtable, subState.PreviousApplicationManifest, pathLock2.Path);
              }
              Queue queue = new Queue();
              do
              {
                FileDownloader.DownloadQueueItem downloadQueueItem = (FileDownloader.DownloadQueueItem) null;
                lock (this._fileQueue)
                {
                  if (this._fileQueue.Count > 0)
                    downloadQueueItem = (FileDownloader.DownloadQueueItem) this._fileQueue.Dequeue();
                }
                if (downloadQueueItem != null)
                {
                  if (!this.PatchSingleFile(downloadQueueItem, hashtable))
                    queue.Enqueue((object) downloadQueueItem);
                }
                else
                  break;
              }
              while (!this._fCancelPending);
              lock (this._fileQueue)
              {
                while (this._fileQueue.Count > 0)
                  queue.Enqueue(this._fileQueue.Dequeue());
                this._fileQueue = queue;
              }
            }
            finally
            {
              if (pathLock2 != null)
                pathLock2.Dispose();
            }
          }
          finally
          {
            if (pathLock1 != null)
              pathLock1.Dispose();
          }
        }
        if (this._fCancelPending)
          throw new DownloadCancelledException();
      }
    }

    public void Download(SubscriptionState subState, X509Certificate2 clientCertificate)
    {
      this.ClientCertificate = clientCertificate;
      try
      {
        this.OnModified();
        if (subState != null)
        {
          CodeMarker_Singleton.Instance.CodeMarker(564);
          this.PatchFiles(subState);
          CodeMarker_Singleton.Instance.CodeMarker(565);
        }
        this.DownloadAllFiles();
      }
      finally
      {
        this.OnCompleted();
      }
    }

    public void SetExpectedBytesTotal(long total)
    {
      this._expectedBytesTotal = total;
    }

    protected void SetBytesTotal()
    {
      if (this._expectedBytesTotal < this._accumulatedBytesTotal)
        this._eventArgs._bytesTotal = this._accumulatedBytesTotal;
      else
        this._eventArgs._bytesTotal = this._expectedBytesTotal;
    }

    internal void CheckForSizeLimit(ulong bytesDownloaded, bool addToSize)
    {
      if (this._options == null || !this._options.EnforceSizeLimit)
        return;
      ulong num = this._options.SizeLimit > this._options.Size ? this._options.SizeLimit - this._options.Size : 0UL;
      if (bytesDownloaded > num)
        throw new DeploymentDownloadException(ExceptionTypes.SizeLimitForPartialTrustOnlineAppExceeded, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_OnlineSemiTrustAppSizeLimitExceeded"), new object[1]
        {
          (object) this._options.SizeLimit
        }));
      if (!addToSize || bytesDownloaded <= 0UL)
        return;
      this._options.Size = this._options.Size + bytesDownloaded;
    }

    protected void OnModifiedWithThrottle(ref int lastTick)
    {
      int tickCount = Environment.TickCount;
      int num = tickCount - lastTick;
      if (num < 0)
        num += int.MaxValue;
      if (num < 100)
        return;
      this.OnModified();
      lastTick = tickCount;
    }

    public virtual void Cancel()
    {
      this._fCancelPending = true;
    }

    protected abstract void DownloadAllFiles();

    public delegate void DownloadModifiedEventHandler(object sender, DownloadEventArgs e);

    public delegate void DownloadCompletedEventHandler(object sender, DownloadEventArgs e);

    protected class DownloadQueueItem
    {
      public Uri _sourceUri;
      public string _targetPath;
      public object _cookie;
      public HashCollection _hashCollection;
      public int _maxFileSize;
      public const int FileOfAnySize = -1;

      public override string ToString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(" _sourceUri = " + (object) this._sourceUri);
        stringBuilder.Append(",  _targetPath = " + this._targetPath);
        return stringBuilder.ToString();
      }
    }
  }
}
