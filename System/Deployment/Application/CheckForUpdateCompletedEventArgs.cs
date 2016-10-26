// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.CheckForUpdateCompletedEventArgs
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.ComponentModel;

namespace System.Deployment.Application
{
  /// <summary>Represents detailed update information obtained through a call to <see cref="M:System.Deployment.Application.ApplicationDeployment.CheckForUpdateAsync" />.</summary>
  public class CheckForUpdateCompletedEventArgs : AsyncCompletedEventArgs
  {
    private readonly bool _updateAvailable;
    private readonly Version _availableVersion;
    private readonly bool _isUpdateRequired;
    private readonly Version _minimumRequiredVersion;
    private readonly long _updateSize;

    /// <summary>Gets whether an uninstalled update is available.</summary>
    /// <returns>true if new version of the application is available; otherwise, false.</returns>
    public bool UpdateAvailable
    {
      get
      {
        this.RaiseExceptionIfNecessary();
        return this._updateAvailable;
      }
    }

    /// <summary>Gets the version number of the latest uninstalled version.</summary>
    /// <returns>The <see cref="T:System.Version" /> expressing the major, minor, build and revision numbers of the latest version.</returns>
    public Version AvailableVersion
    {
      get
      {
        this.RaiseExceptionIfUpdateNotAvailable();
        return this._availableVersion;
      }
    }

    /// <summary>Gets a value indicating whether the update must be installed. </summary>
    /// <returns>true if the update is required; otherwise, false.</returns>
    public bool IsUpdateRequired
    {
      get
      {
        this.RaiseExceptionIfUpdateNotAvailable();
        return this._isUpdateRequired;
      }
    }

    /// <summary>Gets the minimum version that the user must have installed on the computer. </summary>
    /// <returns>A <see cref="T:System.Version" /> object expressing the earliest version that all users must install.</returns>
    public Version MinimumRequiredVersion
    {
      get
      {
        this.RaiseExceptionIfUpdateNotAvailable();
        return this._minimumRequiredVersion;
      }
    }

    /// <summary>Gets the size of the available update.</summary>
    /// <returns>An <see cref="T:System.Int64" /> describing the size, in bytes, of the available update. If no update is available, returns 0. </returns>
    public long UpdateSizeBytes
    {
      get
      {
        this.RaiseExceptionIfUpdateNotAvailable();
        return this._updateSize;
      }
    }

    internal CheckForUpdateCompletedEventArgs(Exception error, bool cancelled, object userState, bool updateAvailable, Version availableVersion, bool isUpdateRequired, Version minimumRequiredVersion, long updateSize)
      : base(error, cancelled, userState)
    {
      this._updateAvailable = updateAvailable;
      this._availableVersion = availableVersion;
      this._isUpdateRequired = isUpdateRequired;
      this._minimumRequiredVersion = minimumRequiredVersion;
      this._updateSize = updateSize;
    }

    private void RaiseExceptionIfUpdateNotAvailable()
    {
      if (!this.UpdateAvailable)
        throw new InvalidOperationException(Resources.GetString("Ex_UpdateNotAvailable"));
    }
  }
}
