// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.CompatibleFrameworks
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Collections.Generic;
using System.Deployment.Application.Manifest;
using System.Deployment.Internal.Isolation.Manifest;

namespace System.Deployment.Application
{
  /// <summary>Provides details about versions of the .NET Framework on which this application can install and run.</summary>
  [Serializable]
  public class CompatibleFrameworks
  {
    private readonly Uri _supportUrl;
    private readonly CompatibleFramework[] _frameworks;

    /// <summary>Gets a <see cref="T:System.Uri" /> that provides the user with instructions for installing a version of the .NET Framework on which this application can install and run.</summary>
    /// <returns>A <see cref="T:System.Uri" /> that provides the user with instructions for installing a version of the .NET Framework on which this application can install and run.</returns>
    public Uri SupportUrl
    {
      get
      {
        return this._supportUrl;
      }
    }

    /// <summary>Gets a list of .NET Framework versions on which this application can install and run.</summary>
    /// <returns>A list of <see cref="T:System.Deployment.Application.CompatibleFramework" /> objects that specify the .NET Framework versions on which this application can install and run.</returns>
    public IList<CompatibleFramework> Frameworks
    {
      get
      {
        return (IList<CompatibleFramework>) Array.AsReadOnly<CompatibleFramework>(this._frameworks);
      }
    }

    internal CompatibleFrameworks(CompatibleFrameworksMetadataEntry compatibleFrameworksMetadataEntry, CompatibleFramework[] frameworks)
    {
      this._supportUrl = AssemblyManifest.UriFromMetadataEntry(compatibleFrameworksMetadataEntry.SupportUrl, "Ex_CompatibleFrameworksSupportUrlNotValid");
      this._frameworks = frameworks;
    }
  }
}
