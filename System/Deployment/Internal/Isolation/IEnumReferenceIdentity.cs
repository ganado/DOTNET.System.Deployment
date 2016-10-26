// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.IEnumReferenceIdentity
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation
{
  [Guid("b30352cf-23da-4577-9b3f-b4e6573be53b")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  internal interface IEnumReferenceIdentity
  {
    [SecurityCritical]
    uint Next([In] uint celt, [MarshalAs(UnmanagedType.LPArray), Out] IReferenceIdentity[] ReferenceIdentity);

    [SecurityCritical]
    void Skip(uint celt);

    [SecurityCritical]
    void Reset();

    [SecurityCritical]
    IEnumReferenceIdentity Clone();
  }
}
