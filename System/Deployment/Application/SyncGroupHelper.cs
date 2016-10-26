// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.SyncGroupHelper
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.ComponentModel;
using System.Threading;

namespace System.Deployment.Application
{
  internal class SyncGroupHelper : IDownloadNotification
  {
    private readonly string groupName;
    private readonly object userState;
    private readonly AsyncOperation asyncOperation;
    private readonly SendOrPostCallback progressReporter;
    private bool _cancellationPending;

    public bool CancellationPending
    {
      get
      {
        return this._cancellationPending;
      }
    }

    public string Group
    {
      get
      {
        return this.groupName;
      }
    }

    public object UserState
    {
      get
      {
        return this.userState;
      }
    }

    public SyncGroupHelper(string groupName, object userState, AsyncOperation asyncOp, SendOrPostCallback progressReporterDelegate)
    {
      if (groupName == null)
        throw new ArgumentNullException("groupName");
      this.groupName = groupName;
      this.userState = userState;
      this.asyncOperation = asyncOp;
      this.progressReporter = progressReporterDelegate;
    }

    public void SetComplete()
    {
    }

    public void CancelAsync()
    {
      this._cancellationPending = true;
    }

    public void DownloadModified(object sender, DownloadEventArgs e)
    {
      if (this._cancellationPending)
        ((FileDownloader) sender).Cancel();
      this.asyncOperation.Post(this.progressReporter, (object) new DeploymentProgressChangedEventArgs(e.Progress, this.userState, e.BytesCompleted, e.BytesTotal, DeploymentProgressState.DownloadingApplicationFiles, this.groupName));
    }

    public void DownloadCompleted(object sender, DownloadEventArgs e)
    {
    }
  }
}
