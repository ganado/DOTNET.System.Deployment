// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.CommitApplicationParams
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Deployment.Application.Manifest;

namespace System.Deployment.Application
{
  internal class CommitApplicationParams
  {
    public DateTime TimeStamp = DateTime.MinValue;
    public DefinitionAppId AppId;
    public bool CommitApp;
    public AssemblyManifest AppManifest;
    public Uri AppSourceUri;
    public string AppManifestPath;
    public string AppPayloadPath;
    public string AppGroup;
    public bool CommitDeploy;
    public AssemblyManifest DeployManifest;
    public Uri DeploySourceUri;
    public string DeployManifestPath;
    public bool IsConfirmed;
    public bool IsUpdate;
    public bool IsRequiredUpdate;
    public bool IsUpdateInPKTGroup;
    public bool IsFullTrustRequested;
    public AppType appType;
    public System.Security.Policy.ApplicationTrust Trust;

    public Description EffectiveDescription
    {
      get
      {
        if (this.AppManifest != null && this.AppManifest.UseManifestForTrust)
          return this.AppManifest.Description;
        if (this.DeployManifest == null)
          return (Description) null;
        return this.DeployManifest.Description;
      }
    }

    public string EffectiveCertificatePublicKeyToken
    {
      get
      {
        if (this.AppManifest != null && this.AppManifest.UseManifestForTrust)
          return this.AppManifest.Identity.PublicKeyToken;
        if (this.DeployManifest == null)
          return (string) null;
        return this.DeployManifest.Identity.PublicKeyToken;
      }
    }

    public CommitApplicationParams()
    {
    }

    public CommitApplicationParams(CommitApplicationParams src)
    {
      this.AppId = src.AppId;
      this.CommitApp = src.CommitApp;
      this.AppManifest = src.AppManifest;
      this.AppSourceUri = src.AppSourceUri;
      this.AppManifestPath = src.AppManifestPath;
      this.AppPayloadPath = src.AppPayloadPath;
      this.AppGroup = src.AppGroup;
      this.CommitDeploy = src.CommitDeploy;
      this.DeployManifest = src.DeployManifest;
      this.DeploySourceUri = src.DeploySourceUri;
      this.DeployManifestPath = src.DeployManifestPath;
      this.TimeStamp = src.TimeStamp;
      this.IsConfirmed = src.IsConfirmed;
      this.IsUpdate = src.IsUpdate;
      this.IsRequiredUpdate = src.IsRequiredUpdate;
      this.IsUpdateInPKTGroup = src.IsUpdateInPKTGroup;
      this.IsFullTrustRequested = src.IsFullTrustRequested;
      this.appType = src.appType;
      this.Trust = src.Trust;
    }
  }
}
