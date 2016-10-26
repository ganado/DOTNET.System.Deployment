// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.IEnumUnknown
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Deployment.Internal.Isolation
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("00000100-0000-0000-C000-000000000046")]
  [ComImport]
  internal interface IEnumUnknown
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int Next(uint celt, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.IUnknown), Out] object[] rgelt, ref uint celtFetched);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int Skip(uint celt);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int Reset();

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int Clone(out IEnumUnknown enumUnknown);
  }
}
