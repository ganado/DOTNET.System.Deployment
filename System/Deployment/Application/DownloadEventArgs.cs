// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.DownloadEventArgs
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

namespace System.Deployment.Application
{
  internal class DownloadEventArgs : EventArgs
  {
    internal int _progress;
    internal int _filesCompleted;
    internal int _filesTotal;
    internal long _bytesCompleted;
    internal long _bytesTotal;
    internal Uri _fileSourceUri;
    internal Uri _fileResponseUri;
    internal string _fileLocalPath;
    internal object _cookie;

    public int Progress
    {
      get
      {
        return this._progress;
      }
    }

    public long BytesCompleted
    {
      get
      {
        return this._bytesCompleted;
      }
    }

    public long BytesTotal
    {
      get
      {
        return this._bytesTotal;
      }
    }

    public Uri FileSourceUri
    {
      get
      {
        return this._fileSourceUri;
      }
    }

    public Uri FileResponseUri
    {
      get
      {
        return this._fileResponseUri;
      }
    }

    internal string FileLocalPath
    {
      get
      {
        return this._fileLocalPath;
      }
      set
      {
        this._fileLocalPath = value;
      }
    }

    internal object Cookie
    {
      get
      {
        return this._cookie;
      }
      set
      {
        this._cookie = value;
      }
    }
  }
}
