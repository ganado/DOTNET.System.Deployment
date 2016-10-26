// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.StoreOperationPinDeployment
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation
{
  internal struct StoreOperationPinDeployment
  {
    [MarshalAs(UnmanagedType.U4)]
    public uint Size;
    [MarshalAs(UnmanagedType.U4)]
    public StoreOperationPinDeployment.OpFlags Flags;
    [MarshalAs(UnmanagedType.Interface)]
    public IDefinitionAppId Application;
    [MarshalAs(UnmanagedType.I8)]
    public long ExpirationTime;
    public IntPtr Reference;

    [SecuritySafeCritical]
    public StoreOperationPinDeployment(IDefinitionAppId AppId, StoreApplicationReference Ref)
    {
      this.Size = (uint) Marshal.SizeOf(typeof (StoreOperationPinDeployment));
      this.Flags = StoreOperationPinDeployment.OpFlags.NeverExpires;
      this.Application = AppId;
      this.Reference = Ref.ToIntPtr();
      this.ExpirationTime = 0L;
    }

    public StoreOperationPinDeployment(IDefinitionAppId AppId, DateTime Expiry, StoreApplicationReference Ref)
    {
      this = new StoreOperationPinDeployment(AppId, Ref);
      this.Flags = this.Flags | StoreOperationPinDeployment.OpFlags.NeverExpires;
    }

    [SecurityCritical]
    public void Destroy()
    {
      StoreApplicationReference.Destroy(this.Reference);
    }

    [System.Flags]
    public enum OpFlags
    {
      Nothing = 0,
      NeverExpires = 1,
    }

    public enum Disposition
    {
      Failed,
      Pinned,
    }
  }
}
