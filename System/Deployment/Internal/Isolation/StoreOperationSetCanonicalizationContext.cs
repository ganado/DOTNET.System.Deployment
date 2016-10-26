// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.StoreOperationSetCanonicalizationContext
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation
{
  internal struct StoreOperationSetCanonicalizationContext
  {
    [MarshalAs(UnmanagedType.U4)]
    public uint Size;
    [MarshalAs(UnmanagedType.U4)]
    public StoreOperationSetCanonicalizationContext.OpFlags Flags;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string BaseAddressFilePath;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string ExportsFilePath;

    [SecurityCritical]
    public StoreOperationSetCanonicalizationContext(string Bases, string Exports)
    {
      this.Size = (uint) Marshal.SizeOf(typeof (StoreOperationSetCanonicalizationContext));
      this.Flags = StoreOperationSetCanonicalizationContext.OpFlags.Nothing;
      this.BaseAddressFilePath = Bases;
      this.ExportsFilePath = Exports;
    }

    public void Destroy()
    {
    }

    [System.Flags]
    public enum OpFlags
    {
      Nothing = 0,
    }
  }
}
