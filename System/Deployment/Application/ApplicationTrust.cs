// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.ApplicationTrust
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Security.Policy;
using System.Threading;

namespace System.Deployment.Application
{
  internal static class ApplicationTrust
  {
    public static System.Security.Policy.ApplicationTrust RequestTrust(SubscriptionState subState, bool isShellVisible, bool isUpdate, ActivationContext actCtx)
    {
      return ApplicationTrust.RequestTrust(subState, isShellVisible, isUpdate, actCtx, new TrustManagerContext()
      {
        IgnorePersistedDecision = false,
        NoPrompt = false,
        Persist = true
      });
    }

    public static System.Security.Policy.ApplicationTrust RequestTrust(SubscriptionState subState, bool isShellVisible, bool isUpdate, ActivationContext actCtx, TrustManagerContext tmc)
    {
      Logger.AddMethodCall("ApplicationTrust.RequestTrust(isShellVisible=" + isShellVisible.ToString() + ", isUpdate=" + isUpdate.ToString() + ", subState.IsInstalled=" + subState.IsInstalled.ToString() + ") called.");
      if (!subState.IsInstalled || subState.IsShellVisible != isShellVisible)
        tmc.IgnorePersistedDecision = true;
      if (isUpdate)
        tmc.PreviousApplicationIdentity = subState.CurrentBind.ToApplicationIdentity();
      bool applicationTrust1;
      try
      {
        Logger.AddInternalState("Calling ApplicationSecurityManager.DetermineApplicationTrust().");
        Logger.AddInternalState("Trust Manager Context=" + Logger.Serialize(tmc));
        applicationTrust1 = ApplicationSecurityManager.DetermineApplicationTrust(actCtx, tmc);
      }
      catch (TypeLoadException ex)
      {
        throw new InvalidDeploymentException(Resources.GetString("Ex_InvalidTrustInfo"), (Exception) ex);
      }
      if (!applicationTrust1)
        throw new TrustNotGrantedException(Resources.GetString("Ex_NoTrust"));
      Logger.AddInternalState("Trust granted.");
      System.Security.Policy.ApplicationTrust applicationTrust2 = (System.Security.Policy.ApplicationTrust) null;
      for (int index = 0; index < 5; ++index)
      {
        applicationTrust2 = ApplicationSecurityManager.UserApplicationTrusts[actCtx.Identity.FullName];
        if (applicationTrust2 == null)
          Thread.Sleep(10);
        else
          break;
      }
      if (applicationTrust2 == null)
        throw new InvalidDeploymentException(Resources.GetString("Ex_InvalidMatchTrust"));
      return applicationTrust2;
    }

    public static void RemoveCachedTrust(DefinitionAppId appId)
    {
      ApplicationSecurityManager.UserApplicationTrusts.Remove(appId.ToApplicationIdentity(), ApplicationVersionMatch.MatchExactVersion);
    }

    public static System.Security.Policy.ApplicationTrust PersistTrustWithoutEvaluation(ActivationContext actCtx)
    {
      ApplicationSecurityInfo applicationSecurityInfo = new ApplicationSecurityInfo(actCtx);
      System.Security.Policy.ApplicationTrust trust = new System.Security.Policy.ApplicationTrust(actCtx.Identity);
      trust.IsApplicationTrustedToRun = true;
      trust.DefaultGrantSet = new PolicyStatement(applicationSecurityInfo.DefaultRequestSet, PolicyStatementAttribute.Nothing);
      trust.Persist = true;
      trust.ApplicationIdentity = actCtx.Identity;
      ApplicationSecurityManager.UserApplicationTrusts.Add(trust);
      return trust;
    }
  }
}
