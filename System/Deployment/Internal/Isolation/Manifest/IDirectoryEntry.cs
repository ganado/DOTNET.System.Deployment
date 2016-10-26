// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.Manifest.IDirectoryEntry
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("9f27c750-7dfb-46a1-a673-52e53e2337a9")]
  [ComImport]
  internal interface IDirectoryEntry
  {
    DirectoryEntry AllData { [SecurityCritical] get; }

    uint Flags { [SecurityCritical] get; }

    uint Protection { [SecurityCritical] get; }

    string BuildFilter { [SecurityCritical] get; }

    object SecurityDescriptor { [SecurityCritical] get; }
  }
}
