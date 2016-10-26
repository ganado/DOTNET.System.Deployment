// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.DownloadProgressChangedEventArgs
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.ComponentModel;

namespace System.Deployment.Application
{
  /// <summary>Provides data for the <see cref="E:System.Deployment.Application.InPlaceHostingManager.DownloadProgressChanged" /> event. </summary>
  public class DownloadProgressChangedEventArgs : ProgressChangedEventArgs
  {
    private long _bytesCompleted;
    private long _bytesTotal;
    private DeploymentProgressState _deploymentProgressState;

    /// <summary>Gets the number of bytes downloaded to the local computer.</summary>
    /// <returns>An <see cref="T:System.Int64" /> representing the number of downloaded bytes. </returns>
    public long BytesDownloaded
    {
      get
      {
        return this._bytesCompleted;
      }
    }

    /// <summary>Gets the total number of bytes for the download operation. </summary>
    /// <returns>An <see cref="T:System.Int64" /> representing the total size of the download, in bytes.</returns>
    public long TotalBytesToDownload
    {
      get
      {
        return this._bytesTotal;
      }
    }

    /// <summary>Gets the progress state of the download.</summary>
    /// <returns>A <see cref="T:System.Deployment.Application.DeploymentProgressState" /> value describing which portion of the ClickOnce application is being downloaded.</returns>
    public DeploymentProgressState State
    {
      get
      {
        return this._deploymentProgressState;
      }
    }

    internal DownloadProgressChangedEventArgs(int progressPercentage, object userState, long bytesCompleted, long bytesTotal, DeploymentProgressState downloadProgressState)
      : base(progressPercentage, userState)
    {
      this._bytesCompleted = bytesCompleted;
      this._bytesTotal = bytesTotal;
      this._deploymentProgressState = downloadProgressState;
    }
  }
}
