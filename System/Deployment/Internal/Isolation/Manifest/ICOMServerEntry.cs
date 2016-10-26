// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.Manifest.ICOMServerEntry
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("3903B11B-FBE8-477c-825F-DB828B5FD174")]
  [ComImport]
  internal interface ICOMServerEntry
  {
    COMServerEntry AllData { [SecurityCritical] get; }

    Guid Clsid { [SecurityCritical] get; }

    uint Flags { [SecurityCritical] get; }

    Guid ConfiguredGuid { [SecurityCritical] get; }

    Guid ImplementedClsid { [SecurityCritical] get; }

    Guid TypeLibrary { [SecurityCritical] get; }

    uint ThreadingModel { [SecurityCritical] get; }

    string RuntimeVersion { [SecurityCritical] get; }

    string HostFile { [SecurityCritical] get; }
  }
}
