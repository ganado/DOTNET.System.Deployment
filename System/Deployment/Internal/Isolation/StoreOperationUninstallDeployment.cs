// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.StoreOperationUninstallDeployment
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation
{
  internal struct StoreOperationUninstallDeployment
  {
    [MarshalAs(UnmanagedType.U4)]
    public uint Size;
    [MarshalAs(UnmanagedType.U4)]
    public StoreOperationUninstallDeployment.OpFlags Flags;
    [MarshalAs(UnmanagedType.Interface)]
    public IDefinitionAppId Application;
    public IntPtr Reference;

    [SecuritySafeCritical]
    public StoreOperationUninstallDeployment(IDefinitionAppId appid, StoreApplicationReference AppRef)
    {
      this.Size = (uint) Marshal.SizeOf(typeof (StoreOperationUninstallDeployment));
      this.Flags = StoreOperationUninstallDeployment.OpFlags.Nothing;
      this.Application = appid;
      this.Reference = AppRef.ToIntPtr();
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
    }

    public enum Disposition
    {
      Failed,
      DidNotExist,
      Uninstalled,
    }
  }
}
