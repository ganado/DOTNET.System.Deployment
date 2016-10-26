// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.StoreApplicationReference
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation
{
  internal struct StoreApplicationReference
  {
    [MarshalAs(UnmanagedType.U4)]
    public uint Size;
    [MarshalAs(UnmanagedType.U4)]
    public StoreApplicationReference.RefFlags Flags;
    public Guid GuidScheme;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string Identifier;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string NonCanonicalData;

    public StoreApplicationReference(Guid RefScheme, string Id, string NcData)
    {
      this.Size = (uint) Marshal.SizeOf(typeof (StoreApplicationReference));
      this.Flags = StoreApplicationReference.RefFlags.Nothing;
      this.GuidScheme = RefScheme;
      this.Identifier = Id;
      this.NonCanonicalData = NcData;
    }

    [SecurityCritical]
    public IntPtr ToIntPtr()
    {
      IntPtr ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf((object) this));
      Marshal.StructureToPtr((object) this, ptr, false);
      return ptr;
    }

    [SecurityCritical]
    public static void Destroy(IntPtr ip)
    {
      if (!(ip != IntPtr.Zero))
        return;
      Marshal.DestroyStructure(ip, typeof (StoreApplicationReference));
      Marshal.FreeCoTaskMem(ip);
    }

    [System.Flags]
    public enum RefFlags
    {
      Nothing = 0,
    }
  }
}
