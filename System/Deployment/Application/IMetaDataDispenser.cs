// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.IMetaDataDispenser
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;

namespace System.Deployment.Application
{
  [Guid("809c652e-7396-11d2-9771-00a0c9b4d50c")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [TypeLibType(TypeLibTypeFlags.FRestricted)]
  [ComImport]
  internal interface IMetaDataDispenser
  {
    [return: MarshalAs(UnmanagedType.Interface)]
    object DefineScope([In] ref Guid rclsid, [In] uint dwCreateFlags, [In] ref Guid riid);

    [return: MarshalAs(UnmanagedType.Interface)]
    object OpenScope([MarshalAs(UnmanagedType.LPWStr), In] string szScope, [In] uint dwOpenFlags, [In] ref Guid riid);

    [return: MarshalAs(UnmanagedType.Interface)]
    object OpenScopeOnMemory([In] IntPtr pData, [In] uint cbData, [In] uint dwOpenFlags, [In] ref Guid riid);
  }
}
