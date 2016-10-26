// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.Manifest.FileEntry
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
  [StructLayout(LayoutKind.Sequential)]
  internal class FileEntry : IDisposable
  {
    [MarshalAs(UnmanagedType.LPWStr)]
    public string Name;
    public uint HashAlgorithm;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string LoadFrom;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string SourcePath;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string ImportPath;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string SourceName;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string Location;
    [MarshalAs(UnmanagedType.SysInt)]
    public IntPtr HashValue;
    public uint HashValueSize;
    public ulong Size;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string Group;
    public uint Flags;
    public MuiResourceMapEntry MuiMapping;
    public uint WritableType;
    public ISection HashElements;

    ~FileEntry()
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
      if (this.HashValue != IntPtr.Zero)
      {
        Marshal.FreeCoTaskMem(this.HashValue);
        this.HashValue = IntPtr.Zero;
      }
      if (!fDisposing)
        return;
      if (this.MuiMapping != null)
      {
        this.MuiMapping.Dispose(true);
        this.MuiMapping = (MuiResourceMapEntry) null;
      }
      GC.SuppressFinalize((object) this);
    }
  }
}
