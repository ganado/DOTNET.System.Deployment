// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.SubscriptionStore
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Collections;
using System.Deployment.Application.Manifest;
using System.Deployment.Application.Win32InterOp;
using System.Deployment.Internal.Isolation;
using System.Deployment.Internal.Isolation.Manifest;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace System.Deployment.Application
{
  internal class SubscriptionStore
  {
    private static object _currentUserLock = new object();
    private static SubscriptionStore _userStore;
    private string _deployPath;
    private string _tempPath;
    private ComponentStore _compStore;
    private object _subscriptionStoreLock;

    public static SubscriptionStore CurrentUser
    {
      get
      {
        if (SubscriptionStore._userStore == null)
        {
          lock (SubscriptionStore._currentUserLock)
          {
            if (SubscriptionStore._userStore == null)
              SubscriptionStore._userStore = new SubscriptionStore(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Deployment"), Path.Combine(Path.GetTempPath(), "Deployment"), ComponentStoreType.UserStore);
          }
        }
        return SubscriptionStore._userStore;
      }
    }

    private DefinitionIdentity SubscriptionStoreLock
    {
      get
      {
        if (this._subscriptionStoreLock == null)
          Interlocked.CompareExchange(ref this._subscriptionStoreLock, (object) new DefinitionIdentity("__SubscriptionStoreLock__"), (object) null);
        return (DefinitionIdentity) this._subscriptionStoreLock;
      }
    }

    private SubscriptionStore(string deployPath, string tempPath, ComponentStoreType storeType)
    {
      this._deployPath = deployPath;
      this._tempPath = tempPath;
      Directory.CreateDirectory(this._deployPath);
      Directory.CreateDirectory(this._tempPath);
      using (this.AcquireStoreWriterLock())
        this._compStore = ComponentStore.GetStore(storeType, this);
    }

    public void RefreshStorePointer()
    {
      using (this.AcquireStoreWriterLock())
        this._compStore.RefreshStorePointer();
    }

    public void CleanOnlineAppCache()
    {
      using (this.AcquireStoreWriterLock())
      {
        this._compStore.RefreshStorePointer();
        this._compStore.CleanOnlineAppCache();
      }
    }

    public void CommitApplication(ref SubscriptionState subState, CommitApplicationParams commitParams)
    {
      Logger.AddMethodCall("CommitApplication called.");
      using (this.AcquireSubscriptionWriterLock(subState))
      {
        if (commitParams.CommitDeploy)
        {
          Logger.AddInternalState("Commiting Deployment :  subscription metadata.");
          UriHelper.ValidateSupportedScheme(commitParams.DeploySourceUri);
          this.CheckDeploymentSubscriptionState(subState, commitParams.DeployManifest);
          this.ValidateFileAssoctiation(subState, commitParams);
          if (commitParams.IsUpdate && !commitParams.IsUpdateInPKTGroup)
            SubscriptionStore.CheckInstalled(subState);
        }
        if (commitParams.CommitApp)
        {
          Logger.AddInternalState("Commiting Application:  application binaries.");
          UriHelper.ValidateSupportedScheme(commitParams.AppSourceUri);
          if (commitParams.AppGroup != null)
            SubscriptionStore.CheckInstalled(subState);
          this.CheckApplicationPayload(commitParams);
        }
        bool flag = false;
        bool identityGroupFound = false;
        bool locationGroupFound = false;
        string identityGroupProductName = "";
        ArrayList arrayList = this._compStore.CollectCrossGroupApplications(commitParams.DeploySourceUri, commitParams.DeployManifest.Identity, ref identityGroupFound, ref locationGroupFound, ref identityGroupProductName);
        if (arrayList.Count > 0)
        {
          flag = true;
          Logger.AddInternalState("This installation is a Cross Group: identityGroupFound=" + identityGroupFound.ToString() + ",locationGroupFound=" + locationGroupFound.ToString());
        }
        if (subState.IsShellVisible & identityGroupFound & locationGroupFound)
          throw new DeploymentException(ExceptionTypes.GroupMultipleMatch, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_GroupMultipleMatch"), new object[1]
          {
            (object) identityGroupProductName
          }));
        subState = this.GetSubscriptionState(commitParams.DeployManifest);
        this._compStore.CommitApplication(subState, commitParams);
        if (flag)
        {
          Logger.AddInternalState("Performing cross group migration.");
          IActContext actContext = IsolationInterop.CreateActContext(subState.CurrentBind.ComPointer);
          actContext.PrepareForExecution(IntPtr.Zero, IntPtr.Zero);
          uint ulDisposition;
          actContext.SetApplicationRunningState(0U, 1U, out ulDisposition);
          actContext.SetApplicationRunningState(0U, 2U, out ulDisposition);
          Logger.AddInternalState("Uninstalling all cross groups.");
          foreach (ComponentStore.CrossGroupApplicationData groupApplicationData in arrayList)
          {
            if (groupApplicationData.CrossGroupType == ComponentStore.CrossGroupApplicationData.GroupType.LocationGroup)
            {
              if (groupApplicationData.SubState.appType == AppType.CustomHostSpecified)
              {
                Logger.AddInternalState("UninstallCustomHostSpecifiedSubscription : " + (groupApplicationData.SubState.SubscriptionId != null ? groupApplicationData.SubState.SubscriptionId.ToString() : "null"));
                this.UninstallCustomHostSpecifiedSubscription(groupApplicationData.SubState);
              }
              else if (groupApplicationData.SubState.appType == AppType.CustomUX)
              {
                Logger.AddInternalState("UninstallCustomUXSubscription : " + (groupApplicationData.SubState.SubscriptionId != null ? groupApplicationData.SubState.SubscriptionId.ToString() : "null"));
                this.UninstallCustomUXSubscription(groupApplicationData.SubState);
              }
              else if (groupApplicationData.SubState.IsShellVisible)
              {
                Logger.AddInternalState("UninstallSubscription : " + (groupApplicationData.SubState.SubscriptionId != null ? groupApplicationData.SubState.SubscriptionId.ToString() : "null"));
                this.UninstallSubscription(groupApplicationData.SubState);
              }
            }
            else if (groupApplicationData.CrossGroupType == ComponentStore.CrossGroupApplicationData.GroupType.IdentityGroup)
              Logger.AddInternalState("Not uninstalled :" + (groupApplicationData.SubState.SubscriptionId != null ? groupApplicationData.SubState.SubscriptionId.ToString() : "null") + ". It is in the identity group.");
          }
        }
        if (commitParams.IsConfirmed && subState.IsInstalled && (subState.IsShellVisible && commitParams.appType != AppType.CustomUX))
          this.UpdateSubscriptionExposure(subState);
        if (commitParams.appType == AppType.CustomUX)
        {
          ShellExposure.ShellExposureInformation exposureInformation = ShellExposure.ShellExposureInformation.CreateShellExposureInformation(subState.SubscriptionId);
          ShellExposure.UpdateShellExtensions(subState, ref exposureInformation);
        }
        SubscriptionStore.OnDeploymentAdded(subState);
      }
    }

    public void RollbackSubscription(SubscriptionState subState)
    {
      using (this.AcquireSubscriptionWriterLock(subState))
      {
        this.CheckInstalledAndShellVisible(subState);
        if (subState.RollbackDeployment == null)
          throw new DeploymentException(ExceptionTypes.SubscriptionState, Resources.GetString("Ex_SubNoRollbackDeployment"));
        if (subState.CurrentApplicationManifest != null)
        {
          string productName = (string) null;
          if (subState.CurrentDeploymentManifest != null && subState.CurrentDeploymentManifest.Description != null)
            productName = subState.CurrentDeploymentManifest.Description.Product;
          if (productName == null)
            productName = subState.SubscriptionId.Name;
          ShellExposure.RemoveShellExtensions(subState.SubscriptionId, subState.CurrentApplicationManifest, productName);
        }
        this._compStore.RollbackSubscription(subState);
        this.UpdateSubscriptionExposure(subState);
        SubscriptionStore.OnDeploymentRemoved(subState);
      }
    }

    public void UninstallSubscription(SubscriptionState subState)
    {
      using (this.AcquireSubscriptionWriterLock(subState))
      {
        this.CheckInstalledAndShellVisible(subState);
        if (subState.CurrentApplicationManifest != null)
        {
          string productName = (string) null;
          if (subState.CurrentDeploymentManifest != null && subState.CurrentDeploymentManifest.Description != null)
            productName = subState.CurrentDeploymentManifest.Description.Product;
          if (productName == null)
            productName = subState.SubscriptionId.Name;
          ShellExposure.RemoveShellExtensions(subState.SubscriptionId, subState.CurrentApplicationManifest, productName);
          ShellExposure.RemovePins(subState);
        }
        this._compStore.RemoveSubscription(subState);
        SubscriptionStore.RemoveSubscriptionExposure(subState);
        SubscriptionStore.OnDeploymentRemoved(subState);
      }
    }

    public void UninstallCustomUXSubscription(SubscriptionState subState)
    {
      using (this.AcquireSubscriptionWriterLock(subState))
      {
        SubscriptionStore.CheckInstalled(subState);
        if (subState.appType != AppType.CustomUX)
          throw new InvalidOperationException(Resources.GetString("Ex_CannotCallUninstallCustomUXApplication"));
        if (subState.CurrentApplicationManifest != null)
        {
          string productName = (string) null;
          if (subState.CurrentDeploymentManifest != null && subState.CurrentDeploymentManifest.Description != null)
            productName = subState.CurrentDeploymentManifest.Description.Product;
          if (productName == null)
            productName = subState.SubscriptionId.Name;
          ShellExposure.RemoveShellExtensions(subState.SubscriptionId, subState.CurrentApplicationManifest, productName);
        }
        this._compStore.RemoveSubscription(subState);
        SubscriptionStore.OnDeploymentRemoved(subState);
      }
    }

    public void UninstallCustomHostSpecifiedSubscription(SubscriptionState subState)
    {
      using (this.AcquireSubscriptionWriterLock(subState))
      {
        SubscriptionStore.CheckInstalled(subState);
        if (subState.appType != AppType.CustomHostSpecified)
          throw new InvalidOperationException(Resources.GetString("Ex_CannotCallUninstallCustomAddIn"));
        this._compStore.RemoveSubscription(subState);
        SubscriptionStore.OnDeploymentRemoved(subState);
      }
    }

    public void SetPendingDeployment(SubscriptionState subState, DefinitionIdentity deployId, DateTime checkTime)
    {
      using (this.AcquireSubscriptionWriterLock(subState))
      {
        this.CheckInstalledAndShellVisible(subState);
        this._compStore.SetPendingDeployment(subState, deployId, checkTime);
      }
    }

    public void SetLastCheckTimeToNow(SubscriptionState subState)
    {
      using (this.AcquireSubscriptionWriterLock(subState))
      {
        SubscriptionStore.CheckInstalled(subState);
        this._compStore.SetPendingDeployment(subState, (DefinitionIdentity) null, DateTime.UtcNow);
      }
    }

    public void SetUpdateSkipTime(SubscriptionState subState, DefinitionIdentity updateSkippedDeployment, DateTime updateSkipTime)
    {
      using (this.AcquireSubscriptionWriterLock(subState))
      {
        this.CheckInstalledAndShellVisible(subState);
        this._compStore.SetUpdateSkipTime(subState, updateSkippedDeployment, updateSkipTime);
      }
    }

    public bool CheckAndReferenceApplication(SubscriptionState subState, DefinitionAppId appId, long transactionId)
    {
      DefinitionIdentity deploymentIdentity = appId.DeploymentIdentity;
      DefinitionIdentity applicationIdentity = appId.ApplicationIdentity;
      if (!subState.IsInstalled || !this.IsAssemblyInstalled(deploymentIdentity))
        return false;
      if (!this.IsAssemblyInstalled(applicationIdentity))
        throw new DeploymentException(ExceptionTypes.Subscription, Resources.GetString("Ex_IllegalApplicationId"));
      if (!appId.Equals((object) subState.CurrentBind))
        return appId.Equals((object) subState.PreviousBind);
      return true;
    }

    public void ActivateApplication(DefinitionAppId appId, string activationParameter, bool useActivationParameter)
    {
      using (this.AcquireStoreReaderLock())
        this._compStore.ActivateApplication(appId, activationParameter, useActivationParameter);
    }

    public FileStream AcquireReferenceTransaction(out long transactionId)
    {
      transactionId = 0L;
      return (FileStream) null;
    }

    public SubscriptionState GetSubscriptionState(DefinitionIdentity subId)
    {
      return new SubscriptionState(this, subId);
    }

    public SubscriptionState GetSubscriptionState(AssemblyManifest deployment)
    {
      return new SubscriptionState(this, deployment);
    }

    public SubscriptionStateInternal GetSubscriptionStateInternal(SubscriptionState subState)
    {
      using (this.AcquireSubscriptionReaderLock(subState))
        return this._compStore.GetSubscriptionStateInternal(subState);
    }

    public void CheckForDeploymentUpdate(SubscriptionState subState)
    {
      this.CheckInstalledAndShellVisible(subState);
      Uri deploymentProviderUri = subState.DeploymentProviderUri;
      TempFile tempFile = (TempFile) null;
      try
      {
        AssemblyManifest deployment = DownloadManager.DownloadDeploymentManifest(subState.SubscriptionStore, ref deploymentProviderUri, out tempFile);
        Version version = this.CheckUpdateInManifest(subState, deploymentProviderUri, deployment, subState.CurrentDeployment.Version);
        DefinitionIdentity deployId = version != (Version) null ? deployment.Identity : (DefinitionIdentity) null;
        this.SetPendingDeployment(subState, deployId, DateTime.UtcNow);
        if (!(version != (Version) null) || !deployment.Identity.Equals((object) subState.PendingDeployment))
          return;
        Logger.AddPhaseInformation(Resources.GetString("Upd_FoundUpdate"), (object) subState.SubscriptionId.ToString(), (object) deployment.Identity.Version.ToString(), (object) deploymentProviderUri.AbsoluteUri);
      }
      finally
      {
        if (tempFile != null)
          tempFile.Dispose();
      }
    }

    public Version CheckUpdateInManifest(SubscriptionState subState, Uri updateCodebaseUri, AssemblyManifest deployment, Version currentVersion)
    {
      bool bUpdateInPKTGroup = false;
      return this.CheckUpdateInManifest(subState, updateCodebaseUri, deployment, currentVersion, ref bUpdateInPKTGroup);
    }

    public Version CheckUpdateInManifest(SubscriptionState subState, Uri updateCodebaseUri, AssemblyManifest deployment, Version currentVersion, ref bool bUpdateInPKTGroup)
    {
      SubscriptionStore.CheckOnlineShellVisibleConflict(subState, deployment);
      SubscriptionStore.CheckInstalledAndUpdateableConflict(subState, deployment);
      SubscriptionStore.CheckMinimumRequiredVersion(subState, deployment);
      SubscriptionState subscriptionState = this.GetSubscriptionState(deployment);
      if (!subscriptionState.SubscriptionId.Equals((object) subState.SubscriptionId))
      {
        Logger.AddInternalState("Cross family update detected. Check if only PKT has changed between versions.");
        Logger.AddInternalState("updateCodebaseUri=" + (object) updateCodebaseUri + ", subState.DeploymentProviderUri=" + (object) subState.DeploymentProviderUri);
        Logger.AddInternalState("subState=" + (object) subState.SubscriptionId + ", manSubState.SubscriptionId=" + (object) subscriptionState.SubscriptionId);
        if (!updateCodebaseUri.Equals((object) subState.DeploymentProviderUri) || !subState.PKTGroupId.Equals((object) subscriptionState.PKTGroupId))
          throw new DeploymentException(ExceptionTypes.SubscriptionState, Resources.GetString("Ex_DeploymentIdentityNotInSubscription"));
        Logger.AddInternalState("PKT has changed.");
        bUpdateInPKTGroup = true;
      }
      Version version = deployment.Identity.Version;
      if (version.CompareTo(currentVersion) == 0)
        return (Version) null;
      return version;
    }

    public void CheckDeploymentSubscriptionState(SubscriptionState subState, AssemblyManifest deployment)
    {
      if (!subState.IsInstalled)
        return;
      SubscriptionStore.CheckOnlineShellVisibleConflict(subState, deployment);
      SubscriptionStore.CheckInstalledAndUpdateableConflict(subState, deployment);
      SubscriptionStore.CheckMinimumRequiredVersion(subState, deployment);
    }

    public void CheckCustomUXFlag(SubscriptionState subState, AssemblyManifest application)
    {
      if (!subState.IsInstalled)
        return;
      if (application.EntryPoints[0].CustomUX && subState.appType != AppType.CustomUX)
        throw new DeploymentException(Resources.GetString("Ex_CustomUXAlready"));
      if (!application.EntryPoints[0].CustomUX && subState.appType == AppType.CustomUX)
        throw new DeploymentException(Resources.GetString("Ex_NotCustomUXAlready"));
    }

    public void ValidateFileAssoctiation(SubscriptionState subState, CommitApplicationParams commitParams)
    {
      if (commitParams.DeployManifest != null && commitParams.AppManifest != null && (!commitParams.DeployManifest.Deployment.Install && commitParams.AppManifest.FileAssociations.Length != 0))
        throw new DeploymentException(ExceptionTypes.SubscriptionState, Resources.GetString("Ex_OnlineAppWithFileAssociation"));
    }

    public void CheckInstalledAndShellVisible(SubscriptionState subState)
    {
      SubscriptionStore.CheckInstalled(subState);
      SubscriptionStore.CheckShellVisible(subState);
    }

    public static void CheckInstalled(SubscriptionState subState)
    {
      if (!subState.IsInstalled)
        throw new DeploymentException(ExceptionTypes.SubscriptionState, Resources.GetString("Ex_SubNotInstalled"));
    }

    public static void CheckShellVisible(SubscriptionState subState)
    {
      if (!subState.IsShellVisible)
        throw new DeploymentException(ExceptionTypes.SubscriptionState, Resources.GetString("Ex_SubNotShellVisible"));
    }

    public bool CheckGroupInstalled(SubscriptionState subState, DefinitionAppId appId, string groupName)
    {
      using (this.AcquireSubscriptionReaderLock(subState))
        return this._compStore.CheckGroupInstalled(appId, groupName);
    }

    public bool CheckGroupInstalled(SubscriptionState subState, DefinitionAppId appId, AssemblyManifest appManifest, string groupName)
    {
      using (this.AcquireSubscriptionReaderLock(subState))
        return this._compStore.CheckGroupInstalled(appId, appManifest, groupName);
    }

    public IDisposable AcquireSubscriptionReaderLock(SubscriptionState subState)
    {
      subState.Invalidate();
      return this.AcquireStoreReaderLock();
    }

    public IDisposable AcquireSubscriptionWriterLock(SubscriptionState subState)
    {
      subState.Invalidate();
      return this.AcquireStoreWriterLock();
    }

    public IDisposable AcquireStoreReaderLock()
    {
      return this.AcquireLock(this.SubscriptionStoreLock, false);
    }

    public IDisposable AcquireStoreWriterLock()
    {
      return this.AcquireLock(this.SubscriptionStoreLock, true);
    }

    public TempDirectory AcquireTempDirectory()
    {
      return new TempDirectory(this._tempPath);
    }

    public TempFile AcquireTempFile(string suffix)
    {
      return new TempFile(this._tempPath, suffix);
    }

    internal ulong GetPrivateSize(DefinitionAppId appId)
    {
      ArrayList deployAppIds = new ArrayList();
      deployAppIds.Add((object) appId);
      using (this.AcquireStoreReaderLock())
        return this._compStore.GetPrivateSize(deployAppIds);
    }

    internal ulong GetSharedSize(DefinitionAppId appId)
    {
      ArrayList deployAppIds = new ArrayList();
      deployAppIds.Add((object) appId);
      using (this.AcquireStoreReaderLock())
        return this._compStore.GetSharedSize(deployAppIds);
    }

    internal ulong GetOnlineAppQuotaInBytes()
    {
      return this._compStore.GetOnlineAppQuotaInBytes();
    }

    internal ulong GetSizeLimitInBytesForSemiTrustApps()
    {
      return this.GetOnlineAppQuotaInBytes() / 2UL;
    }

    internal Store.IPathLock LockApplicationPath(DefinitionAppId definitionAppId)
    {
      using (this.AcquireStoreReaderLock())
        return this._compStore.LockApplicationPath(definitionAppId);
    }

    private static void CheckOnlineShellVisibleConflict(SubscriptionState subState, AssemblyManifest deployment)
    {
      if (!deployment.Deployment.Install && subState.IsShellVisible)
        throw new DeploymentException(ExceptionTypes.SubscriptionState, Resources.GetString("Ex_OnlineAlreadyShellVisible"));
    }

    private static void CheckInstalledAndUpdateableConflict(SubscriptionState subState, AssemblyManifest deployment)
    {
    }

    private static void CheckMinimumRequiredVersion(SubscriptionState subState, AssemblyManifest deployment)
    {
      if (!(subState.MinimumRequiredVersion != (Version) null))
        return;
      if (deployment.Identity.Version < subState.MinimumRequiredVersion)
        throw new DeploymentException(ExceptionTypes.SubscriptionState, Resources.GetString("Ex_DeploymentBelowMinimumRequiredVersion"));
      if (deployment.Deployment.MinimumRequiredVersion != (Version) null && deployment.Deployment.MinimumRequiredVersion < subState.MinimumRequiredVersion)
        throw new DeploymentException(ExceptionTypes.SubscriptionState, Resources.GetString("Ex_DecreasingMinimumRequiredVersion"));
    }

    private void CheckApplicationPayload(CommitApplicationParams commitParams)
    {
      if (commitParams.AppGroup == null && commitParams.appType != AppType.CustomHostSpecified)
      {
        string path = Path.Combine(commitParams.AppPayloadPath, commitParams.AppManifest.EntryPoints[0].CommandFile);
        if (!PlatformDetector.IsWin8orLater())
          SystemUtils.CheckSupportedImageAndCLRVersions(path);
      }
      string str = (string) null;
      Store.IPathLock pathLock = (Store.IPathLock) null;
      try
      {
        pathLock = this._compStore.LockAssemblyPath(commitParams.AppManifest.Identity);
        str = Path.GetDirectoryName(pathLock.Path);
        str = Path.Combine(str, "manifests");
        str = Path.Combine(str, Path.GetFileName(pathLock.Path) + ".manifest");
      }
      catch (DeploymentException ex)
      {
      }
      catch (COMException ex)
      {
      }
      finally
      {
        if (pathLock != null)
          pathLock.Dispose();
      }
      if (string.IsNullOrEmpty(str) || !System.IO.File.Exists(str) || (string.IsNullOrEmpty(commitParams.AppManifestPath) || !System.IO.File.Exists(commitParams.AppManifestPath)))
        return;
      byte[] digestValue1 = ComponentVerifier.GenerateDigestValue(str, CMS_HASH_DIGESTMETHOD.CMS_HASH_DIGESTMETHOD_SHA1, CMS_HASH_TRANSFORM.CMS_HASH_TRANSFORM_IDENTITY);
      byte[] digestValue2 = ComponentVerifier.GenerateDigestValue(commitParams.AppManifestPath, CMS_HASH_DIGESTMETHOD.CMS_HASH_DIGESTMETHOD_SHA1, CMS_HASH_TRANSFORM.CMS_HASH_TRANSFORM_IDENTITY);
      bool flag = false;
      Logger.AddInternalState("In-place update check. Existing manifest path = " + str + ", Existing manifest hash=" + Encoding.UTF8.GetString(digestValue1) + ", New manifest path=" + commitParams.AppManifestPath + ", New manifest hash=" + Encoding.UTF8.GetString(digestValue2));
      if (digestValue1.Length == digestValue2.Length)
      {
        int index = 0;
        while (index < digestValue1.Length && (int) digestValue1[index] == (int) digestValue2[index])
          ++index;
        if (index >= digestValue1.Length)
          flag = true;
      }
      if (!flag)
        throw new DeploymentException(ExceptionTypes.Subscription, Resources.GetString("Ex_ApplicationInplaceUpdate"));
    }

    private void UpdateSubscriptionExposure(SubscriptionState subState)
    {
      this.CheckInstalledAndShellVisible(subState);
      ShellExposure.UpdateSubscriptionShellExposure(subState);
    }

    private static void RemoveSubscriptionExposure(SubscriptionState subState)
    {
      ShellExposure.RemoveSubscriptionShellExposure(subState);
    }

    private bool IsAssemblyInstalled(DefinitionIdentity asmId)
    {
      using (this.AcquireStoreReaderLock())
        return this._compStore.IsAssemblyInstalled(asmId);
    }

    private IDisposable AcquireLock(DefinitionIdentity asmId, bool writer)
    {
      string keyForm = asmId.KeyForm;
      Directory.CreateDirectory(this._deployPath);
      return LockedFile.AcquireLock(Path.Combine(this._deployPath, keyForm), Constants.LockTimeout, writer);
    }

    private static void OnDeploymentAdded(SubscriptionState subState)
    {
    }

    private static void OnDeploymentRemoved(SubscriptionState subState)
    {
    }
  }
}
