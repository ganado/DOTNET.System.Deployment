// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.Manifest.IAssemblyReferenceDependentAssemblyEntry
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("C31FF59E-CD25-47b8-9EF3-CF4433EB97CC")]
  [ComImport]
  internal interface IAssemblyReferenceDependentAssemblyEntry
  {
    AssemblyReferenceDependentAssemblyEntry AllData { [SecurityCritical] get; }

    string Group { [SecurityCritical] get; }

    string Codebase { [SecurityCritical] get; }

    ulong Size { [SecurityCritical] get; }

    object HashValue { [SecurityCritical] get; }

    uint HashAlgorithm { [SecurityCritical] get; }

    uint Flags { [SecurityCritical] get; }

    string ResourceFallbackCulture { [SecurityCritical] get; }

    string Description { [SecurityCritical] get; }

    string SupportUrl { [SecurityCritical] get; }

    ISection HashElements { [SecurityCritical] get; }
  }
}
