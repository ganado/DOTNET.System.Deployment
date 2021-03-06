﻿// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.Manifest.IDeploymentMetadataEntry
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("CFA3F59F-334D-46bf-A5A5-5D11BB2D7EBC")]
  [ComImport]
  internal interface IDeploymentMetadataEntry
  {
    DeploymentMetadataEntry AllData { [SecurityCritical] get; }

    string DeploymentProviderCodebase { [SecurityCritical] get; }

    string MinimumRequiredVersion { [SecurityCritical] get; }

    ushort MaximumAge { [SecurityCritical] get; }

    byte MaximumAge_Unit { [SecurityCritical] get; }

    uint DeploymentFlags { [SecurityCritical] get; }
  }
}
