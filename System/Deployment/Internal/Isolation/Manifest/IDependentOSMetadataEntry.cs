// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.Manifest.IDependentOSMetadataEntry
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("CF168CF4-4E8F-4d92-9D2A-60E5CA21CF85")]
  [ComImport]
  internal interface IDependentOSMetadataEntry
  {
    DependentOSMetadataEntry AllData { [SecurityCritical] get; }

    string SupportUrl { [SecurityCritical] get; }

    string Description { [SecurityCritical] get; }

    ushort MajorVersion { [SecurityCritical] get; }

    ushort MinorVersion { [SecurityCritical] get; }

    ushort BuildNumber { [SecurityCritical] get; }

    byte ServicePackMajor { [SecurityCritical] get; }

    byte ServicePackMinor { [SecurityCritical] get; }
  }
}
