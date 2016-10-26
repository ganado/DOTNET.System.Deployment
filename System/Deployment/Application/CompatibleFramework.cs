// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.CompatibleFramework
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Deployment.Internal.Isolation.Manifest;

namespace System.Deployment.Application
{
  /// <summary>Represents a version of the .NET Framework where an application can install and run.</summary>
  [Serializable]
  public class CompatibleFramework
  {
    private readonly string _supportedRuntime;
    private readonly string _profile;
    private readonly string _targetVersion;

    /// <summary>Specifies the runtime version number of the .NET Framework where an application can install and run.</summary>
    /// <returns>A string that represents the runtime version.</returns>
    public string SupportedRuntime
    {
      get
      {
        return this._supportedRuntime;
      }
    }

    /// <summary>Specifies the profile of the .NET Framework version where an application can install and run.</summary>
    /// <returns>A string that represents the profile.</returns>
    public string Profile
    {
      get
      {
        return this._profile;
      }
    }

    /// <summary>Specifies the version of the .NET Framework where an application can install and run.</summary>
    /// <returns>A string that represents the version.</returns>
    public string TargetVersion
    {
      get
      {
        return this._targetVersion;
      }
    }

    internal CompatibleFramework(CompatibleFrameworkEntry compatibleFrameworkEntry)
    {
      this._supportedRuntime = compatibleFrameworkEntry.SupportedRuntime;
      this._profile = compatibleFrameworkEntry.Profile;
      this._targetVersion = compatibleFrameworkEntry.TargetVersion;
    }
  }
}
