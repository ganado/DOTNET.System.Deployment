// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.DownloadFileGroupCompletedEventArgs
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.ComponentModel;

namespace System.Deployment.Application
{
  /// <summary>Describes a file download that has recently completed. </summary>
  public class DownloadFileGroupCompletedEventArgs : AsyncCompletedEventArgs
  {
    private readonly string _groupName;

    /// <summary>Gets the name of the file group being downloaded.</summary>
    /// <returns>A <see cref="T:System.String" /> representing the name of the file group.</returns>
    public string Group
    {
      get
      {
        return this._groupName;
      }
    }

    internal DownloadFileGroupCompletedEventArgs(Exception error, bool cancelled, object userState, string groupName)
      : base(error, cancelled, userState)
    {
      this._groupName = groupName;
    }
  }
}
