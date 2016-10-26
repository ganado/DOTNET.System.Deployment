// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.SystemNetDownloader
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace System.Deployment.Application
{
  internal class SystemNetDownloader : FileDownloader
  {
    private static Stream GetOutputFileStream(string targetPath)
    {
      return (Stream) new FileStream(targetPath, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
    }

    private WebRequest CreateWebRequest(Uri uri)
    {
      WebRequest webRequest = WebRequest.Create(uri);
      webRequest.Credentials = CredentialCache.DefaultCredentials;
      RequestCachePolicy requestCachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
      webRequest.CachePolicy = requestCachePolicy;
      HttpWebRequest httpWebRequest = webRequest as HttpWebRequest;
      if (httpWebRequest != null)
      {
        httpWebRequest.UnsafeAuthenticatedConnectionSharing = true;
        httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
        httpWebRequest.CookieContainer = SystemNetDownloader.GetUriCookieContainer(httpWebRequest.RequestUri);
        IWebProxy defaultWebProxy = WebRequest.DefaultWebProxy;
        if (defaultWebProxy != null)
          defaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
        Logger.AddInternalState("HttpWebRequest=" + Logger.Serialize((WebRequest) httpWebRequest));
      }
      return webRequest;
    }

    protected void DownloadSingleFile(FileDownloader.DownloadQueueItem next)
    {
      Logger.AddMethodCall("DownloadSingleFile called");
      Logger.AddInternalState("DownloadQueueItem : " + (next != null ? next.ToString() : "null"));
      WebRequest webRequest = this.CreateWebRequest(next._sourceUri);
      if (this._fCancelPending)
        return;
      WebResponse response1 = (WebResponse) null;
      try
      {
        if (this.ClientCertificate == null)
        {
          try
          {
            response1 = webRequest.GetResponse();
          }
          catch (WebException ex)
          {
            HttpWebResponse response2 = ex.Response as HttpWebResponse;
            if (response2 != null && response2.StatusCode == HttpStatusCode.Forbidden)
            {
              this.ClientCertificate = this.GetClientCertificate();
              if (this.ClientCertificate == null)
                throw;
              else
                webRequest = this.CreateWebRequest(next._sourceUri);
            }
            else
              throw;
          }
        }
        if (this.ClientCertificate != null)
        {
          HttpWebRequest httpWebRequest = webRequest as HttpWebRequest;
          if (httpWebRequest != null)
            httpWebRequest.ClientCertificates.Add((X509Certificate) this.ClientCertificate);
          response1 = webRequest.GetResponse();
        }
        UriHelper.ValidateSupportedScheme(response1.ResponseUri);
        if (this._fCancelPending)
          return;
        this._eventArgs._fileSourceUri = next._sourceUri;
        this._eventArgs._fileResponseUri = response1.ResponseUri;
        this._eventArgs.FileLocalPath = next._targetPath;
        this._eventArgs.Cookie = (object) null;
        if (response1.ContentLength > 0L)
        {
          this.CheckForSizeLimit((ulong) response1.ContentLength, false);
          this._accumulatedBytesTotal = this._accumulatedBytesTotal + response1.ContentLength;
        }
        this.SetBytesTotal();
        this.OnModified();
        Stream stream1 = (Stream) null;
        Stream stream2 = (Stream) null;
        int tickCount = Environment.TickCount;
        try
        {
          stream1 = response1.GetResponseStream();
          Directory.CreateDirectory(Path.GetDirectoryName(next._targetPath));
          stream2 = SystemNetDownloader.GetOutputFileStream(next._targetPath);
          if (stream2 != null)
          {
            long num = 0;
            if (response1.ContentLength > 0L)
              stream2.SetLength(response1.ContentLength);
            while (!this._fCancelPending)
            {
              int count = stream1.Read(this._buffer, 0, this._buffer.Length);
              if (count > 0)
                stream2.Write(this._buffer, 0, count);
              this._eventArgs._bytesCompleted += (long) count;
              if (response1.ContentLength <= 0L)
              {
                this._accumulatedBytesTotal = this._accumulatedBytesTotal + (long) count;
                this.SetBytesTotal();
              }
              num += (long) count;
              if (next._maxFileSize != -1 && num > (long) next._maxFileSize)
                throw new InvalidDeploymentException(ExceptionTypes.FileSizeValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_FileBeingDownloadedTooLarge"), new object[2]
                {
                  (object) next._sourceUri.ToString(),
                  (object) next._maxFileSize
                }));
              this.CheckForSizeLimit((ulong) count, true);
              if (this._eventArgs._bytesTotal > 0L)
                this._eventArgs._progress = (int) (this._eventArgs._bytesCompleted * 100L / this._eventArgs._bytesTotal);
              this.OnModifiedWithThrottle(ref tickCount);
              if (count <= 0)
              {
                if (response1.ContentLength != num)
                {
                  stream2.SetLength(num);
                  goto label_40;
                }
                else
                  goto label_40;
              }
            }
            return;
          }
        }
        finally
        {
          if (stream1 != null)
            stream1.Close();
          if (stream2 != null)
            stream2.Close();
        }
label_40:
        this._eventArgs.Cookie = next._cookie;
        ++this._eventArgs._filesCompleted;
        Logger.AddInternalState("HttpWebResponse=" + Logger.Serialize(response1));
        this.OnModified();
        this._downloadResults.Add((object) new DownloadResult()
        {
          ResponseUri = response1.ResponseUri,
          ServerInformation = {
            Server = response1.Headers["Server"],
            PoweredBy = response1.Headers["X-Powered-By"],
            AspNetVersion = response1.Headers["X-AspNet-Version"]
          }
        });
      }
      catch (InvalidOperationException ex)
      {
        throw new DeploymentDownloadException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_FailedWhileDownloading"), new object[1]
        {
          (object) next._sourceUri
        }), (Exception) ex);
      }
      catch (IOException ex)
      {
        throw new DeploymentDownloadException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_FailedWhileDownloading"), new object[1]
        {
          (object) next._sourceUri
        }), (Exception) ex);
      }
      catch (UnauthorizedAccessException ex)
      {
        throw new DeploymentDownloadException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_FailedWhileDownloading"), new object[1]
        {
          (object) next._sourceUri
        }), (Exception) ex);
      }
      finally
      {
        if (response1 != null)
          response1.Close();
      }
    }

    private X509Certificate2 GetClientCertificate()
    {
      X509Certificate2 x509Certificate2 = (X509Certificate2) null;
      X509Store x509Store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
      x509Store.Open(OpenFlags.OpenExistingOnly);
      X509Certificate2Collection certificate2Collection = X509Certificate2UI.SelectFromCollection(x509Store.Certificates.Find(X509FindType.FindByApplicationPolicy, (object) "1.3.6.1.5.5.7.3.2", false), (string) null, (string) null, X509SelectionFlag.SingleSelection);
      if (certificate2Collection.Count > 0)
        x509Certificate2 = certificate2Collection[0];
      return x509Certificate2;
    }

    protected override void DownloadAllFiles()
    {
      if (ServicePointManager.SecurityProtocol != (ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12))
      {
        Logger.AddInternalState(" Current TLS Protocol = " + ServicePointManager.SecurityProtocol.ToString());
        ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        Logger.AddInternalState(string.Format(" Default TLS protocol is changed to = {0} with implicit fallback ", (object) ServicePointManager.SecurityProtocol.ToString()));
      }
      do
      {
        FileDownloader.DownloadQueueItem next = (FileDownloader.DownloadQueueItem) null;
        lock (this._fileQueue)
        {
          if (this._fileQueue.Count > 0)
            next = (FileDownloader.DownloadQueueItem) this._fileQueue.Dequeue();
        }
        if (next != null)
          this.DownloadSingleFile(next);
        else
          break;
      }
      while (!this._fCancelPending);
      if (this._fCancelPending)
        throw new DownloadCancelledException();
    }

    private static CookieContainer GetUriCookieContainer(Uri uri)
    {
      CookieContainer cookieContainer = (CookieContainer) null;
      uint bytes = 0;
      if (NativeMethods.InternetGetCookieW(uri.ToString(), (string) null, (StringBuilder) null, out bytes))
      {
        StringBuilder cookieData = new StringBuilder((int) (bytes / 2U));
        if (NativeMethods.InternetGetCookieW(uri.ToString(), (string) null, cookieData, out bytes))
        {
          if (cookieData.Length > 0)
          {
            try
            {
              cookieContainer = new CookieContainer();
              cookieContainer.SetCookies(uri, cookieData.ToString().Replace(';', ','));
            }
            catch (CookieException ex)
            {
              cookieContainer = (CookieContainer) null;
            }
          }
        }
      }
      return cookieContainer;
    }
  }
}
