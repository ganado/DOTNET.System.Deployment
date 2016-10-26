// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.IEnumSTORE_CATEGORY_SUBCATEGORY
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation
{
  [Guid("19be1967-b2fc-4dc1-9627-f3cb6305d2a7")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  internal interface IEnumSTORE_CATEGORY_SUBCATEGORY
  {
    [SecurityCritical]
    uint Next([In] uint celt, [MarshalAs(UnmanagedType.LPArray), Out] STORE_CATEGORY_SUBCATEGORY[] rgElements);

    [SecurityCritical]
    void Skip([In] uint ulElements);

    [SecurityCritical]
    void Reset();

    [SecurityCritical]
    IEnumSTORE_CATEGORY_SUBCATEGORY Clone();
  }
}
