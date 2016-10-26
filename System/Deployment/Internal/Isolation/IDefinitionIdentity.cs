// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.IDefinitionIdentity
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation
{
  [Guid("587bf538-4d90-4a3c-9ef1-58a200a8a9e7")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  internal interface IDefinitionIdentity
  {
    [SecurityCritical]
    [return: MarshalAs(UnmanagedType.LPWStr)]
    string GetAttribute([MarshalAs(UnmanagedType.LPWStr), In] string Namespace, [MarshalAs(UnmanagedType.LPWStr), In] string Name);

    [SecurityCritical]
    void SetAttribute([MarshalAs(UnmanagedType.LPWStr), In] string Namespace, [MarshalAs(UnmanagedType.LPWStr), In] string Name, [MarshalAs(UnmanagedType.LPWStr), In] string Value);

    [SecurityCritical]
    IEnumIDENTITY_ATTRIBUTE EnumAttributes();

    [SecurityCritical]
    IDefinitionIdentity Clone([In] IntPtr cDeltas, [MarshalAs(UnmanagedType.LPArray), In] IDENTITY_ATTRIBUTE[] Deltas);
  }
}
