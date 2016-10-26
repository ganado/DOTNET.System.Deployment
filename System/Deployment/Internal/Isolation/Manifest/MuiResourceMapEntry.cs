// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.Manifest.MuiResourceMapEntry
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
  [StructLayout(LayoutKind.Sequential)]
  internal class MuiResourceMapEntry : IDisposable
  {
    [MarshalAs(UnmanagedType.SysInt)]
    public IntPtr ResourceTypeIdInt;
    public uint ResourceTypeIdIntSize;
    [MarshalAs(UnmanagedType.SysInt)]
    public IntPtr ResourceTypeIdString;
    public uint ResourceTypeIdStringSize;

    ~MuiResourceMapEntry()
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
      if (this.ResourceTypeIdInt != IntPtr.Zero)
      {
        Marshal.FreeCoTaskMem(this.ResourceTypeIdInt);
        this.ResourceTypeIdInt = IntPtr.Zero;
      }
      if (this.ResourceTypeIdString != IntPtr.Zero)
      {
        Marshal.FreeCoTaskMem(this.ResourceTypeIdString);
        this.ResourceTypeIdString = IntPtr.Zero;
      }
      if (!fDisposing)
        return;
      GC.SuppressFinalize((object) this);
    }
  }
}
