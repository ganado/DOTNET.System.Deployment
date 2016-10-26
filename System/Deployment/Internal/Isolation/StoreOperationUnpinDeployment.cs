// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.StoreOperationUnpinDeployment
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation
{
  internal struct StoreOperationUnpinDeployment
  {
    [MarshalAs(UnmanagedType.U4)]
    public uint Size;
    [MarshalAs(UnmanagedType.U4)]
    public StoreOperationUnpinDeployment.OpFlags Flags;
    [MarshalAs(UnmanagedType.Interface)]
    public IDefinitionAppId Application;
    public IntPtr Reference;

    [SecuritySafeCritical]
    public StoreOperationUnpinDeployment(IDefinitionAppId app, StoreApplicationReference reference)
    {
      this.Size = (uint) Marshal.SizeOf(typeof (StoreOperationUnpinDeployment));
      this.Flags = StoreOperationUnpinDeployment.OpFlags.Nothing;
      this.Application = app;
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
    }

    public enum Disposition
    {
      Failed,
      Unpinned,
    }
  }
}
