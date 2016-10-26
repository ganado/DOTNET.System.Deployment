// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.DeploymentManager
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Deployment.Application.Manifest;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading;

namespace System.Deployment.Application
{
  internal class DeploymentManager : IDisposable, IDownloadNotification
  {
    private static readonly object bindCompletedKey = new object();
    private static readonly object synchronizeCompletedKey = new object();
    private static readonly object progressChangedKey = new object();
    private ManualResetEvent _trustNotGrantedEvent = new ManualResetEvent(false);
    private ManualResetEvent _trustGrantedEvent = new ManualResetEvent(false);
    private ManualResetEvent _platformRequirementsFailedEvent = new ManualResetEvent(false);
    private bool _isConfirmed = true;
    private DeploymentProgressState _state = DeploymentProgressState.DownloadingApplicationFiles;
    private readonly ThreadStart bindWorker;
    private readonly ThreadStart synchronizeWorker;
    private readonly WaitCallback synchronizeGroupWorker;
    private readonly SendOrPostCallback bindCompleted;
    private readonly SendOrPostCallback synchronizeCompleted;
    private readonly SendOrPostCallback progressReporter;
    private readonly AsyncOperation asyncOperation;
    private int _bindGuard;
    private int _syncGuard;
    private bool _cancellationPending;
    private bool _cached;
    private ManualResetEvent[] _assertApplicationReqEvents;
    private DeploymentManager.CallerType _callerType;
    private Uri _deploySource;
    private DefinitionAppId _bindAppId;
    private SubscriptionStore _subStore;
    private bool _isupdate;
    private DownloadOptions _downloadOptions;
    private EventHandlerList _events;
    private Hashtable _syncGroupMap;
    private ActivationDescription _actDesc;
    private ActivationContext _actCtx;
    private TempFile _tempDeployment;
    private TempDirectory _tempApplicationDirectory;
    private FileStream _referenceTransaction;
    private Logger.LogIdentity _log;
    private long _downloadedAppSize;

    public DeploymentManager.CallerType Callertype
    {
      get
      {
        return this._callerType;
      }
      set
      {
        this._callerType = value;
      }
    }

    public Logger.LogIdentity LogId
    {
      get
      {
        return this._log;
      }
    }

    public bool CancellationPending
    {
      get
      {
        return this._cancellationPending;
      }
    }

    public string ShortcutAppId
    {
      get
      {
        SubscriptionState subscriptionState = this._subStore.GetSubscriptionState(this._actDesc.DeployManifest);
        string str = (string) null;
        if (subscriptionState.IsInstalled)
          str = string.Format("{0}#{1}", (object) subscriptionState.DeploymentProviderUri.AbsoluteUri, (object) subscriptionState.SubscriptionId.ToString());
        return str;
      }
    }

    public string LogFilePath
    {
      get
      {
        string str = Logger.GetLogFilePath(this._log);
        if (!Logger.FlushLog(this._log))
          str = (string) null;
        return str;
      }
    }

    internal ActivationDescription ActivationDescription
    {
      get
      {
        return this._actDesc;
      }
    }

    private EventHandlerList Events
    {
      get
      {
        return this._events;
      }
    }

    public event BindCompletedEventHandler BindCompleted
    {
      add
      {
        this.Events.AddHandler(DeploymentManager.bindCompletedKey, (Delegate) value);
      }
      remove
      {
        this.Events.RemoveHandler(DeploymentManager.bindCompletedKey, (Delegate) value);
      }
    }

    public event SynchronizeCompletedEventHandler SynchronizeCompleted
    {
      add
      {
        this.Events.AddHandler(DeploymentManager.synchronizeCompletedKey, (Delegate) value);
      }
      remove
      {
        this.Events.RemoveHandler(DeploymentManager.synchronizeCompletedKey, (Delegate) value);
      }
    }

