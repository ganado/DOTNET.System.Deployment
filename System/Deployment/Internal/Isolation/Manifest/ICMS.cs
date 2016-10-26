// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.Manifest.ICMS
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("a504e5b0-8ccf-4cb4-9902-c9d1b9abd033")]
  [ComImport]
  internal interface ICMS
  {
    IDefinitionIdentity Identity { [SecurityCritical] get; }

    ISection FileSection { [SecurityCritical] get; }

    ISection CategoryMembershipSection { [SecurityCritical] get; }

    ISection COMRedirectionSection { [SecurityCritical] get; }

    ISection ProgIdRedirectionSection { [SecurityCritical] get; }

    ISection CLRSurrogateSection { [SecurityCritical] get; }

    ISection AssemblyReferenceSection { [SecurityCritical] get; }

    ISection WindowClassSection { [SecurityCritical] get; }

    ISection StringSection { [SecurityCritical] get; }

    ISection EntryPointSection { [SecurityCritical] get; }

    ISection PermissionSetSection { [SecurityCritical] get; }

    ISectionEntry MetadataSectionEntry { [SecurityCritical] get; }

    ISection AssemblyRequestSection { [SecurityCritical] get; }

    ISection RegistryKeySection { [SecurityCritical] get; }

    ISection DirectorySection { [SecurityCritical] get; }

    ISection FileAssociationSection { [SecurityCritical] get; }

    ISection CompatibleFrameworksSection { [SecurityCritical] get; }

    ISection EventSection { [SecurityCritical] get; }

    ISection EventMapSection { [SecurityCritical] get; }

    ISection EventTagSection { [SecurityCritical] get; }

    ISection CounterSetSection { [SecurityCritical] get; }

    ISection CounterSection { [SecurityCritical] get; }
  }
}
