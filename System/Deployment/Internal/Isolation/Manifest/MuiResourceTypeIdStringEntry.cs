// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.Manifest.MuiResourceTypeIdStringEntry
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
  [StructLayout(LayoutKind.Sequential)]
  internal class MuiResourceTypeIdStringEntry : IDisposable
  {
    [MarshalAs(UnmanagedType.SysInt)]
    public IntPtr StringIds;
    public uint StringIdsSize;
    [MarshalAs(UnmanagedType.SysInt)]
    public IntPtr IntegerIds;
    public uint IntegerIdsSize;

    ~MuiResourceTypeIdStringEntry()
    {
      this.Dispose(false);
    }

    void IDisposable.Dispose()
    {
      this.Dispose(true);
    }

    [SecuritySafeCritical]
    public void Dispose(bool fDisposing)
    {
      if (this.StringIds != IntPtr.Zero)
      {
        Marshal.FreeCoTaskMem(this.StringIds);
        this.StringIds = IntPtr.Zero;
      }
      if (this.IntegerIds != IntPtr.Zero)
      {
        Marshal.FreeCoTaskMem(this.IntegerIds);
        this.IntegerIds = IntPtr.Zero;
      }
      if (!fDisposing)
        return;
      GC.SuppressFinalize((object) this);
    }
  }
}
