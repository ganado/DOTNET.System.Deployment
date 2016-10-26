// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.IManifestInformation
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation
{
  [Guid("81c85208-fe61-4c15-b5bb-ff5ea66baad9")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  internal interface IManifestInformation
  {
    [SecurityCritical]
    void get_FullPath([MarshalAs(UnmanagedType.LPWStr)] out string FullPath);
  }
}
