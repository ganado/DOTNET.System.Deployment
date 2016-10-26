// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.Manifest.MetadataSectionEntry
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
  [StructLayout(LayoutKind.Sequential)]
  internal class MetadataSectionEntry : IDisposable
  {
    public uint SchemaVersion;
    public uint ManifestFlags;
    public uint UsagePatterns;
    public IDefinitionIdentity CdfIdentity;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string LocalPath;
    public uint HashAlgorithm;
    [MarshalAs(UnmanagedType.SysInt)]
    public IntPtr ManifestHash;
    public uint ManifestHashSize;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string ContentType;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string RuntimeImageVersion;
    [MarshalAs(UnmanagedType.SysInt)]
    public IntPtr MvidValue;
    public uint MvidValueSize;
    public DescriptionMetadataEntry DescriptionData;
    public DeploymentMetadataEntry DeploymentData;
    public DependentOSMetadataEntry DependentOSData;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string defaultPermissionSetID;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string RequestedExecutionLevel;
    public bool RequestedExecutionLevelUIAccess;
    public IReferenceIdentity ResourceTypeResourcesDependency;
    public IReferenceIdentity ResourceTypeManifestResourcesDependency;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string KeyInfoElement;
    public CompatibleFrameworksMetadataEntry CompatibleFrameworksData;

    ~MetadataSectionEntry()
    {
      this.Dispose(false);
    }

    void IDisposable.Dispose()
    {
      this.Dispose(true);
    }

    [SecuritySafeCritical]
    public void Dispose(bool fDisposing)
    {
      if (this.ManifestHash != IntPtr.Zero)
      {
        Marshal.FreeCoTaskMem(this.ManifestHash);
        this.ManifestHash = IntPtr.Zero;
      }
      if (this.MvidValue != IntPtr.Zero)
      {
        Marshal.FreeCoTaskMem(this.MvidValue);
        this.MvidValue = IntPtr.Zero;
      }
      if (!fDisposing)
        return;
      GC.SuppressFinalize((object) this);
    }
  }
}
