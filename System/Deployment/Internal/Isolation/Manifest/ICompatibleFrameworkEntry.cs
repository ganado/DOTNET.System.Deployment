// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.Manifest.ICompatibleFrameworkEntry
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("C98BFE2A-62C9-40AD-ADCE-A9037BE2BE6C")]
  [ComImport]
  internal interface ICompatibleFrameworkEntry
  {
    CompatibleFrameworkEntry AllData { [SecurityCritical] get; }

    uint index { [SecurityCritical] get; }

    string TargetVersion { [SecurityCritical] get; }

    string Profile { [SecurityCritical] get; }

    string SupportedRuntime { [SecurityCritical] get; }
  }
}
