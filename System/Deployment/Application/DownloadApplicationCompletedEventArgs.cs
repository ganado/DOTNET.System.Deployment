// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.DownloadApplicationCompletedEventArgs
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.ComponentModel;

namespace System.Deployment.Application
{
  /// <summary>Provides data for the <see cref="E:System.Deployment.Application.InPlaceHostingManager.DownloadApplicationCompleted" /> event. </summary>
  public class DownloadApplicationCompletedEventArgs : AsyncCompletedEventArgs
  {
    private string _logFilePath;
    private string _shortcutAppId;

    /// <summary>Gets the path of the ClickOnce log file. </summary>
    /// <returns>The path of the ClickOnce log file.</returns>
    public string LogFilePath
    {
      get
      {
        return this._logFilePath;
      }
    }

    /// <summary>Gets the contents of an .appref-ms file that can launch this ClickOnce application.</summary>
    /// <returns>The contents of an .appref-ms file.</returns>
    public string ShortcutAppId
    {
      get
      {
        return this._shortcutAppId;
      }
    }

    internal DownloadApplicationCompletedEventArgs(AsyncCompletedEventArgs e, string logFilePath, string shortcutAppId)
      : base(e.Error, e.Cancelled, e.UserState)
    {
      this._logFilePath = logFilePath;
      this._shortcutAppId = shortcutAppId;
    }
  }
}
