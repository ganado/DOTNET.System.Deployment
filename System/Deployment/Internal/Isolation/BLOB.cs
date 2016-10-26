// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.BLOB
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation
{
  internal struct BLOB : IDisposable
  {
    [MarshalAs(UnmanagedType.U4)]
    public uint Size;
    [MarshalAs(UnmanagedType.SysInt)]
    public IntPtr BlobData;

    [SecuritySafeCritical]
    public void Dispose()
    {
      if (!(this.BlobData != IntPtr.Zero))
        return;
      Marshal.FreeCoTaskMem(this.BlobData);
      this.BlobData = IntPtr.Zero;
    }
  }
}
