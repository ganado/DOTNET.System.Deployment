// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.SubscriptionStateInternal
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Deployment.Application.Manifest;
using System.Text;

namespace System.Deployment.Application
{
  internal class SubscriptionStateInternal
  {
    public bool IsInstalled;
    public bool IsShellVisible;
    public DefinitionAppId CurrentBind;
    public DefinitionAppId PreviousBind;
    public DefinitionAppId PendingBind;
    public DefinitionIdentity PendingDeployment;
    public DefinitionIdentity ExcludedDeployment;
    public Uri DeploymentProviderUri;
    public Version MinimumRequiredVersion;
    public DateTime LastCheckTime;
    public DateTime UpdateSkipTime;
    public DefinitionIdentity UpdateSkippedDeployment;
    public AppType appType;
    public DefinitionIdentity CurrentDeployment;
    public DefinitionIdentity RollbackDeployment;
    public AssemblyManifest CurrentDeploymentManifest;
    public Uri CurrentDeploymentSourceUri;
    public DefinitionIdentity CurrentApplication;
    public AssemblyManifest CurrentApplicationManifest;
    public Uri CurrentApplicationSourceUri;
    public DefinitionIdentity PreviousApplication;
    public AssemblyManifest PreviousApplicationManifest;

    public SubscriptionStateInternal()
    {
      this.Reset();
    }

    public SubscriptionStateInternal(SubscriptionState subState)
    {
      this.IsInstalled = subState.IsInstalled;
      this.IsShellVisible = subState.IsShellVisible;
      this.CurrentBind = subState.CurrentBind;
      this.PreviousBind = subState.PreviousBind;
      this.PendingBind = subState.PreviousBind;
      this.PendingDeployment = subState.PendingDeployment;
      this.ExcludedDeployment = subState.ExcludedDeployment;
      this.DeploymentProviderUri = subState.DeploymentProviderUri;
      this.MinimumRequiredVersion = subState.MinimumRequiredVersion;
      this.LastCheckTime = subState.LastCheckTime;
      this.UpdateSkippedDeployment = subState.UpdateSkippedDeployment;
      this.UpdateSkipTime = subState.UpdateSkipTime;
      this.appType = subState.appType;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder1 = new StringBuilder();
      stringBuilder1.Append("IsInstalled=" + this.IsInstalled.ToString() + "\r\n");
      stringBuilder1.Append("IsShellVisible=" + this.IsShellVisible.ToString() + "\r\n");
      stringBuilder1.Append("CurrentBind=" + (this.CurrentBind != null ? this.CurrentBind.ToString() : "null") + "\r\n");
      stringBuilder1.Append("PreviousBind=" + (this.PreviousBind != null ? this.PreviousBind.ToString() : "null") + "\r\n");
      stringBuilder1.Append("PendingBind=" + (this.PendingBind != null ? this.PendingBind.ToString() : "null") + "\r\n");
      stringBuilder1.Append("PendingDeployment=" + (this.PendingDeployment != null ? this.PendingDeployment.ToString() : "null") + "\r\n");
      stringBuilder1.Append("ExcludedDeployment=" + (this.ExcludedDeployment != null ? this.ExcludedDeployment.ToString() : "null") + "\r\n");
      stringBuilder1.Append("DeploymentProviderUri=" + (this.DeploymentProviderUri != (Uri) null ? this.DeploymentProviderUri.ToString() : "null") + "\r\n");
      stringBuilder1.Append("MinimumRequiredVersion=" + (this.MinimumRequiredVersion != (Version) null ? this.MinimumRequiredVersion.ToString() : "null") + "\r\n");
      StringBuilder stringBuilder2 = stringBuilder1;
      string str1 = "LastCheckTime=";
      DateTime lastCheckTime = this.LastCheckTime;
      string str2 = this.LastCheckTime.ToString();
      string str3 = "\r\n";
      string str4 = str1 + str2 + str3;
      stringBuilder2.Append(str4);
      StringBuilder stringBuilder3 = stringBuilder1;
      string str5 = "UpdateSkipTime=";
      DateTime updateSkipTime = this.UpdateSkipTime;
      string str6 = this.UpdateSkipTime.ToString();
      string str7 = "\r\n";
      string str8 = str5 + str6 + str7;
      stringBuilder3.Append(str8);
      stringBuilder1.Append("UpdateSkippedDeployment=" + (this.UpdateSkippedDeployment != null ? this.UpdateSkippedDeployment.ToString() : "null") + "\r\n");
      stringBuilder1.Append("appType=" + (object) (ushort) this.appType + "\r\n");
      return stringBuilder1.ToString();
    }

    public void Reset()
    {
      this.IsInstalled = this.IsShellVisible = false;
      this.CurrentBind = this.PreviousBind = this.PendingBind = (DefinitionAppId) null;
      this.ExcludedDeployment = this.PendingDeployment = (DefinitionIdentity) null;
      this.DeploymentProviderUri = (Uri) null;
      this.MinimumRequiredVersion = (Version) null;
      this.LastCheckTime = DateTime.MinValue;
      this.UpdateSkippedDeployment = (DefinitionIdentity) null;
      this.UpdateSkipTime = DateTime.MinValue;
      this.CurrentDeployment = (DefinitionIdentity) null;
      this.RollbackDeployment = (DefinitionIdentity) null;
      this.CurrentDeploymentManifest = (AssemblyManifest) null;
      this.CurrentDeploymentSourceUri = (Uri) null;
      this.CurrentApplication = (DefinitionIdentity) null;
      this.CurrentApplicationManifest = (AssemblyManifest) null;
      this.CurrentApplicationSourceUri = (Uri) null;
      this.PreviousApplication = (DefinitionIdentity) null;
      this.PreviousApplicationManifest = (AssemblyManifest) null;
      this.appType = AppType.None;
    }
  }
}
