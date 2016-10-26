// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.Manifest.DependentOS
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Deployment.Internal.Isolation.Manifest;

namespace System.Deployment.Application.Manifest
{
  internal class DependentOS
  {
    private readonly ushort _majorVersion;
    private readonly ushort _minorVersion;
    private readonly ushort _buildNumber;
    private readonly byte _servicePackMajor;
    private readonly byte _servicePackMinor;
    private readonly Uri _supportUrl;

    public ushort MajorVersion
    {
      get
      {
        return this._majorVersion;
      }
    }

    public ushort MinorVersion
    {
      get
      {
        return this._minorVersion;
      }
    }

    public ushort BuildNumber
    {
      get
      {
        return this._buildNumber;
      }
    }

    public byte ServicePackMajor
    {
      get
      {
        return this._servicePackMajor;
      }
    }

    public byte ServicePackMinor
    {
      get
      {
        return this._servicePackMinor;
      }
    }

    public Uri SupportUrl
    {
      get
      {
        return this._supportUrl;
      }
    }

    public DependentOS(DependentOSMetadataEntry dependentOSMetadataEntry)
    {
      this._majorVersion = dependentOSMetadataEntry.MajorVersion;
      this._minorVersion = dependentOSMetadataEntry.MinorVersion;
      this._buildNumber = dependentOSMetadataEntry.BuildNumber;
      this._servicePackMajor = dependentOSMetadataEntry.ServicePackMajor;
      this._servicePackMinor = dependentOSMetadataEntry.ServicePackMinor;
      this._supportUrl = AssemblyManifest.UriFromMetadataEntry(dependentOSMetadataEntry.SupportUrl, "Ex_DependentOSSupportUrlNotValid");
    }
  }
}
