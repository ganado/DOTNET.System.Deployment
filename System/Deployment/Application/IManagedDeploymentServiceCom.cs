// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.IManagedDeploymentServiceCom
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;

namespace System.Deployment.Application
{
  [Guid("B3CA4E79-0107-4CA7-9708-3BE0A97957FB")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  internal interface IManagedDeploymentServiceCom
  {
    void ActivateDeployment(string deploymentLocation, bool isShortcut);

    void ActivateDeploymentEx(string deploymentLocation, int unsignedPolicy, int signedPolicy);

    void ActivateApplicationExtension(string textualSubId, string deploymentProviderUrl, string targetAssociatedFile);

    void MaintainSubscription(string textualSubId);

    void CheckForDeploymentUpdate(string textualSubId);

    void EndServiceRightNow();

    void CleanOnlineAppCache();
  }
}
