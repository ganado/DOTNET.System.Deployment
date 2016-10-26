// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.IReferenceAppId
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation
{
  [Guid("054f0bef-9e45-4363-8f5a-2f8e142d9a3b")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  internal interface IReferenceAppId
  {
    [SecurityCritical]
    [return: MarshalAs(UnmanagedType.LPWStr)]
    string get_SubscriptionId();

    void put_SubscriptionId([MarshalAs(UnmanagedType.LPWStr), In] string Subscription);

    [SecurityCritical]
    [return: MarshalAs(UnmanagedType.LPWStr)]
    string get_Codebase();

    void put_Codebase([MarshalAs(UnmanagedType.LPWStr), In] string CodeBase);

    [SecurityCritical]
    IEnumReferenceIdentity EnumAppPath();
  }
}