    public event DeploymentProgressChangedEventHandler ProgressChanged
    {
      add
      {
        this.Events.AddHandler(DeploymentManager.progressChangedKey, (Delegate) value);
      }
      remove
      {
        this.Events.RemoveHandler(DeploymentManager.progressChangedKey, (Delegate) value);
      }
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public DeploymentManager(string appId)
      : this(appId, false, true, (DownloadOptions) null, (AsyncOperation) null)
    {
      if (appId == null)
        throw new ArgumentNullException("appId");
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public DeploymentManager(Uri deploymentSource)
      : this(deploymentSource, false, true, (DownloadOptions) null, (AsyncOperation) null)
    {
      if (deploymentSource == (Uri) null)
        throw new ArgumentNullException("deploymentSource");
      UriHelper.ValidateSupportedSchemeInArgument(deploymentSource, "deploymentSource");
    }

    internal DeploymentManager(Uri deploymentSource, bool isUpdate, bool isConfirmed, DownloadOptions downloadOptions, AsyncOperation optionalAsyncOp)
    {
      this._deploySource = deploymentSource;
      this._isupdate = isUpdate;
      this._isConfirmed = isConfirmed;
      this._downloadOptions = downloadOptions;
      this._events = new EventHandlerList();
      this._syncGroupMap = CollectionsUtil.CreateCaseInsensitiveHashtable();
      this._subStore = SubscriptionStore.CurrentUser;
      this.bindWorker = new ThreadStart(this.BindAsyncWorker);
      this.synchronizeWorker = new ThreadStart(this.SynchronizeAsyncWorker);
      this.synchronizeGroupWorker = new WaitCallback(this.SynchronizeGroupAsyncWorker);
      this.bindCompleted = new SendOrPostCallback(this.BindAsyncCompleted);
      this.synchronizeCompleted = new SendOrPostCallback(this.SynchronizeAsyncCompleted);
      this.progressReporter = new SendOrPostCallback(this.ProgressReporter);
      this.asyncOperation = optionalAsyncOp != null ? optionalAsyncOp : AsyncOperationManager.CreateOperation((object) null);
      this._log = Logger.StartLogging();
      if (deploymentSource != (Uri) null)
        Logger.SetSubscriptionUrl(this._log, deploymentSource);
      this._assertApplicationReqEvents = new ManualResetEvent[3];
      this._assertApplicationReqEvents[0] = this._trustNotGrantedEvent;
      this._assertApplicationReqEvents[1] = this._platformRequirementsFailedEvent;
      this._assertApplicationReqEvents[2] = this._trustGrantedEvent;
      this._callerType = DeploymentManager.CallerType.Other;
      PolicyKeys.SkipApplicationDependencyHashCheck();
      PolicyKeys.SkipDeploymentProvider();
      PolicyKeys.SkipSchemaValidation();
      PolicyKeys.SkipSemanticValidation();
      PolicyKeys.SkipSignatureValidation();
    }

    internal DeploymentManager(string appId, bool isUpdate, bool isConfirmed, DownloadOptions downloadOptions, AsyncOperation optionalAsyncOp)
      : this((Uri) null, isUpdate, isConfirmed, downloadOptions, optionalAsyncOp)
    {
      this._bindAppId = new DefinitionAppId(appId);
    }

    public void BindAsync()
    {
      Logger.AddMethodCall(this._log, "DeploymentManager.BindAsync() called.");
      if (this._cancellationPending)
        return;
      if (Interlocked.Exchange(ref this._bindGuard, 1) != 0)
        throw new InvalidOperationException(Resources.GetString("Ex_BindOnce"));
      this.bindWorker.BeginInvoke((AsyncCallback) null, (object) null);
    }

    public ActivationContext Bind()
    {
      if (Interlocked.Exchange(ref this._bindGuard, 1) != 0)
        throw new InvalidOperationException(Resources.GetString("Ex_BindOnce"));
      bool flag = false;
      TempFile tempDeploy = (TempFile) null;
      TempDirectory tempAppDir = (TempDirectory) null;
      FileStream refTransaction = (FileStream) null;
      try
      {
        string productName = (string) null;
        this.BindCore(true, ref tempDeploy, ref tempAppDir, ref refTransaction, ref productName);
      }
      catch (Exception ex)
      {
        flag = true;
        throw;
      }
      finally
      {
        this._state = DeploymentProgressState.DownloadingApplicationFiles;
        if (flag)
        {
          if (tempAppDir != null)
            tempAppDir.Dispose();
          if (tempDeploy != null)
            tempDeploy.Dispose();
          if (refTransaction != null)
            refTransaction.Close();
        }
      }
      return this._actCtx;
    }

    public void DeterminePlatformRequirements()
    {
      try
      {
        if (this._actDesc == null)
          throw new InvalidOperationException(Resources.GetString("Ex_BindFirst"));
        this.DeterminePlatformRequirementsCore(true);
      }
      catch (Exception ex)
      {
        this._platformRequirementsFailedEvent.Set();
        throw;
      }
    }

    public void DetermineTrust(TrustParams trustParams)
    {
      try
      {
        if (this._actDesc == null)
          throw new InvalidOperationException(Resources.GetString("Ex_BindFirst"));
        this.DetermineTrustCore(true, trustParams);
      }
      catch (Exception ex)
      {
        this._trustNotGrantedEvent.Set();
        throw;
      }
      this._trustGrantedEvent.Set();
    }

    public void SynchronizeAsync()
    {
      Logger.AddMethodCall(this._log, "DeploymentManager.SynchronizeAsync() called.");
      if (this._cancellationPending)
        return;
      if (this._actDesc == null)
        throw new InvalidOperationException(Resources.GetString("Ex_BindFirst"));
      if (Interlocked.Exchange(ref this._syncGuard, 1) != 0)
        throw new InvalidOperationException(Resources.GetString("Ex_SyncNullOnce"));
      this.synchronizeWorker.BeginInvoke((AsyncCallback) null, (object) null);
    }

    public void Synchronize()
    {
      if (this._actDesc == null)
        throw new InvalidOperationException(Resources.GetString("Ex_BindFirst"));
      if (Interlocked.Exchange(ref this._syncGuard, 1) != 0)
        throw new InvalidOperationException(Resources.GetString("Ex_SyncNullOnce"));
      this.SynchronizeCore(true);
    }

    public void SynchronizeAsync(string groupName)
    {
      this.SynchronizeAsync(groupName, (object) null);
    }

    public void SynchronizeAsync(string groupName, object userState)
    {
      if (groupName == null)
      {
        this.SynchronizeAsync();
      }
      else
      {
        if (this._actDesc == null)
          throw new InvalidOperationException(Resources.GetString("Ex_BindFirst"));
        if (!this._cached)
          throw new InvalidOperationException(Resources.GetString("Ex_SyncNullFirst"));
        bool created;
        SyncGroupHelper group = this.AttachToGroup(groupName, userState, out created);
        if (created)
          ThreadPool.QueueUserWorkItem(this.synchronizeGroupWorker, (object) group);
        else
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_SyncGroupOnce"), new object[1]
          {
            (object) groupName
          }));
      }
    }

