// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.StoreOperationInstallDeployment
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation
{
  internal struct StoreOperationInstallDeployment
  {
    [MarshalAs(UnmanagedType.U4)]
    public uint Size;
    [MarshalAs(UnmanagedType.U4)]
    public StoreOperationInstallDeployment.OpFlags Flags;
    [MarshalAs(UnmanagedType.Interface)]
    public IDefinitionAppId Application;
    public IntPtr Reference;

    public StoreOperationInstallDeployment(IDefinitionAppId App, StoreApplicationReference reference)
    {
      this = new StoreOperationInstallDeployment(App, true, reference);
    }

    [SecuritySafeCritical]
    public StoreOperationInstallDeployment(IDefinitionAppId App, bool UninstallOthers, StoreApplicationReference reference)
    {
      this.Size = (uint) Marshal.SizeOf(typeof (StoreOperationInstallDeployment));
      this.Flags = StoreOperationInstallDeployment.OpFlags.Nothing;
      this.Application = App;
      if (UninstallOthers)
        this.Flags = this.Flags | StoreOperationInstallDeployment.OpFlags.UninstallOthers;
      this.Reference = reference.ToIntPtr();
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
      UninstallOthers = 1,
    }

    public enum Disposition
    {
      Failed,
      AlreadyInstalled,
      Installed,
    }
  }
}
