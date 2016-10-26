// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.Manifest.HashElementEntry
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
  [StructLayout(LayoutKind.Sequential)]
  internal class HashElementEntry : IDisposable
  {
    public uint index;
    public byte Transform;
    [MarshalAs(UnmanagedType.SysInt)]
    public IntPtr TransformMetadata;
    public uint TransformMetadataSize;
    public byte DigestMethod;
    [MarshalAs(UnmanagedType.SysInt)]
    public IntPtr DigestValue;
    public uint DigestValueSize;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string Xml;

    ~HashElementEntry()
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
      if (this.TransformMetadata != IntPtr.Zero)
      {
        Marshal.FreeCoTaskMem(this.TransformMetadata);
        this.TransformMetadata = IntPtr.Zero;
      }
      if (this.DigestValue != IntPtr.Zero)
      {
        Marshal.FreeCoTaskMem(this.DigestValue);
        this.DigestValue = IntPtr.Zero;
      }
      if (!fDisposing)
        return;
      GC.SuppressFinalize((object) this);
    }
  }
}
