// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.Manifest.IMetadataSectionEntry
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("AB1ED79F-943E-407d-A80B-0744E3A95B28")]
  [ComImport]
  internal interface IMetadataSectionEntry
  {
    MetadataSectionEntry AllData { [SecurityCritical] get; }

    uint SchemaVersion { [SecurityCritical] get; }

    uint ManifestFlags { [SecurityCritical] get; }

    uint UsagePatterns { [SecurityCritical] get; }

    IDefinitionIdentity CdfIdentity { [SecurityCritical] get; }

    string LocalPath { [SecurityCritical] get; }

    uint HashAlgorithm { [SecurityCritical] get; }

    object ManifestHash { [SecurityCritical] get; }

    string ContentType { [SecurityCritical] get; }

    string RuntimeImageVersion { [SecurityCritical] get; }

    object MvidValue { [SecurityCritical] get; }

    IDescriptionMetadataEntry DescriptionData { [SecurityCritical] get; }

    IDeploymentMetadataEntry DeploymentData { [SecurityCritical] get; }

    IDependentOSMetadataEntry DependentOSData { [SecurityCritical] get; }

    string defaultPermissionSetID { [SecurityCritical] get; }

    string RequestedExecutionLevel { [SecurityCritical] get; }

    bool RequestedExecutionLevelUIAccess { [SecurityCritical] get; }

    IReferenceIdentity ResourceTypeResourcesDependency { [SecurityCritical] get; }

    IReferenceIdentity ResourceTypeManifestResourcesDependency { [SecurityCritical] get; }

    string KeyInfoElement { [SecurityCritical] get; }

    ICompatibleFrameworksMetadataEntry CompatibleFrameworksData { [SecurityCritical] get; }
  }
}
