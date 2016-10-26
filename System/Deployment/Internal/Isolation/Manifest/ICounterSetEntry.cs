// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.Manifest.ICounterSetEntry
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("8CD3FC85-AFD3-477a-8FD5-146C291195BB")]
  [ComImport]
  internal interface ICounterSetEntry
  {
    CounterSetEntry AllData { [SecurityCritical] get; }

    Guid CounterSetGuid { [SecurityCritical] get; }

    Guid ProviderGuid { [SecurityCritical] get; }

    string Name { [SecurityCritical] get; }

    string Description { [SecurityCritical] get; }

    bool InstanceType { [SecurityCritical] get; }
  }
}
