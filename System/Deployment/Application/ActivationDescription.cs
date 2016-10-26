// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.ActivationDescription
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Deployment.Application.Manifest;

namespace System.Deployment.Application
{
  internal class ActivationDescription : CommitApplicationParams
  {
    private ActivationType activationType;

    public ActivationType ActType
    {
      get
      {
        return this.activationType;
      }
      set
      {
        this.activationType = value;
      }
    }

    public void SetApplicationManifest(AssemblyManifest manifest, Uri manifestUri, string manifestPath)
    {
      this.AppManifest = manifest;
      this.AppSourceUri = manifestUri;
      this.AppManifestPath = manifestPath;
      if (this.AppManifest.EntryPoints[0].CustomHostSpecified)
        this.appType = AppType.CustomHostSpecified;
      if (!this.AppManifest.EntryPoints[0].CustomUX)
        return;
      this.appType = AppType.CustomUX;
    }

    public void SetDeploymentManifest(AssemblyManifest manifest, Uri manifestUri, string manifestPath)
    {
      this.DeploySourceUri = manifestUri;
      this.DeployManifest = manifest;
      this.DeployManifestPath = manifestPath;
    }

    public string ToAppCodebase()
    {
      return (this.DeploySourceUri.Query == null || this.DeploySourceUri.Query.Length <= 0 ? this.DeploySourceUri : new Uri(this.DeploySourceUri.GetLeftPart(UriPartial.Path))).AbsoluteUri;
    }

    public ActivationContext ToActivationContext()
    {
      return ActivationContext.CreatePartialActivationContext(this.AppId.ToApplicationIdentity(), new string[2]
      {
        this.DeployManifestPath,
        this.AppManifestPath
      });
    }
  }
}
