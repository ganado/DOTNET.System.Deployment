// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.Manifest.AssemblyReferenceDependentAssemblyEntry
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
  [StructLayout(LayoutKind.Sequential)]
  internal class AssemblyReferenceDependentAssemblyEntry : IDisposable
  {
    [MarshalAs(UnmanagedType.LPWStr)]
    public string Group;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string Codebase;
    public ulong Size;
    [MarshalAs(UnmanagedType.SysInt)]
    public IntPtr HashValue;
    public uint HashValueSize;
    public uint HashAlgorithm;
    public uint Flags;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string ResourceFallbackCulture;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string Description;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string SupportUrl;
    public ISection HashElements;

    ~AssemblyReferenceDependentAssemblyEntry()
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
      GC.SuppressFinalize((object) this);
    }
  }
}