    public void Synchronize(string groupName)
    {
      if (groupName == null)
      {
        this.Synchronize();
      }
      else
      {
        if (this._actDesc == null)
          throw new InvalidOperationException(Resources.GetString("Ex_BindFirst"));
        if (!this._cached)
          throw new InvalidOperationException(Resources.GetString("Ex_SyncNullFirst"));
        bool created;
        SyncGroupHelper group = this.AttachToGroup(groupName, (object) null, out created);
        if (created)
          this.SynchronizeGroupCore(true, group);
        else
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_SyncGroupOnce"), new object[1]
          {
            (object) groupName
          }));
      }
    }

    public ObjectHandle ExecuteNewDomain()
    {
      if (this._actDesc == null)
        throw new InvalidOperationException(Resources.GetString("Ex_BindFirst"));
      if (!this._cached)
        throw new InvalidOperationException(Resources.GetString("Ex_SyncNullFirst"));
      Logger.AddInternalState(this._log, "Activating " + (this._actCtx == null || this._actCtx.Identity == null ? "null" : this._actCtx.Identity.ToString()) + " in a new domain.");
      return Activator.CreateInstance(this._actCtx);
    }

    public void ExecuteNewProcess()
    {
      if (this._actDesc == null)
        throw new InvalidOperationException(Resources.GetString("Ex_BindFirst"));
      if (!this._cached)
        throw new InvalidOperationException(Resources.GetString("Ex_SyncNullFirst"));
      this._subStore.ActivateApplication(this._actDesc.AppId, (string) null, false);
    }

    public void CancelAsync()
    {
      this._cancellationPending = true;
    }

    public void CancelAsync(string groupName)
    {
      if (groupName == null)
      {
        this.CancelAsync();
      }
      else
      {
        lock (this._syncGroupMap.SyncRoot)
        {
          SyncGroupHelper local_2 = (SyncGroupHelper) this._syncGroupMap[(object) groupName];
          if (local_2 == null)
            return;
          local_2.CancelAsync();
        }
      }
    }

    public void Dispose()
    {
      this._events.Dispose();
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    void IDownloadNotification.DownloadModified(object sender, DownloadEventArgs e)
    {
      if (this._cancellationPending)
        ((FileDownloader) sender).Cancel();
      this.asyncOperation.Post(this.progressReporter, (object) new DeploymentProgressChangedEventArgs(e.Progress, (object) null, e.BytesCompleted, e.BytesTotal, this._state, (string) null));
    }

    void IDownloadNotification.DownloadCompleted(object sender, DownloadEventArgs e)
    {
      this._downloadedAppSize = e.BytesCompleted;
    }

    private void BindAsyncCompleted(object arg)
    {
      BindCompletedEventArgs e = (BindCompletedEventArgs) arg;
      BindCompletedEventHandler completedEventHandler = (BindCompletedEventHandler) this.Events[DeploymentManager.bindCompletedKey];
      if (completedEventHandler == null)
        return;
      completedEventHandler((object) this, e);
    }

    private void SynchronizeAsyncCompleted(object arg)
    {
      SynchronizeCompletedEventArgs e = (SynchronizeCompletedEventArgs) arg;
      SynchronizeCompletedEventHandler completedEventHandler = (SynchronizeCompletedEventHandler) this.Events[DeploymentManager.synchronizeCompletedKey];
      if (completedEventHandler == null)
        return;
      completedEventHandler((object) this, e);
    }

    private void ProgressReporter(object arg)
    {
      DeploymentProgressChangedEventArgs e = (DeploymentProgressChangedEventArgs) arg;
      DeploymentProgressChangedEventHandler changedEventHandler = (DeploymentProgressChangedEventHandler) this.Events[DeploymentManager.progressChangedKey];
      if (changedEventHandler == null)
        return;
      changedEventHandler((object) this, e);
    }

    private void BindAsyncWorker()
    {
      Exception error = (Exception) null;
      bool cancelled = false;
      string productName = (string) null;
      TempFile tempDeploy = (TempFile) null;
      TempDirectory tempAppDir = (TempDirectory) null;
      FileStream refTransaction = (FileStream) null;
      try
      {
        Logger.AddInternalState(this._log, "Binding started in a worker thread.");
        cancelled = this.BindCore(false, ref tempDeploy, ref tempAppDir, ref refTransaction, ref productName);
        Logger.AddInternalState(this._log, "Binding is successful.");
      }
      catch (Exception ex)
      {
        if (ExceptionUtility.IsHardException(ex))
          throw;
        else if (ex is DownloadCancelledException)
          cancelled = true;
        else
          error = ex;
      }
      finally
      {
        this._state = DeploymentProgressState.DownloadingApplicationFiles;
        if (error != null | cancelled)
        {
          if (tempAppDir != null)
            tempAppDir.Dispose();
          if (tempDeploy != null)
            tempDeploy.Dispose();
          if (refTransaction != null)
            refTransaction.Close();
        }
        this.asyncOperation.Post(this.bindCompleted, (object) new BindCompletedEventArgs(error, cancelled, (object) null, this._actCtx, productName, this._cached));
      }
    }

    private bool BindCore(bool blocking, ref TempFile tempDeploy, ref TempDirectory tempAppDir, ref FileStream refTransaction, ref string productName)
    {
      try
      {
        if (this._deploySource == (Uri) null)
          return this.BindCoreWithAppId(blocking, ref refTransaction, ref productName);
        Uri deploySource = this._deploySource;
        this._state = DeploymentProgressState.DownloadingDeploymentInformation;
        Logger.AddInternalState(this._log, "Internal state=" + (object) this._state);
        AssemblyManifest assemblyManifest1 = DownloadManager.DownloadDeploymentManifest(this._subStore, ref deploySource, out tempDeploy, blocking ? (IDownloadNotification) null : (IDownloadNotification) this, this._downloadOptions);
        string path = tempDeploy.Path;
        ActivationDescription actDesc = new ActivationDescription();
        actDesc.SetDeploymentManifest(assemblyManifest1, deploySource, path);
        Logger.SetDeploymentManifest(this._log, assemblyManifest1);
        actDesc.IsUpdate = this._isupdate;
        if (actDesc.DeployManifest.Deployment == null)
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_NotDeploymentOrShortcut"));
        if (!blocking && this._cancellationPending)
          return true;
        long transactionId;
        refTransaction = this._subStore.AcquireReferenceTransaction(out transactionId);
        SubscriptionState subscriptionState = this._subStore.GetSubscriptionState(actDesc.DeployManifest);
        if (actDesc.DeployManifest.Deployment.Install && actDesc.DeployManifest.Deployment.ProviderCodebaseUri == (Uri) null && (subscriptionState != null && subscriptionState.DeploymentProviderUri != (Uri) null) && !subscriptionState.DeploymentProviderUri.Equals((object) deploySource))
          throw new DeploymentException(ExceptionTypes.DeploymentUriDifferent, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_DeploymentUriDifferentExText"), new object[3]
          {
            (object) actDesc.DeployManifest.Description.FilteredProduct,
            (object) deploySource.AbsoluteUri,
            (object) subscriptionState.DeploymentProviderUri.AbsoluteUri
          }));
        DefinitionAppId appId;
        try
        {
          appId = new DefinitionAppId(actDesc.ToAppCodebase(), new DefinitionIdentity[2]
          {
            actDesc.DeployManifest.Identity,
            new DefinitionIdentity(actDesc.DeployManifest.MainDependentAssembly.Identity)
          });
        }
        catch (COMException ex)
        {
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_IdentityIsNotValid"), (Exception) ex);
        }
        catch (SEHException ex)
        {
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_IdentityIsNotValid"), (Exception) ex);
        }
        Logger.AddInternalState(this._log, "expectedAppId=" + appId.ToString());
        bool flag1 = this._subStore.CheckAndReferenceApplication(subscriptionState, appId, transactionId);
        if (flag1 && appId.Equals((object) subscriptionState.CurrentBind))
        {
          Logger.AddInternalState(this._log, "Application is found in store and it is the CurrentBind. Binding with appid.");
          this._bindAppId = appId;
          return this.BindCoreWithAppId(blocking, ref refTransaction, ref productName);
        }
        if (flag1)
          Logger.AddInternalState(this._log, "Application is found in store but it is not the CurrentBind.");
        else
          Logger.AddInternalState(this._log, "Application is not found in store.");
        if (!blocking && this._cancellationPending)
          return true;
        this._state = DeploymentProgressState.DownloadingApplicationInformation;
        Logger.AddInternalState(this._log, "Internal state=" + (object) this._state);
        tempAppDir = this._subStore.AcquireTempDirectory();
        Uri appSourceUri;
        string appManifestPath;
        AssemblyManifest assemblyManifest2 = DownloadManager.DownloadApplicationManifest(actDesc.DeployManifest, tempAppDir.Path, actDesc.DeploySourceUri, blocking ? (IDownloadNotification) null : (IDownloadNotification) this, this._downloadOptions, out appSourceUri, out appManifestPath);
        AssemblyManifest.ReValidateManifestSignatures(actDesc.DeployManifest, assemblyManifest2);
        Logger.SetApplicationManifest(this._log, assemblyManifest2);
        Logger.SetApplicationUrl(this._log, appSourceUri);
        actDesc.SetApplicationManifest(assemblyManifest2, appSourceUri, appManifestPath);
        actDesc.AppId = new DefinitionAppId(actDesc.ToAppCodebase(), new DefinitionIdentity[2]
        {
          actDesc.DeployManifest.Identity,
          actDesc.AppManifest.Identity
        });
        bool flag2 = this._subStore.CheckAndReferenceApplication(subscriptionState, actDesc.AppId, transactionId);
        if (!blocking && this._cancellationPending)
          return true;
        Description effectiveDescription = actDesc.EffectiveDescription;
        productName = effectiveDescription.Product;
        this._cached = flag2;
        Logger.AddInternalState(this._log, "_cached=" + this._cached.ToString());
        Logger.AddInternalState(this._log, "_isupdate=" + this._isupdate.ToString());
        this._tempApplicationDirectory = tempAppDir;
        this._tempDeployment = tempDeploy;
        this._referenceTransaction = refTransaction;
        this._actCtx = DeploymentManager.ConstructActivationContext(actDesc);
        this._actDesc = actDesc;
      }
      catch (Exception ex)
      {
        this.LogError(Resources.GetString("Ex_FailedToDownloadManifest"), ex);
        Logger.AddInternalState(this._log, "Exception thrown in  BindCore(): " + ex.GetType().ToString() + " : " + ex.Message + "\r\n" + ex.StackTrace);
        throw;
      }
      return false;
    }

    private bool BindCoreWithAppId(bool blocking, ref FileStream refTransaction, ref string productName)
    {
      SubscriptionState subscriptionState = this._subStore.GetSubscriptionState(this._bindAppId.DeploymentIdentity.ToSubscriptionId());
      if (!subscriptionState.IsInstalled)
        throw new InvalidDeploymentException(Resources.GetString("Ex_BindAppIdNotInstalled"));
      if (!this._bindAppId.Equals((object) subscriptionState.CurrentBind))
        throw new InvalidDeploymentException(Resources.GetString("Ex_BindAppIdNotCurrrent"));
      if (!blocking && this._cancellationPending)
        return true;
      long transactionId;
      refTransaction = this._subStore.AcquireReferenceTransaction(out transactionId);
      bool flag = this._subStore.CheckAndReferenceApplication(subscriptionState, this._bindAppId, transactionId);
      ActivationDescription activationDescription = new ActivationDescription();
      activationDescription.SetDeploymentManifest(subscriptionState.CurrentDeploymentManifest, subscriptionState.CurrentDeploymentSourceUri, (string) null);
      Logger.SetDeploymentManifest(this._log, subscriptionState.CurrentDeploymentManifest);
      activationDescription.IsUpdate = this._isupdate;
      activationDescription.SetApplicationManifest(subscriptionState.CurrentApplicationManifest, subscriptionState.CurrentApplicationSourceUri, (string) null);
      Logger.SetApplicationManifest(this._log, subscriptionState.CurrentApplicationManifest);
      Logger.SetApplicationUrl(this._log, subscriptionState.CurrentApplicationSourceUri);
      activationDescription.AppId = new DefinitionAppId(activationDescription.ToAppCodebase(), new DefinitionIdentity[2]
      {
        activationDescription.DeployManifest.Identity,
        activationDescription.AppManifest.Identity
      });
      if (!blocking && this._cancellationPending)
        return true;
      Description effectiveDescription = subscriptionState.EffectiveDescription;
      productName = effectiveDescription.Product;
      this._cached = flag;
      Logger.AddInternalState(this._log, "_cached=" + this._cached.ToString());
      Logger.AddInternalState(this._log, "_isupdate=" + this._isupdate.ToString());
      this._referenceTransaction = refTransaction;
      this._actCtx = DeploymentManager.ConstructActivationContextFromStore(activationDescription.AppId);
      this._actDesc = activationDescription;
      return false;
    }

    private bool DeterminePlatformRequirementsCore(bool blocking)
    {
      try
      {
        Logger.AddMethodCall(this._log, "DeploymentManager.DeterminePlatformRequirementsCore(" + blocking.ToString() + ") called.");
        if (!blocking && this._cancellationPending)
          return true;
        using (TempDirectory tempDirectory = this._subStore.AcquireTempDirectory())
          PlatformDetector.VerifyPlatformDependencies(this._actDesc.AppManifest, this._actDesc.DeployManifest, tempDirectory.Path);
      }
      catch (Exception ex)
      {
        this.LogError(Resources.GetString("Ex_DeterminePlatformRequirementsFailed"), ex);
        Logger.AddInternalState(this._log, "Exception thrown in  DeterminePlatformRequirementsCore(): " + ex.GetType().ToString() + " : " + ex.Message + "\r\n" + ex.StackTrace);
        throw;
      }
      return false;
    }

    private bool DetermineTrustCore(bool blocking, TrustParams tp)
    {
      try
      {
        Logger.AddMethodCall(this._log, "DeploymentManager.DetermineTrustCore() called.");
        SubscriptionState subscriptionState = this._subStore.GetSubscriptionState(this._actDesc.DeployManifest);
        TrustManagerContext tmc = new TrustManagerContext();
        tmc.IgnorePersistedDecision = false;
        tmc.NoPrompt = false;
        tmc.Persist = true;
        if (tp != null)
          tmc.NoPrompt = tp.NoPrompt;
        if (!blocking && this._cancellationPending)
          return true;
        if (subscriptionState.IsInstalled && !string.Equals(subscriptionState.EffectiveCertificatePublicKeyToken, this._actDesc.EffectiveCertificatePublicKeyToken, StringComparison.Ordinal))
        {
          Logger.AddInternalState(this._log, "Application family is installed but effective certificate public key token has changed between versions: subState.EffectiveCertificatePublicKeyToken=" + subscriptionState.EffectiveCertificatePublicKeyToken + ",_actDesc.EffectiveCertificatePublicKeyToken=" + this._actDesc.EffectiveCertificatePublicKeyToken);
          Logger.AddInternalState(this._log, "Removing cached trust for the CurrentBind.");
          ApplicationTrust.RemoveCachedTrust(subscriptionState.CurrentBind);
        }
        bool isUpdate = false;
        if (this._actDesc.IsUpdate)
          isUpdate = true;
        if (this._actDesc.IsUpdateInPKTGroup)
        {
          isUpdate = false;
          this._actDesc.IsFullTrustRequested = new ApplicationSecurityInfo(this._actCtx).DefaultRequestSet.IsUnrestricted();
        }
        this._actDesc.Trust = ApplicationTrust.RequestTrust(subscriptionState, this._actDesc.DeployManifest.Deployment.Install, isUpdate, this._actCtx, tmc);
      }
      catch (Exception ex)
      {
        this.LogError(Resources.GetString("Ex_DetermineTrustFailed"), ex);
        Logger.AddInternalState(this._log, "Exception thrown in  DetermineTrustCore(): " + ex.GetType().ToString() + " : " + ex.Message + "\r\n" + ex.StackTrace);
        throw;
      }
      return false;
    }

    public void PersistTrustWithoutEvaluation()
    {
      try
      {
        this._actDesc.Trust = ApplicationTrust.PersistTrustWithoutEvaluation(this._actCtx);
      }
      catch (Exception ex)
      {
        this._trustNotGrantedEvent.Set();
        throw;
      }
      this._trustGrantedEvent.Set();
    }

    private void SynchronizeAsyncWorker()
    {
      Exception error = (Exception) null;
      bool cancelled = false;
      try
      {
        Logger.AddInternalState(this._log, "Download and install of the application started in a worker thread.");
        cancelled = this.SynchronizeCore(false);
        Logger.AddInternalState(this._log, "Installation is successful.");
      }
      catch (Exception ex)
      {
        if (ExceptionUtility.IsHardException(ex))
          throw;
        else if (ex is DownloadCancelledException)
          cancelled = true;
        else
          error = ex;
      }
      finally
      {
        this.asyncOperation.Post(this.synchronizeCompleted, (object) new SynchronizeCompletedEventArgs(error, cancelled, (object) null, (string) null));
      }
    }

    private bool SynchronizeCore(bool blocking)
    {
      try
      {
        AssemblyManifest deployManifest = this._actDesc.DeployManifest;
        SubscriptionState subscriptionState = this._subStore.GetSubscriptionState(deployManifest);
        this._subStore.CheckDeploymentSubscriptionState(subscriptionState, deployManifest);
        this._subStore.CheckCustomUXFlag(subscriptionState, this._actDesc.AppManifest);
        if (this._actDesc.DeployManifestPath != null)
        {
          this._actDesc.CommitDeploy = true;
          this._actDesc.IsConfirmed = this._isConfirmed;
          this._actDesc.TimeStamp = DateTime.UtcNow;
        }
        else
          this._actDesc.CommitDeploy = false;
        if (!blocking && this._cancellationPending)
          return true;
        if (!this._cached)
        {
          Logger.AddInternalState(this._log, "Application is not cached.");
          bool flag1 = false;
          if (this._actDesc.appType != AppType.CustomHostSpecified)
          {
            if (this._actDesc.Trust != null)
            {
              bool flag2 = this._actDesc.Trust.DefaultGrantSet.PermissionSet.IsUnrestricted();
              Logger.AddInternalState(this._log, "fullTrust=" + flag2.ToString());
              if (!flag2 && this._actDesc.AppManifest.FileAssociations.Length != 0)
                throw new DeploymentException(ExceptionTypes.ManifestSemanticValidation, Resources.GetString("Ex_FileExtensionNotSupported"));
              bool flag3 = !this._actDesc.DeployManifest.Deployment.Install;
              if (!flag2 & flag3)
              {
                Logger.AddInternalState(this._log, "Application is semi-trust and online. Size limits will be checked during download.");
                if (this._downloadOptions == null)
                  this._downloadOptions = new DownloadOptions();
                this._downloadOptions.EnforceSizeLimit = true;
                this._downloadOptions.SizeLimit = this._subStore.GetSizeLimitInBytesForSemiTrustApps();
                this._downloadOptions.Size = this._actDesc.DeployManifest.SizeInBytes + this._actDesc.AppManifest.SizeInBytes;
              }
            }
            else
              flag1 = true;
          }
          DownloadManager.DownloadDependencies(subscriptionState, this._actDesc.DeployManifest, this._actDesc.AppManifest, this._actDesc.AppSourceUri, this._tempApplicationDirectory.Path, (string) null, blocking ? (IDownloadNotification) null : (IDownloadNotification) this, this._downloadOptions);
          if (!blocking && this._cancellationPending)
            return true;
          this.WaitForAssertApplicationRequirements();
          if (flag1)
            this.CheckSizeLimit();
          this._actDesc.CommitApp = true;
          this._actDesc.AppPayloadPath = this._tempApplicationDirectory.Path;
        }
        else
        {
          Logger.AddInternalState(this._log, "Application is cached.");
          this.WaitForAssertApplicationRequirements();
        }
        if (this._actDesc.CommitDeploy || this._actDesc.CommitApp)
        {
          this._subStore.CommitApplication(ref subscriptionState, (CommitApplicationParams) this._actDesc);
          Logger.AddInternalState(this._log, "Application is successfully committed to the store.");
        }
        if (this._tempApplicationDirectory != null)
        {
          this._tempApplicationDirectory.Dispose();
          this._tempApplicationDirectory = (TempDirectory) null;
        }
        if (this._tempDeployment != null)
        {
          this._tempDeployment.Dispose();
          this._tempDeployment = (TempFile) null;
        }
        if (this._referenceTransaction != null)
        {
          this._referenceTransaction.Close();
          this._referenceTransaction = (FileStream) null;
        }
        Logger.AddInternalState(this._log, "Refreshing ActivationContext from store.");
        ActivationContext actCtx = this._actCtx;
        this._actCtx = DeploymentManager.ConstructActivationContextFromStore(this._actDesc.AppId);
        actCtx.Dispose();
        this._cached = true;
      }
      catch (Exception ex)
      {
        this.LogError(Resources.GetString("Ex_DownloadApplicationFailed"), ex);
        Logger.AddInternalState(this._log, "Exception thrown in  SynchronizeCore(): " + ex.GetType().ToString() + " : " + ex.Message + "\r\n" + ex.StackTrace);
        throw;
      }
      return false;
    }

    private void WaitForAssertApplicationRequirements()
    {
      if (this._actDesc.appType == AppType.CustomHostSpecified || this._callerType == DeploymentManager.CallerType.ApplicationDeployment)
        return;
      Logger.AddInternalState(this._log, "WaitForAssertApplicationRequirements() called.");
      switch (WaitHandle.WaitAny((WaitHandle[]) this._assertApplicationReqEvents, Constants.AssertApplicationRequirementsTimeout, false))
      {
        case 258:
          throw new DeploymentException(Resources.GetString("Ex_CannotCommitNoTrustDecision"));
        case 0:
          throw new DeploymentException(Resources.GetString("Ex_CannotCommitTrustFailed"));
        case 1:
          throw new DeploymentException(Resources.GetString("Ex_CannotCommitPlatformRequirementsFailed"));
        default:
          Logger.AddInternalState(this._log, "WaitForAssertApplicationRequirements() returned.");
          break;
      }
    }

    private void CheckSizeLimit()
    {
      if (this._actDesc.appType == AppType.CustomHostSpecified || this._actDesc.Trust == null || !(!this._actDesc.Trust.DefaultGrantSet.PermissionSet.IsUnrestricted() & !this._actDesc.DeployManifest.Deployment.Install))
        return;
      ulong forSemiTrustApps = this._subStore.GetSizeLimitInBytesForSemiTrustApps();
      if ((ulong) this._downloadedAppSize > forSemiTrustApps)
        throw new DeploymentDownloadException(ExceptionTypes.SizeLimitForPartialTrustOnlineAppExceeded, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_OnlineSemiTrustAppSizeLimitExceeded"), new object[1]
        {
          (object) forSemiTrustApps
        }));
    }

    private void SynchronizeGroupAsyncWorker(object arg)
    {
      Exception error = (Exception) null;
      bool cancelled = false;
      string groupName = (string) null;
      object userState = (object) null;
      try
      {
        SyncGroupHelper sgh = (SyncGroupHelper) arg;
        groupName = sgh.Group;
        userState = sgh.UserState;
        cancelled = this.SynchronizeGroupCore(false, sgh);
      }
      catch (Exception ex)
      {
        if (ExceptionUtility.IsHardException(ex))
          throw;
        else if (ex is DownloadCancelledException)
          cancelled = true;
        else
          error = ex;
      }
      finally
      {
        this.asyncOperation.Post(this.synchronizeCompleted, (object) new SynchronizeCompletedEventArgs(error, cancelled, userState, groupName));
      }
    }

    [PermissionSet(SecurityAction.Assert, Name = "FullTrust")]
    private bool SynchronizeGroupCore(bool blocking, SyncGroupHelper sgh)
    {
      TempDirectory tempDirectory = (TempDirectory) null;
      try
      {
        string group = sgh.Group;
        SubscriptionState subscriptionState = this._subStore.GetSubscriptionState(this._actDesc.DeployManifest);
        if (this._subStore.CheckGroupInstalled(subscriptionState, this._actDesc.AppId, this._actDesc.AppManifest, group))
          return false;
        bool flag1 = AppDomain.CurrentDomain.ApplicationTrust.DefaultGrantSet.PermissionSet.IsUnrestricted();
        if (!flag1 && this._actDesc.AppManifest.FileAssociations.Length != 0)
          throw new DeploymentException(ExceptionTypes.ManifestSemanticValidation, Resources.GetString("Ex_FileExtensionNotSupported"));
        bool flag2 = !this._actDesc.DeployManifest.Deployment.Install;
        if (!flag1 & flag2)
        {
          if (this._downloadOptions == null)
            this._downloadOptions = new DownloadOptions();
          this._downloadOptions.EnforceSizeLimit = true;
          this._downloadOptions.SizeLimit = this._subStore.GetSizeLimitInBytesForSemiTrustApps();
          this._downloadOptions.Size = this._subStore.GetPrivateSize(this._actDesc.AppId);
        }
        tempDirectory = this._subStore.AcquireTempDirectory();
        DownloadManager.DownloadDependencies(subscriptionState, this._actDesc.DeployManifest, this._actDesc.AppManifest, this._actDesc.AppSourceUri, tempDirectory.Path, group, blocking ? (IDownloadNotification) null : (IDownloadNotification) sgh, this._downloadOptions);
        if (!blocking && sgh.CancellationPending)
          return true;
        this._subStore.CommitApplication(ref subscriptionState, new CommitApplicationParams((CommitApplicationParams) this._actDesc)
        {
          CommitApp = true,
          AppPayloadPath = tempDirectory.Path,
          AppManifestPath = (string) null,
          AppGroup = group,
          CommitDeploy = false
        });
      }
      finally
      {
        this.DetachFromGroup(sgh);
        if (tempDirectory != null)
          tempDirectory.Dispose();
      }
      return false;
    }

    private SyncGroupHelper AttachToGroup(string groupName, object userState, out bool created)
    {
      created = false;
      SyncGroupHelper syncGroupHelper = (SyncGroupHelper) null;
      lock (this._syncGroupMap.SyncRoot)
      {
        syncGroupHelper = (SyncGroupHelper) this._syncGroupMap[(object) groupName];
        if (syncGroupHelper == null)
        {
          syncGroupHelper = new SyncGroupHelper(groupName, userState, this.asyncOperation, this.progressReporter);
          this._syncGroupMap[(object) groupName] = (object) syncGroupHelper;
          created = true;
        }
      }
      return syncGroupHelper;
    }

    private void DetachFromGroup(SyncGroupHelper sgh)
    {
      string group = sgh.Group;
      lock (this._syncGroupMap.SyncRoot)
        this._syncGroupMap.Remove((object) group);
      sgh.SetComplete();
    }

    private void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      Logger.EndLogging(this._log);
      if (this._tempDeployment != null)
        this._tempDeployment.Dispose();
      if (this._tempApplicationDirectory != null)
        this._tempApplicationDirectory.Dispose();
      if (this._referenceTransaction != null)
        this._referenceTransaction.Close();
      if (this._actCtx != null)
        this._actCtx.Dispose();
      if (this._events != null)
        this._events.Dispose();
      if (this._trustNotGrantedEvent != null)
        this._trustNotGrantedEvent.Close();
      if (this._trustGrantedEvent != null)
        this._trustGrantedEvent.Close();
      if (this._platformRequirementsFailedEvent == null)
        return;
      this._platformRequirementsFailedEvent.Close();
    }

    private static ActivationContext ConstructActivationContext(ActivationDescription actDesc)
    {
      return ActivationContext.CreatePartialActivationContext(actDesc.AppId.ToApplicationIdentity(), new string[2]
      {
        actDesc.DeployManifestPath,
        actDesc.AppManifestPath
      });
    }

    private static ActivationContext ConstructActivationContextFromStore(DefinitionAppId defAppId)
    {
      return ActivationContext.CreatePartialActivationContext(defAppId.ToApplicationIdentity());
    }

    private void LogError(string message, Exception ex)
    {
      Logger.AddErrorInformation(this._log, message, ex);
      Logger.FlushLog(this._log);
    }

    public enum CallerType
    {
      Other,
      ApplicationDeployment,
      InPlaceHostingManager,
    }
  }
}
