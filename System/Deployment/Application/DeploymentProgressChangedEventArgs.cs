// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.DeploymentProgressChangedEventArgs
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.ComponentModel;

namespace System.Deployment.Application
{
  /// <summary>Represents progress data reported in an asynchronous operation.</summary>
  public class DeploymentProgressChangedEventArgs : ProgressChangedEventArgs
  {
    private readonly long _bytesCompleted;
    private readonly long _bytesTotal;
    private readonly DeploymentProgressState _state;
    private readonly string _groupName;

    /// <summary>Gets the number of bytes already downloaded by this operation.</summary>
    /// <returns>An <see cref="T:System.Int64" /> representing the data already transferred, in bytes. </returns>
    public long BytesCompleted
    {
      get
      {
        return this._bytesCompleted;
      }
    }

    /// <summary>Gets the total number of bytes in the download operation.</summary>
    /// <returns>An <see cref="T:System.Int64" /> representing the total size of the download, in bytes.</returns>
    public long BytesTotal
    {
      get
      {
        return this._bytesTotal;
      }
    }

    /// <summary>Gets the action that the process is currently executing.</summary>
    /// <returns>A <see cref="T:System.Deployment.Application.DeploymentProgressState" /> value, stating what element or elements the operation is currently downloading. </returns>
    public DeploymentProgressState State
    {
      get
      {
        return this._state;
      }
    }

    /// <summary>Gets the name of the file group being downloaded.</summary>
    /// <returns>A <see cref="T:System.String" /> containing the name of the file group, if the event occurred as the result of a call to <see cref="Overload:System.Deployment.Application.ApplicationDeployment.DownloadFileGroupAsync" />; otherwise, a zero-length string. </returns>
    public string Group
    {
      get
      {
        return this._groupName;
      }
    }

    internal DeploymentProgressChangedEventArgs(int progressPercentage, object userState, long bytesCompleted, long bytesTotal, DeploymentProgressState state, string groupName)
      : base(progressPercentage, userState)
    {
      this._bytesCompleted = bytesCompleted;
      this._bytesTotal = bytesTotal;
      this._state = state;
      this._groupName = groupName;
    }
  }
}
