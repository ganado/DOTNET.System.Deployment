// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.Manifest.IEventEntry
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("8AD3FC86-AFD3-477a-8FD5-146C291195BB")]
  [ComImport]
  internal interface IEventEntry
  {
    EventEntry AllData { [SecurityCritical] get; }

    uint EventID { [SecurityCritical] get; }

    uint Level { [SecurityCritical] get; }

    uint Version { [SecurityCritical] get; }

    Guid Guid { [SecurityCritical] get; }

    string SubTypeName { [SecurityCritical] get; }

    uint SubTypeValue { [SecurityCritical] get; }

    string DisplayName { [SecurityCritical] get; }

    uint EventNameMicrodomIndex { [SecurityCritical] get; }
  }
}
