// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.IStateManager
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation
{
  [Guid("07662534-750b-4ed5-9cfb-1c5bc5acfd07")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  internal interface IStateManager
  {
    [SecurityCritical]
    void PrepareApplicationState([In] UIntPtr Inputs, ref UIntPtr Outputs);

    [SecurityCritical]
    void SetApplicationRunningState([In] uint Flags, [In] IActContext Context, [In] uint RunningState, out uint Disposition);

    [SecurityCritical]
    void GetApplicationStateFilesystemLocation([In] uint Flags, [In] IDefinitionAppId Appidentity, [In] IDefinitionIdentity ComponentIdentity, [In] UIntPtr Coordinates, [MarshalAs(UnmanagedType.LPWStr)] out string Path);

    [SecurityCritical]
    void Scavenge([In] uint Flags, out uint Disposition);
  }
}
