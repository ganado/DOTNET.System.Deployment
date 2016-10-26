// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.DeploymentServiceComWrapper
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

namespace System.Deployment.Application
{
  internal class DeploymentServiceComWrapper : IManagedDeploymentServiceCom
  {
    private DeploymentServiceCom m_deploymentServiceCom;

    public DeploymentServiceComWrapper()
    {
      this.m_deploymentServiceCom = new DeploymentServiceCom();
    }

    public void ActivateApplicationExtension(string textualSubId, string deploymentProviderUrl, string targetAssociatedFile)
    {
      this.m_deploymentServiceCom.ActivateApplicationExtension(textualSubId, deploymentProviderUrl, targetAssociatedFile);
    }

    public void ActivateDeployment(string deploymentLocation, bool isShortcut)
    {
      this.m_deploymentServiceCom.ActivateDeployment(deploymentLocation, isShortcut);
    }

    public void ActivateDeploymentEx(string deploymentLocation, int unsignedPolicy, int signedPolicy)
    {
      this.m_deploymentServiceCom.ActivateDeploymentEx(deploymentLocation, unsignedPolicy, signedPolicy);
    }

    public void CheckForDeploymentUpdate(string textualSubId)
    {
      this.m_deploymentServiceCom.CheckForDeploymentUpdate(textualSubId);
    }

    public void CleanOnlineAppCache()
    {
      this.m_deploymentServiceCom.CleanOnlineAppCache();
    }

    public void EndServiceRightNow()
    {
      this.m_deploymentServiceCom.EndServiceRightNow();
    }

    public void MaintainSubscription(string textualSubId)
    {
      this.m_deploymentServiceCom.MaintainSubscription(textualSubId);
    }
  }
}
