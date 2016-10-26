// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.ComponentStore
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using Microsoft.Win32;
using System.Collections;
using System.Deployment.Application.Manifest;
using System.Deployment.Internal.Isolation;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace System.Deployment.Application
{
  internal class ComponentStore
  {
    private const string DateTimeFormatString = "yyyy/MM/dd HH:mm:ss";
    private static object _installReference;
    private ComponentStoreType _storeType;
    private Store _store;
    private IStateManager _stateMgr;
    private SubscriptionStore _subStore;
    private bool _firstRefresh;

    private StoreApplicationReference InstallReference
    {
      get
      {
        if (ComponentStore._installReference == null)
          Interlocked.CompareExchange(ref ComponentStore._installReference, (object) new StoreApplicationReference(IsolationInterop.GUID_SXS_INSTALL_REFERENCE_SCHEME_OPAQUESTRING, "{3f471841-eef2-47d6-89c0-d028f03a4ad5}", (string) null), (object) null);
        return (StoreApplicationReference) ComponentStore._installReference;
      }
    }

    private ComponentStore(ComponentStoreType storeType, SubscriptionStore subStore)
    {
      if (storeType != ComponentStoreType.UserStore)
        throw new NotImplementedException();
      this._storeType = storeType;
      this._subStore = subStore;
      this._store = IsolationInterop.GetUserStore();
      Guid guidOfType = IsolationInterop.GetGuidOfType(typeof (IStateManager));
      this._stateMgr = IsolationInterop.GetUserStateManager(0U, IntPtr.Zero, ref guidOfType) as IStateManager;
      this._firstRefresh = true;
    }

    public static ComponentStore GetStore(ComponentStoreType storeType, SubscriptionStore subStore)
    {
      return new ComponentStore(storeType, subStore);
    }

    internal ulong GetOnlineAppQuotaInBytes()
    {
      uint num1 = 256000;
      using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Classes\\Software\\Microsoft\\Windows\\CurrentVersion\\Deployment"))
      {
        if (registryKey != null)
        {
          object obj = registryKey.GetValue("OnlineAppQuotaInKB");
          if (obj is int)
          {
            int num2 = (int) obj;
            num1 = num2 >= 0 ? (uint) num2 : (uint) (-1 - -num2 + 1);
          }
        }
      }
      return (ulong) num1 * 1024UL;
    }

    internal ulong GetPrivateSize(ArrayList deployAppIds)
    {
      ulong privateSize;
      ulong sharedSize;
      this.GetPrivateAndSharedSizes(deployAppIds, out privateSize, out sharedSize);
      return privateSize;
    }

    internal ulong GetSharedSize(ArrayList deployAppIds)
    {
      ulong privateSize;
      ulong sharedSize;
      this.GetPrivateAndSharedSizes(deployAppIds, out privateSize, out sharedSize);
      return sharedSize;
    }

    internal ArrayList CollectCrossGroupApplications(Uri codebaseUri, DefinitionIdentity deploymentIdentity, ref bool identityGroupFound, ref bool locationGroupFound, ref string identityGroupProductName)
    {
      Hashtable hashtable = new Hashtable();
      ArrayList arrayList = new ArrayList();
      foreach (STORE_ASSEMBLY enumAssembly in this._store.EnumAssemblies(Store.EnumAssembliesFlags.Nothing))
      {
        DefinitionIdentity subscriptionId = new DefinitionIdentity(enumAssembly.DefinitionIdentity).ToSubscriptionId();
        SubscriptionState subscriptionState = this._subStore.GetSubscriptionState(subscriptionId);
        if (subscriptionState.IsInstalled)
        {
          bool flag1 = subscriptionState.DeploymentProviderUri.Equals((object) codebaseUri);
          bool flag2 = subscriptionState.PKTGroupId.Equals((object) deploymentIdentity.ToPKTGroupId());
          bool flag3 = subscriptionState.SubscriptionId.PublicKeyToken.Equals(deploymentIdentity.ToSubscriptionId().PublicKeyToken);
          if (!(flag1 & flag2 & flag3))
          {
            if (flag1 & flag2 && !flag3)
            {
              if (!hashtable.Contains((object) subscriptionId))
              {
                hashtable.Add((object) subscriptionId, (object) subscriptionState);
                arrayList.Add((object) new ComponentStore.CrossGroupApplicationData(subscriptionState, ComponentStore.CrossGroupApplicationData.GroupType.LocationGroup));
                locationGroupFound = true;
              }
            }
            else if (!flag1 & flag2 & flag3 && !hashtable.Contains((object) subscriptionId))
            {
              hashtable.Add((object) subscriptionId, (object) subscriptionState);
              arrayList.Add((object) new ComponentStore.CrossGroupApplicationData(subscriptionState, ComponentStore.CrossGroupApplicationData.GroupType.IdentityGroup));
              if (subscriptionState.CurrentDeploymentManifest != null && subscriptionState.CurrentDeploymentManifest.Description != null && subscriptionState.CurrentDeploymentManifest.Description.Product != null)
                identityGroupProductName = subscriptionState.CurrentDeploymentManifest.Description.Product;
              identityGroupFound = true;
            }
          }
        }
      }
      return arrayList;
    }

    internal void RemoveApplicationInstance(SubscriptionState subState, DefinitionAppId appId)
    {
      using (ComponentStore.StoreTransactionContext storeTxn = new ComponentStore.StoreTransactionContext(this))
      {
        this.PrepareRemoveDeployment(storeTxn, subState, appId);
        this.SubmitStoreTransaction(storeTxn, subState);
      }
    }

    private void GetPrivateAndSharedSizes(ArrayList deployAppIds, out ulong privateSize, out ulong sharedSize)
    {
      privateSize = 0UL;
      sharedSize = 0UL;
      if (deployAppIds == null || deployAppIds.Count <= 0)
        return;
      IDefinitionAppId[] comPtrs = ComponentStore.DeployAppIdsToComPtrs(deployAppIds);
      this.CalculateDeploymentsUnderQuota(comPtrs.Length, comPtrs, ulong.MaxValue, ref privateSize, ref sharedSize);
    }

    private int CalculateDeploymentsUnderQuota(int numberOfDeployments, IDefinitionAppId[] deployAppIdPtrs, ulong quotaSize, ref ulong privateSize, ref ulong sharedSize)
    {
      uint Delimiter = 0;
      StoreApplicationReference installReference = this.InstallReference;
      this._store.CalculateDelimiterOfDeploymentsBasedOnQuota(0U, (uint) numberOfDeployments, deployAppIdPtrs, ref installReference, quotaSize, ref Delimiter, ref sharedSize, ref privateSize);
      return (int) Delimiter;
    }

    private static IDefinitionAppId[] DeployAppIdsToComPtrs(ArrayList deployAppIdList)
    {
      IDefinitionAppId[] definitionAppIdArray = new IDefinitionAppId[deployAppIdList.Count];
      for (int index = 0; index < deployAppIdList.Count; ++index)
        definitionAppIdArray[index] = ((DefinitionAppId) deployAppIdList[index]).ComPointer;
      return definitionAppIdArray;
    }

    public void RefreshStorePointer()
    {
      if (this._firstRefresh)
      {
        this._firstRefresh = false;
      }
      else
      {
        if (this._storeType != ComponentStoreType.UserStore)
          throw new NotImplementedException();
        Marshal.ReleaseComObject((object) this._store.InternalStore);
        Marshal.ReleaseComObject((object) this._stateMgr);
        this._store = IsolationInterop.GetUserStore();
        Guid guidOfType = IsolationInterop.GetGuidOfType(typeof (IStateManager));
        this._stateMgr = IsolationInterop.GetUserStateManager(0U, IntPtr.Zero, ref guidOfType) as IStateManager;
      }
    }

    public void CleanOnlineAppCache()
    {
      using (ComponentStore.StoreTransactionContext transactionContext = new ComponentStore.StoreTransactionContext(this))
        transactionContext.ScavengeContext.CleanOnlineAppCache();
    }

    public void CommitApplication(SubscriptionState subState, CommitApplicationParams commitParams)
    {
      try
      {
        using (ComponentStore.StoreTransactionContext storeTxn = new ComponentStore.StoreTransactionContext(this))
        {
          this.PrepareCommitApplication(storeTxn, subState, commitParams);
          this.SubmitStoreTransactionCheckQuota(storeTxn, subState);
        }
      }
      catch (COMException ex)
      {
        if (ex.ErrorCode == -2147024784)
          throw new DeploymentException(ExceptionTypes.DiskIsFull, Resources.GetString("Ex_StoreOperationFailed"), (Exception) ex);
        if (ex.ErrorCode == -2147023590)
          throw new DeploymentException(ExceptionTypes.ComponentStore, Resources.GetString("Ex_InplaceUpdateOfApplicationAttempted"), (Exception) ex);
        throw;
      }
    }

    public void RemoveSubscription(SubscriptionState subState)
    {
      try
      {
        using (ComponentStore.StoreTransactionContext storeTxn = new ComponentStore.StoreTransactionContext(this))
        {
          this.PrepareRemoveSubscription(storeTxn, subState);
          this.SubmitStoreTransactionCheckQuota(storeTxn, subState);
        }
      }
      catch (COMException ex)
      {
        if (ex.ErrorCode == -2147024784)
          throw new DeploymentException(ExceptionTypes.DiskIsFull, Resources.GetString("Ex_StoreOperationFailed"), (Exception) ex);
        throw;
      }
    }

    public void RollbackSubscription(SubscriptionState subState)
    {
      try
      {
        using (ComponentStore.StoreTransactionContext storeTxn = new ComponentStore.StoreTransactionContext(this))
        {
          this.PrepareRollbackSubscription(storeTxn, subState);
          this.SubmitStoreTransactionCheckQuota(storeTxn, subState);
        }
      }
      catch (COMException ex)
      {
        if (ex.ErrorCode == -2147024784)
          throw new DeploymentException(ExceptionTypes.DiskIsFull, Resources.GetString("Ex_StoreOperationFailed"), (Exception) ex);
        throw;
      }
    }

    public void SetPendingDeployment(SubscriptionState subState, DefinitionIdentity deployId, DateTime checkTime)
    {
      try
      {
        using (ComponentStore.StoreTransactionContext storeTxn = new ComponentStore.StoreTransactionContext(this))
        {
          this.PrepareSetPendingDeployment(storeTxn, subState, deployId, checkTime);
          this.SubmitStoreTransaction(storeTxn, subState);
        }
      }
      catch (COMException ex)
      {
        if (ex.ErrorCode == -2147024784)
          throw new DeploymentException(ExceptionTypes.DiskIsFull, Resources.GetString("Ex_StoreOperationFailed"), (Exception) ex);
        throw;
      }
    }

    public void SetUpdateSkipTime(SubscriptionState subState, DefinitionIdentity updateSkippedDeployment, DateTime updateSkipTime)
    {
      try
      {
        using (ComponentStore.StoreTransactionContext storeTxn = new ComponentStore.StoreTransactionContext(this))
        {
          this.PrepareUpdateSkipTime(storeTxn, subState, updateSkippedDeployment, updateSkipTime);
          this.SubmitStoreTransaction(storeTxn, subState);
        }
      }
      catch (COMException ex)
      {
        if (ex.ErrorCode == -2147024784)
          throw new DeploymentException(ExceptionTypes.DiskIsFull, Resources.GetString("Ex_StoreOperationFailed"), (Exception) ex);
        throw;
      }
    }

    public SubscriptionStateInternal GetSubscriptionStateInternal(SubscriptionState subState)
    {
      return this.GetSubscriptionStateInternal(subState.SubscriptionId);
    }

    public SubscriptionStateInternal GetSubscriptionStateInternal(DefinitionIdentity subId)
    {
      SubscriptionStateInternal subscriptionStateInternal = new SubscriptionStateInternal();
      subscriptionStateInternal.IsInstalled = this.IsSubscriptionInstalled(subId);
      if (subscriptionStateInternal.IsInstalled)
      {
        DefinitionAppId appId = new DefinitionAppId(new DefinitionIdentity[1]
        {
          subId
        });
        subscriptionStateInternal.IsShellVisible = this.GetPropertyBoolean(appId, "IsShellVisible");
        subscriptionStateInternal.CurrentBind = this.GetPropertyDefinitionAppId(appId, "CurrentBind");
        subscriptionStateInternal.PreviousBind = this.GetPropertyDefinitionAppId(appId, "PreviousBind");
        subscriptionStateInternal.PendingBind = this.GetPropertyDefinitionAppId(appId, "PendingBind");
        subscriptionStateInternal.ExcludedDeployment = this.GetPropertyDefinitionIdentity(appId, "ExcludedDeployment");
        subscriptionStateInternal.PendingDeployment = this.GetPropertyDefinitionIdentity(appId, "PendingDeployment");
        subscriptionStateInternal.DeploymentProviderUri = this.GetPropertyUri(appId, "DeploymentProviderUri");
        subscriptionStateInternal.MinimumRequiredVersion = this.GetPropertyVersion(appId, "MinimumRequiredVersion");
        subscriptionStateInternal.LastCheckTime = this.GetPropertyDateTime(appId, "LastCheckTime");
        subscriptionStateInternal.UpdateSkippedDeployment = this.GetPropertyDefinitionIdentity(appId, "UpdateSkippedDeployment");
        subscriptionStateInternal.UpdateSkipTime = this.GetPropertyDateTime(appId, "UpdateSkipTime");
        subscriptionStateInternal.appType = this.GetPropertyAppType(appId, "AppType");
        if (subscriptionStateInternal.CurrentBind == null)
          throw new InvalidDeploymentException(Resources.GetString("Ex_NoCurrentBind"));
        subscriptionStateInternal.CurrentDeployment = subscriptionStateInternal.CurrentBind.DeploymentIdentity;
        subscriptionStateInternal.CurrentDeploymentManifest = this.GetAssemblyManifest(subscriptionStateInternal.CurrentDeployment);
        subscriptionStateInternal.CurrentDeploymentSourceUri = this.GetPropertyUri(subscriptionStateInternal.CurrentBind, "DeploymentSourceUri");
        subscriptionStateInternal.CurrentApplication = subscriptionStateInternal.CurrentBind.ApplicationIdentity;
        subscriptionStateInternal.CurrentApplicationManifest = this.GetAssemblyManifest(subscriptionStateInternal.CurrentBind.ApplicationIdentity);
        subscriptionStateInternal.CurrentApplicationSourceUri = this.GetPropertyUri(subscriptionStateInternal.CurrentBind, "ApplicationSourceUri");
        DefinitionIdentity definitionIdentity = subscriptionStateInternal.PreviousBind != null ? subscriptionStateInternal.PreviousBind.DeploymentIdentity : (DefinitionIdentity) null;
        subscriptionStateInternal.RollbackDeployment = definitionIdentity == null || !(subscriptionStateInternal.MinimumRequiredVersion == (Version) null) && !(definitionIdentity.Version >= subscriptionStateInternal.MinimumRequiredVersion) ? (DefinitionIdentity) null : definitionIdentity;
        if (subscriptionStateInternal.PreviousBind != null)
        {
          try
          {
            subscriptionStateInternal.PreviousApplication = subscriptionStateInternal.PreviousBind.ApplicationIdentity;
            subscriptionStateInternal.PreviousApplicationManifest = this.GetAssemblyManifest(subscriptionStateInternal.PreviousBind.ApplicationIdentity);
          }
          catch (Exception ex)
          {
            if (ExceptionUtility.IsHardException(ex))
            {
              throw;
            }
            else
            {
              Logger.AddInternalState("Exception thrown for GetAssemblyManifest in GetSubscriptionStateInternal: " + ex.GetType().ToString() + ":" + ex.Message);
              subscriptionStateInternal.PreviousBind = (DefinitionAppId) null;
              subscriptionStateInternal.RollbackDeployment = (DefinitionIdentity) null;
              subscriptionStateInternal.PreviousApplication = (DefinitionIdentity) null;
              subscriptionStateInternal.PreviousApplicationManifest = (AssemblyManifest) null;
            }
          }
        }
      }
      return subscriptionStateInternal;
    }

    public void ActivateApplication(DefinitionAppId appId, string activationParameter, bool useActivationParameter)
    {
      ComponentStore.HostType hostType1 = this.GetHostTypeFromMetadata(appId);
      switch (PolicyKeys.ClrHostType())
      {
        case PolicyKeys.HostType.AppLaunch:
          hostType1 = ComponentStore.HostType.AppLaunch;
          break;
        case PolicyKeys.HostType.Cor:
          hostType1 = ComponentStore.HostType.CorFlag;
          break;
      }
      string applicationFullName = appId.ToString();
      AssemblyManifest assemblyManifest = this.GetAssemblyManifest(appId.DeploymentIdentity);
      Logger.AddMethodCall("ComponentStore.ActivateApplication(appId=[" + applicationFullName + "] ,activationParameter=" + activationParameter + ",useActivationParameter=" + useActivationParameter.ToString() + ") called.");
      Logger.AddInternalState("HostType=" + (object) (uint) hostType1);
      int activationDataCount = 0;
      string[] activationData = (string[]) null;
      if (activationParameter != null)
      {
        if (assemblyManifest.Deployment.TrustURLParameters | useActivationParameter)
        {
          activationDataCount = 1;
          activationData = new string[1]
          {
            activationParameter
          };
        }
        else
          Logger.AddInternalState("Activation parameters are not passed.");
      }
      uint hostType2 = (uint) hostType1;
      if (!assemblyManifest.Deployment.Install)
        hostType2 |= 2147483904U;
      try
      {
        Logger.AddInternalState("Activating application via CorLaunchApplication.");
        NativeMethods.CorLaunchApplication(hostType2, applicationFullName, 0, (string[]) null, activationDataCount, activationData, new NativeMethods.PROCESS_INFORMATION());
      }
      catch (COMException ex)
      {
        int num = ex.ErrorCode & (int) ushort.MaxValue;
        if (num >= 14000 && num <= 14999)
          throw new DeploymentException(ExceptionTypes.Activation, Resources.GetString("Ex_ActivationFailureDueToSxSError"), (Exception) ex);
        if (ex.ErrorCode == -2147024784)
          throw new DeploymentException(ExceptionTypes.DiskIsFull, Resources.GetString("Ex_StoreOperationFailed"), (Exception) ex);
        throw;
      }
      catch (UnauthorizedAccessException ex)
      {
        throw new DeploymentException(ExceptionTypes.Activation, Resources.GetString("Ex_GenericActivationFailure"), (Exception) ex);
      }
      catch (IOException ex)
      {
        throw new DeploymentException(ExceptionTypes.Activation, Resources.GetString("Ex_GenericActivationFailure"), (Exception) ex);
      }
    }

    public bool IsAssemblyInstalled(DefinitionIdentity asmId)
    {
      IDefinitionIdentity definitionIdentity = (IDefinitionIdentity) null;
      try
      {
        definitionIdentity = this._store.GetAssemblyIdentity(0U, asmId.ComPointer);
        return true;
      }
      catch (COMException ex)
      {
        return false;
      }
      finally
      {
        if (definitionIdentity != null)
          Marshal.ReleaseComObject((object) definitionIdentity);
      }
    }

    public Store.IPathLock LockApplicationPath(DefinitionAppId definitionAppId)
    {
      try
      {
        return this._store.LockApplicationPath(definitionAppId.ComPointer);
      }
      catch (COMException ex)
      {
        if (ex.ErrorCode == -2147024784)
          throw new DeploymentException(ExceptionTypes.DiskIsFull, Resources.GetString("Ex_StoreOperationFailed"), (Exception) ex);
        throw;
      }
    }

    public Store.IPathLock LockAssemblyPath(DefinitionIdentity asmId)
    {
      try
      {
        return this._store.LockAssemblyPath(asmId.ComPointer);
      }
      catch (COMException ex)
      {
        if (ex.ErrorCode == -2147024784)
          throw new DeploymentException(ExceptionTypes.DiskIsFull, Resources.GetString("Ex_StoreOperationFailed"), (Exception) ex);
        throw;
      }
    }

    public bool CheckGroupInstalled(DefinitionAppId appId, string groupName)
    {
      AssemblyManifest assemblyManifest = this.GetAssemblyManifest(appId.ApplicationIdentity);
      return this.CheckGroupInstalled(appId, assemblyManifest, groupName);
    }

    public bool CheckGroupInstalled(DefinitionAppId appId, AssemblyManifest appManifest, string groupName)
    {
      Store.IPathLock pathLock = (Store.IPathLock) null;
      try
      {
        pathLock = this.LockApplicationPath(appId);
        string path = pathLock.Path;
        System.Deployment.Application.Manifest.File[] filesInGroup = appManifest.GetFilesInGroup(groupName, true);
        foreach (System.Deployment.Application.Manifest.File file in filesInGroup)
        {
          if (!System.IO.File.Exists(Path.Combine(path, file.NameFS)))
            return false;
        }
        DependentAssembly[] assembliesInGroup = appManifest.GetPrivateAssembliesInGroup(groupName, true);
        foreach (DependentAssembly dependentAssembly in assembliesInGroup)
        {
          if (!System.IO.File.Exists(Path.Combine(path, dependentAssembly.CodebaseFS)))
            return false;
        }
        if (filesInGroup.Length + assembliesInGroup.Length == 0)
          throw new InvalidDeploymentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_NoSuchDownloadGroup"), new object[1]
          {
            (object) groupName
          }));
      }
      finally
      {
        if (pathLock != null)
          pathLock.Dispose();
      }
      return true;
    }

    private ComponentStore.HostType GetHostTypeFromMetadata(DefinitionAppId defAppId)
    {
      ComponentStore.HostType hostType = ComponentStore.HostType.Default;
      try
      {
        hostType = !this.GetPropertyBoolean(defAppId, "IsFullTrust") ? ComponentStore.HostType.AppLaunch : ComponentStore.HostType.CorFlag;
      }
      catch (DeploymentException ex)
      {
      }
      return hostType;
    }

    private AssemblyManifest GetAssemblyManifest(DefinitionIdentity asmId)
    {
      return new AssemblyManifest(this._store.GetAssemblyManifest(0U, asmId.ComPointer));
    }

    private bool IsSubscriptionInstalled(DefinitionIdentity subId)
    {
      DefinitionAppId appId = new DefinitionAppId(new DefinitionIdentity[1]
      {
        subId
      });
      try
      {
        return this.GetPropertyDefinitionAppId(appId, "CurrentBind") != null;
      }
      catch (DeploymentException ex)
      {
        return false;
      }
    }

    private string GetPropertyString(DefinitionAppId appId, string propName)
    {
      byte[] deploymentProperty;
      try
      {
        deploymentProperty = this._store.GetDeploymentProperty(Store.GetPackagePropertyFlags.Nothing, appId.ComPointer, this.InstallReference, Constants.DeploymentPropertySet, propName);
      }
      catch (COMException ex)
      {
        return (string) null;
      }
      int length = deploymentProperty.Length;
      if (length == 0 || deploymentProperty.Length % 2 != 0 || ((int) deploymentProperty[length - 2] != 0 || (int) deploymentProperty[length - 1] != 0))
        throw new DeploymentException(ExceptionTypes.SubscriptionState, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_InvalidStoreMetaData"), new object[1]
        {
          (object) propName
        }));
      return Encoding.Unicode.GetString(deploymentProperty, 0, length - 2);
    }

    private DefinitionIdentity GetPropertyDefinitionIdentity(DefinitionAppId appId, string propName)
    {
      try
      {
        string propertyString = this.GetPropertyString(appId, propName);
        return propertyString == null || propertyString.Length <= 0 ? (DefinitionIdentity) null : new DefinitionIdentity(propertyString);
      }
      catch (COMException ex)
      {
        throw new DeploymentException(ExceptionTypes.SubscriptionState, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_InvalidStoreMetaData"), new object[1]
        {
          (object) propName
        }), (Exception) ex);
      }
      catch (SEHException ex)
      {
        throw new DeploymentException(ExceptionTypes.SubscriptionState, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_InvalidStoreMetaData"), new object[1]
        {
          (object) propName
        }), (Exception) ex);
      }
    }

    private DefinitionAppId GetPropertyDefinitionAppId(DefinitionAppId appId, string propName)
    {
      try
      {
        string propertyString = this.GetPropertyString(appId, propName);
        return propertyString == null || propertyString.Length <= 0 ? (DefinitionAppId) null : new DefinitionAppId(propertyString);
      }
      catch (COMException ex)
      {
        throw new DeploymentException(ExceptionTypes.SubscriptionState, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_InvalidStoreMetaData"), new object[1]
        {
          (object) propName
        }), (Exception) ex);
      }
      catch (SEHException ex)
      {
        throw new DeploymentException(ExceptionTypes.SubscriptionState, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_InvalidStoreMetaData"), new object[1]
        {
          (object) propName
        }), (Exception) ex);
      }
    }

    private bool GetPropertyBoolean(DefinitionAppId appId, string propName)
    {
      try
      {
        string propertyString = this.GetPropertyString(appId, propName);
        return propertyString != null && propertyString.Length > 0 && Convert.ToBoolean(propertyString, (IFormatProvider) CultureInfo.InvariantCulture);
      }
      catch (FormatException ex)
      {
        throw new DeploymentException(ExceptionTypes.SubscriptionState, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_InvalidStoreMetaData"), new object[1]
        {
          (object) propName
        }), (Exception) ex);
      }
    }

    private Uri GetPropertyUri(DefinitionAppId appId, string propName)
    {
      try
      {
        string propertyString = this.GetPropertyString(appId, propName);
        return propertyString == null || propertyString.Length <= 0 ? (Uri) null : new Uri(propertyString);
      }
      catch (UriFormatException ex)
      {
        throw new DeploymentException(ExceptionTypes.SubscriptionState, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_InvalidStoreMetaData"), new object[1]
        {
          (object) propName
        }), (Exception) ex);
      }
    }

    private Version GetPropertyVersion(DefinitionAppId appId, string propName)
    {
      try
      {
        string propertyString = this.GetPropertyString(appId, propName);
        return propertyString == null || propertyString.Length <= 0 ? (Version) null : new Version(propertyString);
      }
      catch (ArgumentException ex)
      {
        throw new DeploymentException(ExceptionTypes.SubscriptionState, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_InvalidStoreMetaData"), new object[1]
        {
          (object) propName
        }), (Exception) ex);
      }
      catch (FormatException ex)
      {
        throw new DeploymentException(ExceptionTypes.SubscriptionState, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_InvalidStoreMetaData"), new object[1]
        {
          (object) propName
        }), (Exception) ex);
      }
    }

    private DateTime GetPropertyDateTime(DefinitionAppId appId, string propName)
    {
      try
      {
        string propertyString = this.GetPropertyString(appId, propName);
        return propertyString == null || propertyString.Length <= 0 ? DateTime.MinValue : DateTime.ParseExact(propertyString, "yyyy/MM/dd HH:mm:ss", (IFormatProvider) DateTimeFormatInfo.InvariantInfo);
      }
      catch (FormatException ex)
      {
        throw new DeploymentException(ExceptionTypes.SubscriptionState, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_InvalidStoreMetaData"), new object[1]
        {
          (object) propName
        }), (Exception) ex);
      }
    }

    private AppType GetPropertyAppType(DefinitionAppId appId, string propName)
    {
      string str = (string) null;
      try
      {
        str = this.GetPropertyString(appId, propName);
        if (str == null)
          return AppType.None;
        switch (Convert.ToUInt16(str, (IFormatProvider) CultureInfo.InvariantCulture))
        {
          case 0:
            return AppType.None;
          case 1:
            return AppType.Installed;
          case 2:
            return AppType.Online;
          case 3:
            return AppType.CustomHostSpecified;
          case 4:
            return AppType.CustomUX;
          default:
            return AppType.None;
        }
      }
      catch (DeploymentException ex)
      {
        return AppType.None;
      }
      catch (FormatException ex)
      {
        Logger.AddInternalState("Unable to convert store property," + propName + ", from string to UInt16." + propName + "=" + str);
        throw new DeploymentException(ExceptionTypes.SubscriptionState, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_InvalidStoreMetaData"), new object[1]
        {
          (object) propName
        }), (Exception) ex);
      }
      catch (OverflowException ex)
      {
        Logger.AddInternalState("Unable to convert store property," + propName + ", from string to UInt16." + propName + "=" + str);
        throw new DeploymentException(ExceptionTypes.SubscriptionState, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_InvalidStoreMetaData"), new object[1]
        {
          (object) propName
        }), (Exception) ex);
      }
    }

    private void PrepareCommitApplication(ComponentStore.StoreTransactionContext storeTxn, SubscriptionState subState, CommitApplicationParams commitParams)
    {
      DefinitionAppId appId = commitParams.AppId;
      SubscriptionStateInternal newState = (SubscriptionStateInternal) null;
      if (commitParams.CommitDeploy)
      {
        newState = this.PrepareCommitDeploymentState(storeTxn, subState, commitParams);
        if (commitParams.IsConfirmed && appId.Equals((object) newState.CurrentBind) || !commitParams.IsConfirmed && appId.Equals((object) newState.PendingBind))
          this.PrepareStageDeploymentComponent(storeTxn, subState, commitParams);
      }
      if (commitParams.CommitApp)
      {
        this.PrepareStageAppComponent(storeTxn, commitParams);
        if (!commitParams.DeployManifest.Deployment.Install && commitParams.appType != AppType.CustomHostSpecified)
          storeTxn.ScavengeContext.AddOnlineAppToCommit(appId, subState);
      }
      if (!commitParams.CommitDeploy)
        return;
      this.PrepareSetSubscriptionState(storeTxn, subState, newState);
    }

    private SubscriptionStateInternal PrepareCommitDeploymentState(ComponentStore.StoreTransactionContext storeTxn, SubscriptionState subState, CommitApplicationParams commitParams)
    {
      DefinitionAppId appId = commitParams.AppId;
      AssemblyManifest deployManifest = commitParams.DeployManifest;
      SubscriptionStateInternal newState = new SubscriptionStateInternal(subState);
      if (commitParams.IsConfirmed)
      {
        newState.IsInstalled = true;
        newState.IsShellVisible = deployManifest.Deployment.Install;
        newState.DeploymentProviderUri = deployManifest.Deployment.ProviderCodebaseUri != (Uri) null ? deployManifest.Deployment.ProviderCodebaseUri : commitParams.DeploySourceUri;
        if (deployManifest.Deployment.MinimumRequiredVersion != (Version) null)
          newState.MinimumRequiredVersion = deployManifest.Deployment.MinimumRequiredVersion;
        if (!appId.Equals((object) subState.CurrentBind))
        {
          newState.CurrentBind = appId;
          newState.PreviousBind = !newState.IsShellVisible || subState.IsShellVisible ? subState.CurrentBind : (DefinitionAppId) null;
        }
        newState.PendingBind = (DefinitionAppId) null;
        newState.PendingDeployment = (DefinitionIdentity) null;
        newState.ExcludedDeployment = (DefinitionIdentity) null;
        newState.appType = commitParams.appType;
        ComponentStore.ResetUpdateSkippedState(newState);
      }
      else
      {
        newState.PendingBind = appId;
        newState.PendingDeployment = appId.DeploymentIdentity;
        if (!newState.PendingDeployment.Equals((object) subState.UpdateSkippedDeployment))
          ComponentStore.ResetUpdateSkippedState(newState);
      }
      newState.LastCheckTime = commitParams.TimeStamp;
      ComponentStore.FinalizeSubscriptionState(newState);
      return newState;
    }

    private void PrepareStageDeploymentComponent(ComponentStore.StoreTransactionContext storeTxn, SubscriptionState subState, CommitApplicationParams commitParams)
    {
      DefinitionAppId deploymentAppId = commitParams.AppId.ToDeploymentAppId();
      string deployManifestPath = commitParams.DeployManifestPath;
      storeTxn.Add(new StoreOperationStageComponent(deploymentAppId.ComPointer, deployManifestPath));
      this.PrepareSetDeploymentProperties(storeTxn, commitParams.AppId, commitParams);
    }

    private void PrepareSetDeploymentProperties(ComponentStore.StoreTransactionContext storeTxn, DefinitionAppId appId, CommitApplicationParams commitParams)
    {
      string str1 = (string) null;
      string str2 = (string) null;
      string str3 = (string) null;
      if (commitParams != null)
      {
        str1 = ComponentStore.ToPropertyString((object) commitParams.DeploySourceUri);
        str2 = ComponentStore.ToPropertyString((object) commitParams.AppSourceUri);
        if (commitParams.Trust != null)
        {
          if (commitParams.appType != AppType.CustomHostSpecified)
            str3 = ComponentStore.ToPropertyString((object) commitParams.Trust.DefaultGrantSet.PermissionSet.IsUnrestricted());
        }
        else if (commitParams.IsUpdateInPKTGroup && commitParams.IsFullTrustRequested)
          str3 = ComponentStore.ToPropertyString((object) commitParams.IsFullTrustRequested);
      }
      StoreOperationMetadataProperty[] SetProperties = new StoreOperationMetadataProperty[3]
      {
        new StoreOperationMetadataProperty(Constants.DeploymentPropertySet, "DeploymentSourceUri", str1),
        new StoreOperationMetadataProperty(Constants.DeploymentPropertySet, "ApplicationSourceUri", str2),
        new StoreOperationMetadataProperty(Constants.DeploymentPropertySet, "IsFullTrust", str3)
      };
      storeTxn.Add(new StoreOperationSetDeploymentMetadata(appId.ComPointer, this.InstallReference, SetProperties));
    }

    private void PrepareStageAppComponent(ComponentStore.StoreTransactionContext storeTxn, CommitApplicationParams commitParams)
    {
      DefinitionAppId appId = commitParams.AppId;
      AssemblyManifest appManifest = commitParams.AppManifest;
      string appManifestPath = commitParams.AppManifestPath;
      string appPayloadPath = commitParams.AppPayloadPath;
      string appGroup = commitParams.AppGroup;
      if (appGroup == null)
      {
        if (appManifestPath == null)
          throw new ArgumentNullException("commitParams");
        storeTxn.Add(new StoreOperationStageComponent(appId.ComPointer, appManifestPath));
      }
      foreach (System.Deployment.Application.Manifest.File file in appManifest.GetFilesInGroup(appGroup, true))
        this.PrepareInstallFile(storeTxn, file, appId, (DefinitionIdentity) null, appPayloadPath);
      foreach (DependentAssembly privAsm in appManifest.GetPrivateAssembliesInGroup(appGroup, true))
        this.PrepareInstallPrivateAssembly(storeTxn, privAsm, appId, appPayloadPath);
    }

    private void PrepareInstallFile(ComponentStore.StoreTransactionContext storeTxn, System.Deployment.Application.Manifest.File file, DefinitionAppId appId, DefinitionIdentity asmId, string asmPayloadPath)
    {
      string SrcFile = Path.Combine(asmPayloadPath, file.NameFS);
      string name = file.Name;
      storeTxn.Add(new StoreOperationStageComponentFile(appId.ComPointer, asmId != null ? asmId.ComPointer : (IDefinitionIdentity) null, name, SrcFile));
    }

    private void PrepareInstallPrivateAssembly(ComponentStore.StoreTransactionContext storeTxn, DependentAssembly privAsm, DefinitionAppId appId, string appPayloadPath)
    {
      string codebaseFs = privAsm.CodebaseFS;
      string str1 = Path.Combine(appPayloadPath, codebaseFs);
      string directoryName = Path.GetDirectoryName(str1);
      AssemblyManifest manifest = new AssemblyManifest(str1);
      DefinitionIdentity asmId = manifest.Identity;
      string str2 = manifest.RawXmlFilePath;
      if (str2 == null)
      {
        str2 = str1 + ".genman";
        asmId = ManifestGenerator.GenerateManifest(privAsm.Identity, manifest, str2);
      }
      storeTxn.Add(new StoreOperationStageComponent(appId.ComPointer, asmId.ComPointer, str2));
      foreach (System.Deployment.Application.Manifest.File file in manifest.Files)
        this.PrepareInstallFile(storeTxn, file, appId, asmId, directoryName);
    }

    private void PrepareRemoveSubscription(ComponentStore.StoreTransactionContext storeTxn, SubscriptionState subState)
    {
      this.PrepareFinalizeSubscriptionState(storeTxn, subState, new SubscriptionStateInternal(subState)
      {
        IsInstalled = false
      });
    }

    private void PrepareRollbackSubscription(ComponentStore.StoreTransactionContext storeTxn, SubscriptionState subState)
    {
      this.PrepareFinalizeSubscriptionState(storeTxn, subState, new SubscriptionStateInternal(subState)
      {
        ExcludedDeployment = subState.CurrentBind.DeploymentIdentity,
        CurrentBind = subState.PreviousBind,
        PreviousBind = (DefinitionAppId) null
      });
    }

    private void PrepareSetPendingDeployment(ComponentStore.StoreTransactionContext storeTxn, SubscriptionState subState, DefinitionIdentity deployId, DateTime checkTime)
    {
      SubscriptionStateInternal newState = new SubscriptionStateInternal(subState);
      newState.PendingDeployment = deployId;
      newState.LastCheckTime = checkTime;
      if (newState.PendingDeployment != null && !newState.PendingDeployment.Equals((object) subState.UpdateSkippedDeployment))
        ComponentStore.ResetUpdateSkippedState(newState);
      this.PrepareFinalizeSubscriptionState(storeTxn, subState, newState);
    }

    private void PrepareUpdateSkipTime(ComponentStore.StoreTransactionContext storeTxn, SubscriptionState subState, DefinitionIdentity updateSkippedDeployment, DateTime updateSkipTime)
    {
      this.PrepareFinalizeSubscriptionState(storeTxn, subState, new SubscriptionStateInternal(subState)
      {
        UpdateSkippedDeployment = updateSkippedDeployment,
        UpdateSkipTime = updateSkipTime
      });
    }

    private void PrepareFinalizeSubscriptionState(ComponentStore.StoreTransactionContext storeTxn, SubscriptionState subState, SubscriptionStateInternal newState)
    {
      ComponentStore.FinalizeSubscriptionState(newState);
      this.PrepareSetSubscriptionState(storeTxn, subState, newState);
    }

    private void PrepareSetSubscriptionState(ComponentStore.StoreTransactionContext storeTxn, SubscriptionState subState, SubscriptionStateInternal newState)
    {
      this.PrepareFinalizeSubscription(storeTxn, subState, newState);
      this.PrepareSetSubscriptionProperties(storeTxn, subState, newState);
      this.PrepareRemoveOrphanedDeployments(storeTxn, subState, newState);
    }

    private static void FinalizeSubscriptionState(SubscriptionStateInternal newState)
    {
      if (!newState.IsInstalled)
      {
        newState.Reset();
      }
      else
      {
        DefinitionAppId currentBind = newState.CurrentBind;
        DefinitionIdentity deploymentIdentity = currentBind.DeploymentIdentity;
        DefinitionAppId definitionAppId1 = newState.PreviousBind;
        if (definitionAppId1 != null && definitionAppId1.Equals((object) currentBind))
          newState.PreviousBind = definitionAppId1 = (DefinitionAppId) null;
        DefinitionIdentity definitionIdentity1 = definitionAppId1 != null ? definitionAppId1.DeploymentIdentity : (DefinitionIdentity) null;
        DefinitionIdentity definitionIdentity2 = newState.ExcludedDeployment;
        if (definitionIdentity2 != null && (definitionIdentity2.Equals((object) deploymentIdentity) || definitionIdentity2.Equals((object) definitionIdentity1)))
          newState.ExcludedDeployment = definitionIdentity2 = (DefinitionIdentity) null;
        DefinitionIdentity definitionIdentity3 = newState.PendingDeployment;
        if (definitionIdentity3 != null && (definitionIdentity3.Equals((object) deploymentIdentity) || definitionIdentity3.Equals((object) definitionIdentity2)))
          newState.PendingDeployment = definitionIdentity3 = (DefinitionIdentity) null;
        DefinitionAppId pendingBind = newState.PendingBind;
        if (pendingBind == null || pendingBind.DeploymentIdentity.Equals((object) definitionIdentity3) && !pendingBind.Equals((object) definitionAppId1))
          return;
        DefinitionAppId definitionAppId2;
        newState.PendingBind = definitionAppId2 = (DefinitionAppId) null;
      }
    }

    private static void ResetUpdateSkippedState(SubscriptionStateInternal newState)
    {
      newState.UpdateSkippedDeployment = (DefinitionIdentity) null;
      newState.UpdateSkipTime = DateTime.MinValue;
    }

    private void PrepareSetSubscriptionProperties(ComponentStore.StoreTransactionContext storeTxn, SubscriptionState subState, SubscriptionStateInternal newState)
    {
      Logger.AddInternalState("Changing Subscription Properties:");
      Logger.AddInternalState("Old subscription state = " + subState.ToString());
      Logger.AddInternalState("New subscription state = " + newState.ToString());
      SubscriptionStateVariable[] subscriptionStateVariableArray = new SubscriptionStateVariable[12]
      {
        new SubscriptionStateVariable("IsShellVisible", (object) newState.IsShellVisible, (object) subState.IsShellVisible),
        new SubscriptionStateVariable("PreviousBind", (object) newState.PreviousBind, (object) subState.PreviousBind),
        new SubscriptionStateVariable("PendingBind", (object) newState.PendingBind, (object) subState.PendingBind),
        new SubscriptionStateVariable("ExcludedDeployment", (object) newState.ExcludedDeployment, (object) subState.ExcludedDeployment),
        new SubscriptionStateVariable("PendingDeployment", (object) newState.PendingDeployment, (object) subState.PendingDeployment),
        new SubscriptionStateVariable("DeploymentProviderUri", (object) newState.DeploymentProviderUri, (object) subState.DeploymentProviderUri),
        new SubscriptionStateVariable("MinimumRequiredVersion", (object) newState.MinimumRequiredVersion, (object) subState.MinimumRequiredVersion),
        new SubscriptionStateVariable("LastCheckTime", (object) newState.LastCheckTime, (object) subState.LastCheckTime),
        new SubscriptionStateVariable("UpdateSkippedDeployment", (object) newState.UpdateSkippedDeployment, (object) subState.UpdateSkippedDeployment),
        new SubscriptionStateVariable("UpdateSkipTime", (object) newState.UpdateSkipTime, (object) subState.UpdateSkipTime),
        new SubscriptionStateVariable("AppType", (object) (ushort) newState.appType, (object) (ushort) subState.appType),
        new SubscriptionStateVariable("CurrentBind", (object) newState.CurrentBind, (object) subState.CurrentBind)
      };
      ArrayList arrayList = new ArrayList();
      foreach (SubscriptionStateVariable subscriptionStateVariable in subscriptionStateVariableArray)
      {
        if (!subState.IsInstalled || !subscriptionStateVariable.IsUnchanged || !newState.IsInstalled)
          arrayList.Add((object) new StoreOperationMetadataProperty(Constants.DeploymentPropertySet, subscriptionStateVariable.PropertyName, newState.IsInstalled ? ComponentStore.ToPropertyString(subscriptionStateVariable.NewValue) : (string) null));
      }
      if (arrayList.Count <= 0)
        return;
      StoreOperationMetadataProperty[] array = (StoreOperationMetadataProperty[]) arrayList.ToArray(typeof (StoreOperationMetadataProperty));
      DefinitionAppId definitionAppId = new DefinitionAppId(new DefinitionIdentity[1]
      {
        subState.SubscriptionId
      });
      storeTxn.Add(new StoreOperationSetDeploymentMetadata(definitionAppId.ComPointer, this.InstallReference, array));
    }

    private void PrepareRemoveOrphanedDeployments(ComponentStore.StoreTransactionContext storeTxn, SubscriptionState subState, SubscriptionStateInternal newState)
    {
      ArrayList arrayList = new ArrayList();
      arrayList.Add((object) subState.CurrentBind);
      arrayList.Add((object) subState.PreviousBind);
      arrayList.Add((object) subState.PendingBind);
      arrayList.Remove((object) newState.CurrentBind);
      arrayList.Remove((object) newState.PreviousBind);
      arrayList.Remove((object) newState.PendingBind);
      foreach (DefinitionAppId appId in arrayList)
      {
        if (appId != null)
          this.PrepareRemoveDeployment(storeTxn, subState, appId);
      }
    }

    private void PrepareRemoveDeployment(ComponentStore.StoreTransactionContext storeTxn, SubscriptionState subState, DefinitionAppId appId)
    {
      DefinitionAppId deploymentAppId = appId.ToDeploymentAppId();
      if (subState.IsShellVisible)
        this.PrepareInstallUninstallDeployment(storeTxn, deploymentAppId, false);
      else
        this.PreparePinUnpinDeployment(storeTxn, deploymentAppId, false);
      this.PrepareSetDeploymentProperties(storeTxn, appId, (CommitApplicationParams) null);
      storeTxn.ScavengeContext.AddDeploymentToUnpin(deploymentAppId, subState);
      ApplicationTrust.RemoveCachedTrust(appId);
    }

    private void PrepareFinalizeSubscription(ComponentStore.StoreTransactionContext storeTxn, SubscriptionState subState, SubscriptionStateInternal newState)
    {
      if (!newState.IsInstalled || subState.IsInstalled && newState.IsShellVisible == subState.IsShellVisible && newState.CurrentBind.Equals((object) subState.CurrentBind))
        return;
      DefinitionAppId deploymentAppId = newState.CurrentBind.ToDeploymentAppId();
      if (newState.IsShellVisible)
        this.PrepareInstallUninstallDeployment(storeTxn, deploymentAppId, true);
      else
        this.PreparePinUnpinDeployment(storeTxn, deploymentAppId, true);
    }

    private void PreparePinUnpinDeployment(ComponentStore.StoreTransactionContext storeTxn, DefinitionAppId deployAppId, bool isPin)
    {
      if (isPin)
        storeTxn.Add(new StoreOperationPinDeployment(deployAppId.ComPointer, this.InstallReference));
      else
        storeTxn.Add(new StoreOperationUnpinDeployment(deployAppId.ComPointer, this.InstallReference));
    }

    private void PrepareInstallUninstallDeployment(ComponentStore.StoreTransactionContext storeTxn, DefinitionAppId deployAppId, bool isInstall)
    {
      if (isInstall)
        storeTxn.Add(new StoreOperationInstallDeployment(deployAppId.ComPointer, this.InstallReference));
      else
        storeTxn.Add(new StoreOperationUninstallDeployment(deployAppId.ComPointer, this.InstallReference));
    }

    private void SubmitStoreTransaction(ComponentStore.StoreTransactionContext storeTxn, SubscriptionState subState)
    {
      CodeMarker_Singleton.Instance.CodeMarker(8080);
      storeTxn.Add(new StoreOperationScavenge(false));
      StoreTransactionOperation[] operations = storeTxn.Operations;
      if (operations.Length == 0)
        return;
      uint[] rgDispositions = new uint[operations.Length];
      int[] rgResults = new int[operations.Length];
      try
      {
        this._store.Transact(operations, rgDispositions, rgResults);
        uint Disposition;
        this._stateMgr.Scavenge(0U, out Disposition);
      }
      catch (DirectoryNotFoundException ex)
      {
        throw new DeploymentException(ExceptionTypes.ComponentStore, Resources.GetString("Ex_TransactDirectoryNotFoundException"), (Exception) ex);
      }
      catch (ArgumentException ex)
      {
        throw new DeploymentException(ExceptionTypes.ComponentStore, Resources.GetString("Ex_StoreOperationFailed"), (Exception) ex);
      }
      catch (UnauthorizedAccessException ex)
      {
        throw new DeploymentException(ExceptionTypes.ComponentStore, Resources.GetString("Ex_StoreOperationFailed"), (Exception) ex);
      }
      catch (IOException ex)
      {
        throw new DeploymentException(ExceptionTypes.ComponentStore, Resources.GetString("Ex_StoreOperationFailed"), (Exception) ex);
      }
      finally
      {
        CodeMarker_Singleton.Instance.CodeMarker(8081);
        Logger.AddTransactionInformation(operations, rgDispositions, rgResults);
      }
      if (subState == null)
        return;
      subState.Invalidate();
    }

    private void SubmitStoreTransactionCheckQuota(ComponentStore.StoreTransactionContext storeTxn, SubscriptionState subState)
    {
      storeTxn.ScavengeContext.CalculateSizesPreTransact();
      this.SubmitStoreTransaction(storeTxn, subState);
      storeTxn.ScavengeContext.CalculateSizesPostTransact();
      storeTxn.ScavengeContext.CheckQuotaAndScavenge();
    }

    private static string ToPropertyString(object propValue)
    {
      return propValue != null ? (!(propValue is bool) ? (!(propValue is DateTime) ? ((object) (propValue as Uri) == null ? propValue.ToString() : ((Uri) propValue).AbsoluteUri) : ((DateTime) propValue).ToString("yyyy/MM/dd HH:mm:ss", (IFormatProvider) DateTimeFormatInfo.InvariantInfo)) : ((bool) propValue).ToString((IFormatProvider) CultureInfo.InvariantCulture)) : string.Empty;
    }

    internal class CrossGroupApplicationData
    {
      public SubscriptionState SubState;
      public ComponentStore.CrossGroupApplicationData.GroupType CrossGroupType;

      public CrossGroupApplicationData(SubscriptionState subState, ComponentStore.CrossGroupApplicationData.GroupType groupType)
      {
        this.SubState = subState;
        this.CrossGroupType = groupType;
      }

      public enum GroupType
      {
        UndefinedGroup,
        LocationGroup,
        IdentityGroup,
      }
    }

    private enum HostType
    {
      Default,
      AppLaunch,
      CorFlag,
    }

    private class StoreTransactionContext : StoreTransaction
    {
      private object _scavengeContext;
      private ComponentStore _compStore;

      public ComponentStore.ScavengeContext ScavengeContext
      {
        get
        {
          if (this._scavengeContext == null)
            Interlocked.CompareExchange(ref this._scavengeContext, (object) new ComponentStore.ScavengeContext(this._compStore), (object) null);
          return (ComponentStore.ScavengeContext) this._scavengeContext;
        }
      }

      public StoreTransactionContext(ComponentStore compStore)
      {
        this._compStore = compStore;
      }
    }

    private class ScavengeContext
    {
      private ComponentStore _compStore;
      private ArrayList _onlineDeploysToPin;
      private ArrayList _onlineDeploysToPinAlreadyPinned;
      private ArrayList _shellVisbleDeploysToUnpin;
      private ArrayList _addinDeploysToUnpin;
      private ArrayList _onlineDeploysToUnpin;
      private ulong _onlineToPinPrivateSizePreTransact;
      private ulong _onlineToPinPrivateSizePostTransact;
      private ulong _shellVisibleToUnpinSharedSize;
      private ulong _onlineToUnpinPrivateSize;
      private ulong _addinToUnpinSharedSize;

      public ScavengeContext(ComponentStore compStore)
      {
        this._compStore = compStore;
      }

      public void CheckQuotaAndScavenge()
      {
        ulong onlineAppQuotaInBytes = this._compStore.GetOnlineAppQuotaInBytes();
        ulong quotaUsageEstimate = this.GetOnlineAppQuotaUsageEstimate();
        long num = (long) this._onlineToPinPrivateSizePostTransact - (long) this._onlineToPinPrivateSizePreTransact - (long) this._onlineToUnpinPrivateSize + (long) this._shellVisibleToUnpinSharedSize + (long) this._addinToUnpinSharedSize;
        ulong usage;
        if (num >= 0L)
        {
          usage = quotaUsageEstimate + (ulong) num;
          if (usage < quotaUsageEstimate)
            usage = ulong.MaxValue;
        }
        else
        {
          usage = quotaUsageEstimate - (ulong) -num;
          if (usage > quotaUsageEstimate)
            usage = ulong.MaxValue;
        }
        if (usage > onlineAppQuotaInBytes)
        {
          IDefinitionAppId[] deployAppIdPtrs;
          ComponentStore.ScavengeContext.SubInstance[] subs = this.CollectOnlineAppsMRU(out deployAppIdPtrs);
          ulong privateSize = 0;
          ulong sharedSize = 0;
          if (deployAppIdPtrs.Length != 0)
          {
            this._compStore.CalculateDeploymentsUnderQuota(deployAppIdPtrs.Length, deployAppIdPtrs, ulong.MaxValue, ref privateSize, ref sharedSize);
            if (privateSize > onlineAppQuotaInBytes)
            {
              ulong quotaSize = onlineAppQuotaInBytes / 2UL;
              int deploymentsUnderQuota = this._compStore.CalculateDeploymentsUnderQuota(deployAppIdPtrs.Length, deployAppIdPtrs, quotaSize, ref privateSize, ref sharedSize);
              bool appExcluded;
              this.ScavengeAppsOverQuota(subs, deployAppIdPtrs.Length - deploymentsUnderQuota, out appExcluded);
              if (appExcluded)
              {
                this.CollectOnlineApps(out deployAppIdPtrs);
                this._compStore.CalculateDeploymentsUnderQuota(deployAppIdPtrs.Length, deployAppIdPtrs, ulong.MaxValue, ref privateSize, ref sharedSize);
              }
            }
          }
          usage = privateSize;
        }
        ComponentStore.ScavengeContext.PersistOnlineAppQuotaUsageEstimate(usage);
      }

      public void AddOnlineAppToCommit(DefinitionAppId appId, SubscriptionState subState)
      {
        DefinitionAppId deploymentAppId = appId.ToDeploymentAppId();
        ComponentStore.ScavengeContext.AddDeploymentToList(ref this._onlineDeploysToPin, deploymentAppId);
        if (!appId.Equals((object) subState.CurrentBind) && !appId.Equals((object) subState.PreviousBind))
          return;
        ComponentStore.ScavengeContext.AddDeploymentToList(ref this._onlineDeploysToPinAlreadyPinned, deploymentAppId);
      }

      public void AddDeploymentToUnpin(DefinitionAppId deployAppId, SubscriptionState subState)
      {
        if (subState.IsShellVisible)
          ComponentStore.ScavengeContext.AddDeploymentToList(ref this._shellVisbleDeploysToUnpin, deployAppId);
        else if (subState.appType == AppType.CustomHostSpecified)
          ComponentStore.ScavengeContext.AddDeploymentToList(ref this._addinDeploysToUnpin, deployAppId);
        else
          ComponentStore.ScavengeContext.AddDeploymentToList(ref this._onlineDeploysToUnpin, deployAppId);
      }

      public void CalculateSizesPreTransact()
      {
        this._onlineToPinPrivateSizePreTransact = this._compStore.GetPrivateSize(this._onlineDeploysToPinAlreadyPinned);
        this._onlineToUnpinPrivateSize = this._compStore.GetPrivateSize(this._onlineDeploysToUnpin);
        this._shellVisibleToUnpinSharedSize = this._compStore.GetSharedSize(this._shellVisbleDeploysToUnpin);
        this._addinToUnpinSharedSize = this._compStore.GetSharedSize(this._addinDeploysToUnpin);
      }

      public void CalculateSizesPostTransact()
      {
        this._onlineToPinPrivateSizePostTransact = this._compStore.GetPrivateSize(this._onlineDeploysToPin);
      }

      public void CleanOnlineAppCache()
      {
        IDefinitionAppId[] deployAppIdPtrs;
        ComponentStore.ScavengeContext.SubInstance[] subInstanceArray = this.CollectOnlineApps(out deployAppIdPtrs);
        using (ComponentStore.StoreTransactionContext storeTxn = new ComponentStore.StoreTransactionContext(this._compStore))
        {
          foreach (ComponentStore.ScavengeContext.SubInstance subInstance in subInstanceArray)
            this._compStore.PrepareFinalizeSubscriptionState(storeTxn, subInstance.SubState, new SubscriptionStateInternal(subInstance.SubState)
            {
              IsInstalled = false
            });
          this._compStore.SubmitStoreTransaction(storeTxn, (SubscriptionState) null);
        }
        this.CollectOnlineApps(out deployAppIdPtrs);
        ulong privateSize = 0;
        ulong sharedSize = 0;
        if (deployAppIdPtrs.Length != 0)
          this._compStore.CalculateDeploymentsUnderQuota(deployAppIdPtrs.Length, deployAppIdPtrs, ulong.MaxValue, ref privateSize, ref sharedSize);
        ComponentStore.ScavengeContext.PersistOnlineAppQuotaUsageEstimate(privateSize);
      }

      private static void AddDeploymentToList(ref ArrayList list, DefinitionAppId deployAppId)
      {
        if (list == null)
          list = new ArrayList();
        if (list.Contains((object) deployAppId))
          return;
        list.Add((object) deployAppId);
      }

      private ComponentStore.ScavengeContext.SubInstance[] CollectOnlineApps(out IDefinitionAppId[] deployAppIdPtrs)
      {
        Hashtable hashtable = new Hashtable();
        foreach (STORE_ASSEMBLY enumAssembly in this._compStore._store.EnumAssemblies(Store.EnumAssembliesFlags.Nothing))
        {
          DefinitionIdentity subscriptionId = new DefinitionIdentity(enumAssembly.DefinitionIdentity).ToSubscriptionId();
          SubscriptionState subscriptionState = this._compStore._subStore.GetSubscriptionState(subscriptionId);
          if (subscriptionState.IsInstalled && !subscriptionState.IsShellVisible && (subscriptionState.appType != AppType.CustomHostSpecified && !hashtable.Contains((object) subscriptionId)))
            hashtable.Add((object) subscriptionId, (object) new ComponentStore.ScavengeContext.SubInstance()
            {
              SubState = subscriptionState,
              LastAccessTime = subscriptionState.LastCheckTime
            });
        }
        ComponentStore.ScavengeContext.SubInstance[] subInstanceArray = new ComponentStore.ScavengeContext.SubInstance[hashtable.Count];
        hashtable.Values.CopyTo((Array) subInstanceArray, 0);
        ArrayList arrayList = new ArrayList();
        for (int index = 0; index < subInstanceArray.Length; ++index)
        {
          if (subInstanceArray[index].SubState.CurrentBind != null)
            arrayList.Add((object) subInstanceArray[index].SubState.CurrentBind.ToDeploymentAppId().ComPointer);
          if (subInstanceArray[index].SubState.PreviousBind != null)
            arrayList.Add((object) subInstanceArray[index].SubState.PreviousBind.ToDeploymentAppId().ComPointer);
        }
        deployAppIdPtrs = (IDefinitionAppId[]) arrayList.ToArray(typeof (IDefinitionAppId));
        return subInstanceArray;
      }

      private ComponentStore.ScavengeContext.SubInstance[] CollectOnlineAppsMRU(out IDefinitionAppId[] deployAppIdPtrs)
      {
        Hashtable hashtable = new Hashtable();
        foreach (STORE_ASSEMBLY enumAssembly in this._compStore._store.EnumAssemblies(Store.EnumAssembliesFlags.Nothing))
        {
          DefinitionIdentity subscriptionId = new DefinitionIdentity(enumAssembly.DefinitionIdentity).ToSubscriptionId();
          SubscriptionState subscriptionState = this._compStore._subStore.GetSubscriptionState(subscriptionId);
          if (subscriptionState.IsInstalled && !subscriptionState.IsShellVisible && (subscriptionState.appType != AppType.CustomHostSpecified && !hashtable.Contains((object) subscriptionId)))
            hashtable.Add((object) subscriptionId, (object) new ComponentStore.ScavengeContext.SubInstance()
            {
              SubState = subscriptionState,
              LastAccessTime = subscriptionState.LastCheckTime
            });
        }
        ComponentStore.ScavengeContext.SubInstance[] array = new ComponentStore.ScavengeContext.SubInstance[hashtable.Count];
        hashtable.Values.CopyTo((Array) array, 0);
        Array.Sort<ComponentStore.ScavengeContext.SubInstance>(array);
        ArrayList arrayList = new ArrayList();
        for (int index = 0; index < array.Length; ++index)
        {
          if (array[index].SubState.CurrentBind != null)
            arrayList.Add((object) array[index].SubState.CurrentBind.ToDeploymentAppId().ComPointer);
          if (array[index].SubState.PreviousBind != null)
            arrayList.Add((object) array[index].SubState.PreviousBind.ToDeploymentAppId().ComPointer);
        }
        deployAppIdPtrs = (IDefinitionAppId[]) arrayList.ToArray(typeof (IDefinitionAppId));
        return array;
      }

      private void ScavengeAppsOverQuota(ComponentStore.ScavengeContext.SubInstance[] subs, int deploysToScavenge, out bool appExcluded)
      {
        appExcluded = false;
        DateTime dateTime = DateTime.UtcNow - Constants.OnlineAppScavengingGracePeriod;
        using (ComponentStore.StoreTransactionContext storeTxn = new ComponentStore.StoreTransactionContext(this._compStore))
        {
          for (int index = subs.Length - 1; index >= 0 && deploysToScavenge > 0; --index)
          {
            bool flag1 = false;
            bool flag2 = false;
            if (subs[index].SubState.PreviousBind != null)
            {
              if (subs[index].LastAccessTime >= dateTime)
                appExcluded = true;
              else
                flag1 = true;
              --deploysToScavenge;
            }
            if (deploysToScavenge > 0)
            {
              if (subs[index].LastAccessTime >= dateTime)
                appExcluded = true;
              else
                flag2 = true;
              --deploysToScavenge;
            }
            if (flag2 | flag1)
            {
              SubscriptionStateInternal newState = new SubscriptionStateInternal(subs[index].SubState);
              if (flag2)
                newState.IsInstalled = false;
              else
                newState.PreviousBind = (DefinitionAppId) null;
              this._compStore.PrepareFinalizeSubscriptionState(storeTxn, subs[index].SubState, newState);
            }
          }
          this._compStore.SubmitStoreTransaction(storeTxn, (SubscriptionState) null);
        }
      }

      private ulong GetOnlineAppQuotaUsageEstimate()
      {
        ulong num1 = ulong.MaxValue;
        using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Classes\\Software\\Microsoft\\Windows\\CurrentVersion\\Deployment"))
        {
          if (registryKey != null)
          {
            object obj = registryKey.GetValue("OnlineAppQuotaUsageEstimate");
            if (obj is long)
            {
              long num2 = (long) obj;
              num1 = num2 >= 0L ? (ulong) num2 : (ulong) (-1L - -num2 + 1L);
            }
          }
          return num1;
        }
      }

      private static void PersistOnlineAppQuotaUsageEstimate(ulong usage)
      {
        using (RegistryKey subKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\Software\\Microsoft\\Windows\\CurrentVersion\\Deployment"))
        {
          if (subKey == null)
            return;
          subKey.SetValue("OnlineAppQuotaUsageEstimate", (object) usage, RegistryValueKind.QWord);
        }
      }

      private class SubInstance : IComparable
      {
        public SubscriptionState SubState;
        public DateTime LastAccessTime;

        public int CompareTo(object other)
        {
          return ((ComponentStore.ScavengeContext.SubInstance) other).LastAccessTime.CompareTo(this.LastAccessTime);
        }
      }
    }
  }
}
