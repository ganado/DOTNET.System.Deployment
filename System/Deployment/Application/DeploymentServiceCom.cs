// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.DeploymentServiceCom
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Deployment.Application.Manifest;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Deployment.Application
{
  /// <summary>Provides internal services for the ClickOnce deployment API. </summary>
  [Guid("20FD4E26-8E0F-4F73-A0E0-F27B8C57BE6F")]
  [ClassInterface(ClassInterfaceType.AutoDispatch)]
  [ComVisible(true)]
  [StructLayout(LayoutKind.Sequential)]
  [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
  public class DeploymentServiceCom
  {
    /// <summary>Creates a new instance of <see cref="T:System.Deployment.Application.DeploymentServiceCom" />.</summary>
    public DeploymentServiceCom()
    {
      LifetimeManager.ExtendLifetime();
    }

    /// <summary>Starts the deployment on the client computer. </summary>
    /// <param name="deploymentLocation">The location of the deployment manifest on disk.</param>
    /// <param name="isShortcut">Whether <paramref name="deploymentLocation" /> is a shortcut, or the actual file.</param>
    public void ActivateDeployment(string deploymentLocation, bool isShortcut)
    {
      new ApplicationActivator().ActivateDeployment(deploymentLocation, isShortcut);
    }

    /// <summary>Starts the deployment on the client computer.</summary>
    /// <param name="deploymentLocation">The location of the deployment manifest.</param>
    /// <param name="unsignedPolicy">The policy to use for unsigned applications.</param>
    /// <param name="signedPolicy">The policy to use for signed applications.</param>
    public void ActivateDeploymentEx(string deploymentLocation, int unsignedPolicy, int signedPolicy)
    {
      new ApplicationActivator().ActivateDeploymentEx(deploymentLocation, unsignedPolicy, signedPolicy);
    }

    /// <summary>Activates an application extension.</summary>
    /// <param name="textualSubId">The internal ID of the deployment.</param>
    /// <param name="deploymentProviderUrl">The URL of the deployment.</param>
    /// <param name="targetAssociatedFile">The target file.</param>
    public void ActivateApplicationExtension(string textualSubId, string deploymentProviderUrl, string targetAssociatedFile)
    {
      new ApplicationActivator().ActivateApplicationExtension(textualSubId, deploymentProviderUrl, targetAssociatedFile);
    }

    /// <summary>Maintains the update subscription. </summary>
    /// <param name="textualSubId">The internal ID of the deployment.</param>
    public void MaintainSubscription(string textualSubId)
    {
      LifetimeManager.StartOperation();
      try
      {
        this.MaintainSubscriptionInternal(textualSubId);
      }
      finally
      {
        LifetimeManager.EndOperation();
      }
    }

    /// <summary>Checks the update location to determine whether an updated version of this deployment exists.</summary>
    /// <param name="textualSubId">An internal identifier for the deployment.</param>
    public void CheckForDeploymentUpdate(string textualSubId)
    {
      LifetimeManager.StartOperation();
      try
      {
        this.CheckForDeploymentUpdateInternal(textualSubId);
      }
      finally
      {
        LifetimeManager.EndOperation();
      }
    }

    /// <summary>Stops a deployment update immediately. </summary>
    public void EndServiceRightNow()
    {
      LifetimeManager.EndImmediately();
    }

    /// <summary>Removes all online-only ClickOnce applications that are installed on a computer.</summary>
    public void CleanOnlineAppCache()
    {
      LifetimeManager.StartOperation();
      try
      {
        this.CleanOnlineAppCacheInternal();
      }
      finally
      {
        LifetimeManager.EndOperation();
      }
    }

    private void MaintainSubscriptionInternal(string textualSubId)
    {
      bool flag1 = false;
      string[] strArray = new string[4]
      {
        "Maintain_Exception",
        "Maintain_Completed",
        "Maintain_Failed",
        "Maintain_FailedMsg"
      };
      bool flag2 = false;
      Exception exception = (Exception) null;
      bool flag3 = false;
      string linkUrlMessage = Resources.GetString("ErrorMessage_GenericLinkUrlMessage");
      string linkUrl = (string) null;
      string str = (string) null;
      Logger.StartCurrentThreadLogging();
      Logger.SetTextualSubscriptionIdentity(textualSubId);
      using (UserInterface userInterface = new UserInterface())
      {
        MaintenanceInfo maintenanceInfo = new MaintenanceInfo();
        try
        {
          UserInterfaceInfo info = new UserInterfaceInfo();
          Logger.AddPhaseInformation(Resources.GetString("PhaseLog_StoreQueryForMaintenanceInfo"));
          SubscriptionState subscriptionState = this.GetSubscriptionState(textualSubId);
          bool flag4;
          try
          {
            subscriptionState.SubscriptionStore.CheckInstalledAndShellVisible(subscriptionState);
            if (subscriptionState.RollbackDeployment == null)
            {
              maintenanceInfo.maintenanceFlags |= MaintenanceFlags.RemoveSelected;
            }
            else
            {
              maintenanceInfo.maintenanceFlags |= MaintenanceFlags.RestorationPossible;
              maintenanceInfo.maintenanceFlags |= MaintenanceFlags.RestoreSelected;
            }
            AssemblyManifest deploymentManifest = subscriptionState.CurrentDeploymentManifest;
            if (deploymentManifest != null && deploymentManifest.Description != null)
              str = deploymentManifest.Description.ErrorReportUrl;
            Description effectiveDescription = subscriptionState.EffectiveDescription;
            info.productName = effectiveDescription.Product;
            info.supportUrl = effectiveDescription.SupportUrl;
            info.formTitle = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("UI_MaintenanceTitle"), new object[1]
            {
              (object) info.productName
            });
            flag4 = true;
          }
          catch (DeploymentException ex)
          {
            flag4 = false;
            Logger.AddErrorInformation(Resources.GetString("MaintainLogMsg_FailedStoreLookup"), (Exception) ex);
            maintenanceInfo.maintenanceFlags |= MaintenanceFlags.RemoveSelected;
          }
          catch (FormatException ex)
          {
            flag4 = false;
            Logger.AddErrorInformation(Resources.GetString("MaintainLogMsg_FailedStoreLookup"), (Exception) ex);
            maintenanceInfo.maintenanceFlags |= MaintenanceFlags.RemoveSelected;
          }
          bool flag5 = false;
          if (flag4)
          {
            if (userInterface.ShowMaintenance(info, maintenanceInfo) == UserInterfaceModalResult.Ok)
              flag5 = true;
          }
          else
          {
            maintenanceInfo.maintenanceFlags = MaintenanceFlags.RemoveSelected;
            flag5 = true;
          }
          if (!flag5)
            return;
          flag2 = true;
          if ((maintenanceInfo.maintenanceFlags & MaintenanceFlags.RestoreSelected) != MaintenanceFlags.ClearFlag)
          {
            strArray = new string[4]
            {
              "Rollback_Exception",
              "Rollback_Completed",
              "Rollback_Failed",
              "Rollback_FailedMsg"
            };
            subscriptionState.SubscriptionStore.RollbackSubscription(subscriptionState);
            flag2 = false;
            userInterface.ShowMessage(Resources.GetString("UI_RollbackCompletedMsg"), Resources.GetString("UI_RollbackCompletedTitle"));
          }
          else if ((maintenanceInfo.maintenanceFlags & MaintenanceFlags.RemoveSelected) != MaintenanceFlags.ClearFlag)
          {
            strArray = new string[4]
            {
              "Uninstall_Exception",
              "Uninstall_Completed",
              "Uninstall_Failed",
              "Uninstall_FailedMsg"
            };
            try
            {
              subscriptionState.SubscriptionStore.UninstallSubscription(subscriptionState);
              flag2 = false;
            }
            catch (DeploymentException ex)
            {
              Logger.AddErrorInformation(Resources.GetString("MaintainLogMsg_UninstallFailed"), (Exception) ex);
              flag3 = true;
              ShellExposure.RemoveSubscriptionShellExposure(subscriptionState);
              flag3 = false;
            }
          }
          flag1 = true;
        }
        catch (DeploymentException ex)
        {
          Logger.AddErrorInformation((Exception) ex, Resources.GetString(strArray[0]), new object[1]
          {
            (object) textualSubId
          });
          exception = (Exception) ex;
        }
        finally
        {
          Logger.AddPhaseInformation(Resources.GetString(flag1 ? strArray[1] : strArray[2]), new object[1]
          {
            (object) textualSubId
          });
          if ((uint) (maintenanceInfo.maintenanceFlags & MaintenanceFlags.RestoreSelected) > 0U & flag2 || (maintenanceInfo.maintenanceFlags & MaintenanceFlags.RemoveSelected) != MaintenanceFlags.ClearFlag && flag3 & flag2)
          {
            string logFileLocation = Logger.GetLogFilePath();
            if (!Logger.FlushCurrentThreadLogs())
              logFileLocation = (string) null;
            if (str != null && exception != null)
            {
              Exception innerMostException = this.GetInnerMostException(exception);
              linkUrl = string.Format("{0}?outer={1}&&inner={2}&&msg={3}", new object[4]
              {
                (object) str,
                (object) exception.GetType().ToString(),
                (object) innerMostException.GetType().ToString(),
                (object) innerMostException.Message
              });
              if (linkUrl.Length > 2048)
                linkUrl = linkUrl.Substring(0, 2048);
            }
            userInterface.ShowError(Resources.GetString("UI_MaintenceErrorTitle"), Resources.GetString(strArray[3]), logFileLocation, linkUrl, linkUrlMessage);
          }
          Logger.EndCurrentThreadLogging();
        }
      }
    }

    private void CheckForDeploymentUpdateInternal(string textualSubId)
    {
      bool flag = false;
      Logger.StartCurrentThreadLogging();
      Logger.SetTextualSubscriptionIdentity(textualSubId);
      try
      {
        SubscriptionState subscriptionState = this.GetShellVisibleSubscriptionState(textualSubId);
        subscriptionState.SubscriptionStore.CheckForDeploymentUpdate(subscriptionState);
        flag = true;
      }
      catch (DeploymentException ex)
      {
        Logger.AddErrorInformation(Resources.GetString("Upd_Exception"), (Exception) ex);
      }
      finally
      {
        Logger.AddPhaseInformation(Resources.GetString(flag ? "Upd_Completed" : "Upd_Failed"));
        Logger.EndCurrentThreadLogging();
      }
    }

    private void CleanOnlineAppCacheInternal()
    {
      bool flag = false;
      Logger.StartCurrentThreadLogging();
      try
      {
        SubscriptionStore.CurrentUser.CleanOnlineAppCache();
        flag = true;
      }
      catch (Exception ex)
      {
        Logger.AddErrorInformation(Resources.GetString("Ex_CleanOnlineAppCache"), ex);
        throw;
      }
      finally
      {
        Logger.AddPhaseInformation(Resources.GetString(flag ? "CleanOnlineCache_Completed" : "CleanOnlineCache_Failed"));
        Logger.EndCurrentThreadLogging();
      }
    }

    private SubscriptionState GetShellVisibleSubscriptionState(string textualSubId)
    {
      SubscriptionState subscriptionState = this.GetSubscriptionState(textualSubId);
      subscriptionState.SubscriptionStore.CheckInstalledAndShellVisible(subscriptionState);
      return subscriptionState;
    }

    private SubscriptionState GetSubscriptionState(string textualSubId)
    {
      if (textualSubId == null)
        throw new ArgumentNullException("textualSubId", Resources.GetString("Ex_ComArgSubIdentityNull"));
      DefinitionIdentity subId;
      try
      {
        subId = new DefinitionIdentity(textualSubId);
      }
      catch (COMException ex)
      {
        throw new DeploymentException(ExceptionTypes.SubscriptionState, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.GetString("Ex_ComArgSubIdentityNotValid"), new object[1]
        {
          (object) textualSubId
        }), (Exception) ex);
      }
      catch (SEHException ex)
      {
        throw new DeploymentException(ExceptionTypes.SubscriptionState, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.GetString("Ex_ComArgSubIdentityNotValid"), new object[1]
        {
          (object) textualSubId
        }), (Exception) ex);
      }
      if (subId.Version != (Version) null)
        throw new DeploymentException(ExceptionTypes.SubscriptionState, Resources.GetString("Ex_ComArgSubIdentityWithVersion"));
      SubscriptionStore currentUser = SubscriptionStore.CurrentUser;
      currentUser.RefreshStorePointer();
      return currentUser.GetSubscriptionState(subId);
    }

    private Exception GetInnerMostException(Exception exception)
    {
      if (exception.InnerException != null)
        return this.GetInnerMostException(exception.InnerException);
      return exception;
    }
  }
}
