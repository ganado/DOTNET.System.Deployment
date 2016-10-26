// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.Manifest.Deployment
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Deployment.Internal.Isolation.Manifest;

namespace System.Deployment.Application.Manifest
{
  internal class Deployment
  {
    private readonly Uri _codebaseUri;
    private readonly DeploymentUpdate _update;
    private readonly Version _minimumRequiredVersion;
    private readonly bool _disallowUrlActivation;
    private readonly bool _install;
    private readonly bool _trustURLParameters;
    private readonly bool _mapFileExtensions;
    private readonly bool _createDesktopShortcut;

    public Uri ProviderCodebaseUri
    {
      get
      {
        return this._codebaseUri;
      }
    }

    public DeploymentUpdate DeploymentUpdate
    {
      get
      {
        return this._update;
      }
    }

    public Version MinimumRequiredVersion
    {
      get
      {
        return this._minimumRequiredVersion;
      }
    }

    public bool DisallowUrlActivation
    {
      get
      {
        return this._disallowUrlActivation;
      }
    }

    public bool Install
    {
      get
      {
        return this._install;
      }
    }

    public bool TrustURLParameters
    {
      get
      {
        return this._trustURLParameters;
      }
    }

    public bool MapFileExtensions
    {
      get
      {
        return this._mapFileExtensions;
      }
    }

    public bool CreateDesktopShortcut
    {
      get
      {
        return this._createDesktopShortcut;
      }
    }

    public bool IsUpdateSectionPresent
    {
      get
      {
        return this.DeploymentUpdate.BeforeApplicationStartup || this.DeploymentUpdate.MaximumAgeSpecified;
      }
    }

    public bool IsInstalledAndNoDeploymentProvider
    {
      get
      {
        if (this.Install)
          return this.ProviderCodebaseUri == (Uri) null;
        return false;
      }
    }

    public Deployment(DeploymentMetadataEntry deploymentMetadataEntry)
    {
      this._disallowUrlActivation = (deploymentMetadataEntry.DeploymentFlags & 128U) > 0U;
      this._install = (deploymentMetadataEntry.DeploymentFlags & 32U) > 0U;
      this._trustURLParameters = (deploymentMetadataEntry.DeploymentFlags & 64U) > 0U;
      this._mapFileExtensions = (deploymentMetadataEntry.DeploymentFlags & 256U) > 0U;
      this._createDesktopShortcut = (deploymentMetadataEntry.DeploymentFlags & 512U) > 0U;
      this._update = new DeploymentUpdate(deploymentMetadataEntry);
      this._minimumRequiredVersion = deploymentMetadataEntry.MinimumRequiredVersion != null ? new Version(deploymentMetadataEntry.MinimumRequiredVersion) : (Version) null;
      this._codebaseUri = AssemblyManifest.UriFromMetadataEntry(deploymentMetadataEntry.DeploymentProviderCodebase, "Ex_DepProviderNotValid");
    }
  }
}
