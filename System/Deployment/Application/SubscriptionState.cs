// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.SubscriptionState
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Deployment.Application.Manifest;

namespace System.Deployment.Application
{
  internal class SubscriptionState
  {
    private SubscriptionStore _subStore;
    private DefinitionIdentity _subId;
    private bool _stateIsValid;
    private SubscriptionStateInternal state;

    public DefinitionIdentity SubscriptionId
    {
      get
      {
        return this._subId;
      }
    }

    public SubscriptionStore SubscriptionStore
    {
      get
      {
        return this._subStore;
      }
    }

    public bool IsInstalled
    {
      get
      {
        this.Validate();
        return this.state.IsInstalled;
      }
    }

    public bool IsShellVisible
    {
      get
      {
        this.Validate();
        return this.state.IsShellVisible;
      }
    }

    public DefinitionAppId CurrentBind
    {
      get
      {
        this.Validate();
        return this.state.CurrentBind;
      }
    }

    public DefinitionAppId PreviousBind
    {
      get
      {
        this.Validate();
        return this.state.PreviousBind;
      }
    }

    public DefinitionAppId PendingBind
    {
      get
      {
        this.Validate();
        return this.state.PendingBind;
      }
    }

    public DefinitionIdentity PendingDeployment
    {
      get
      {
        this.Validate();
        return this.state.PendingDeployment;
      }
    }

    public DefinitionIdentity ExcludedDeployment
    {
      get
      {
        this.Validate();
        return this.state.ExcludedDeployment;
      }
    }

    public Uri DeploymentProviderUri
    {
      get
      {
        this.Validate();
        return this.state.DeploymentProviderUri;
      }
    }

    public Version MinimumRequiredVersion
    {
      get
      {
        this.Validate();
        return this.state.MinimumRequiredVersion;
      }
    }

    public DateTime LastCheckTime
    {
      get
      {
        this.Validate();
        return this.state.LastCheckTime;
      }
    }

    public DefinitionIdentity UpdateSkippedDeployment
    {
      get
      {
        this.Validate();
        return this.state.UpdateSkippedDeployment;
      }
    }

    public DateTime UpdateSkipTime
    {
      get
      {
        this.Validate();
        return this.state.UpdateSkipTime;
      }
    }

    public AppType appType
    {
      get
      {
        this.Validate();
        return this.state.appType;
      }
    }

    public DefinitionIdentity CurrentDeployment
    {
      get
      {
        this.Validate();
        return this.state.CurrentDeployment;
      }
    }

    public DefinitionIdentity RollbackDeployment
    {
      get
      {
        this.Validate();
        return this.state.RollbackDeployment;
      }
    }

    public AssemblyManifest CurrentDeploymentManifest
    {
      get
      {
        this.Validate();
        return this.state.CurrentDeploymentManifest;
      }
    }

    public Uri CurrentDeploymentSourceUri
    {
      get
      {
        this.Validate();
        return this.state.CurrentDeploymentSourceUri;
      }
    }

    public AssemblyManifest CurrentApplicationManifest
    {
      get
      {
        this.Validate();
        return this.state.CurrentApplicationManifest;
      }
    }

    public Uri CurrentApplicationSourceUri
    {
      get
      {
        this.Validate();
        return this.state.CurrentApplicationSourceUri;
      }
    }

    public AssemblyManifest PreviousApplicationManifest
    {
      get
      {
        this.Validate();
        return this.state.PreviousApplicationManifest;
      }
    }

    public DefinitionIdentity PKTGroupId
    {
      get
      {
        DefinitionIdentity definitionIdentity = (DefinitionIdentity) this._subId.Clone();
        definitionIdentity["publicKeyToken"] = (string) null;
        return definitionIdentity;
      }
    }

    public Description EffectiveDescription
    {
      get
      {
        if (this.CurrentApplicationManifest != null && this.CurrentApplicationManifest.UseManifestForTrust)
          return this.CurrentApplicationManifest.Description;
        if (this.CurrentDeploymentManifest == null)
          return (Description) null;
        return this.CurrentDeploymentManifest.Description;
      }
    }

    public string EffectiveCertificatePublicKeyToken
    {
      get
      {
        if (this.CurrentApplicationManifest != null && this.CurrentApplicationManifest.UseManifestForTrust)
          return this.CurrentApplicationManifest.Identity.PublicKeyToken;
        if (this.CurrentDeploymentManifest == null)
          return (string) null;
        return this.CurrentDeploymentManifest.Identity.PublicKeyToken;
      }
    }

    public SubscriptionState(SubscriptionStore subStore, DefinitionIdentity subId)
    {
      this.Initialize(subStore, subId);
    }

    public SubscriptionState(SubscriptionStore subStore, AssemblyManifest deployment)
    {
      this.Initialize(subStore, deployment.Identity.ToSubscriptionId());
    }

    public void Invalidate()
    {
      this._stateIsValid = false;
    }

    private void Validate()
    {
      if (this._stateIsValid)
        return;
      this.state = this._subStore.GetSubscriptionStateInternal(this);
      this._stateIsValid = true;
    }

    private void Initialize(SubscriptionStore subStore, DefinitionIdentity subId)
    {
      this._subStore = subStore;
      this._subId = subId;
      this.Invalidate();
    }

    public override string ToString()
    {
      this.Validate();
      return this.state.ToString();
    }
  }
}
