// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.ApplicationDeployment
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.ComponentModel;
using System.Deployment.Application.Manifest;
using System.Deployment.Internal;
using System.Globalization;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Threading;

namespace System.Deployment.Application
{
  /// <summary>Supports updates of the current deployment programmatically, and handles on-demand downloading of files. This class cannot be inherited.</summary>
  public sealed class ApplicationDeployment
  {
    private static readonly object checkForUpdateCompletedKey = new object();
    private static readonly object updateCompletedKey = new object();
    private static readonly object downloadFileGroupCompletedKey = new object();
    private static readonly object checkForUpdateProgressChangedKey = new object();
    private static readonly object updateProgressChangedKey = new object();
    private static readonly object downloadFileGroupProgressChangedKey = new object();
    private static readonly object lockObject = new object();
    private static ApplicationDeployment _currentDeployment = (ApplicationDeployment) null;
    private const int guardInitial = 0;
    private const int guardAsync = 1;
    private const int guardSync = 2;
    private readonly AsyncOperation asyncOperation;
    private readonly CodeAccessPermission accessPermission;
    private int _guard;
    private bool _cancellationPending;
    private SubscriptionStore _subStore;
    private EventHandlerList _events;
    private DefinitionAppId _fullAppId;
    private Version _currentVersion;
    private SubscriptionState _subState;
    private object _syncGroupDeploymentManager;

    /// <summary>Returns the current <see cref="T:System.Deployment.Application.ApplicationDeployment" /> for this deployment.</summary>
    /// <returns>The current deployment.</returns>
    /// <exception cref="T:System.Deployment.Application.InvalidDeploymentException">You attempted to call this static property from a non-ClickOnce application. </exception>
    public static ApplicationDeployment CurrentDeployment
    {
      [PermissionSet(SecurityAction.Assert, Name = "FullTrust")] get
      {
        bool flag = false;
        if (ApplicationDeployment._currentDeployment == null)
        {
          lock (ApplicationDeployment.lockObject)
          {
            if (ApplicationDeployment._currentDeployment == null)
            {
              string local_3 = (string) null;
              ActivationContext local_4 = AppDomain.CurrentDomain.ActivationContext;
              if (local_4 != null)
                local_3 = local_4.Identity.FullName;
              if (string.IsNullOrEmpty(local_3))
                throw new InvalidDeploymentException(Resources.GetString("Ex_AppIdNotSet"));
              ApplicationDeployment._currentDeployment = new ApplicationDeployment(local_3);
              flag = true;
            }
          }
        }
        if (!flag)
          ApplicationDeployment._currentDeployment.DemandPermission();
        return ApplicationDeployment._currentDeployment;
      }
    }

    /// <summary>Gets a value indicating whether the current application is a ClickOnce application.</summary>
    /// <returns>true if this is a ClickOnce application; otherwise, false.</returns>
    public static bool IsNetworkDeployed
    {
      get
      {
        bool flag = true;
        try
        {
          ApplicationDeployment currentDeployment = ApplicationDeployment.CurrentDeployment;
        }
        catch (InvalidDeploymentException ex)
        {
          flag = false;
        }
        return flag;
      }
    }

    /// <summary>Gets the version of the deployment for the current running instance of the application.</summary>
    /// <returns>The current deployment version.</returns>
    public Version CurrentVersion
    {
      get
      {
        return this._currentVersion;
      }
    }

    /// <summary>Gets the version of the update that was recently downloaded.</summary>
    /// <returns>The <see cref="T:System.Version" /> describing the version of the update.</returns>
    public Version UpdatedVersion
    {
      [PermissionSet(SecurityAction.Assert, Name = "FullTrust")] get
      {
        this._subState.Invalidate();
        return this._subState.CurrentDeployment.Version;
      }
    }

    /// <summary>Gets the full name of the application after it has been updated.</summary>
    /// <returns>A <see cref="T:System.String" /> that contains the full name of the application.</returns>
    public string UpdatedApplicationFullName
    {
      [PermissionSet(SecurityAction.Assert, Name = "FullTrust")] get
      {
        this._subState.Invalidate();
        return this._subState.CurrentBind.ToString();
      }
    }

    /// <summary>Gets the date and the time ClickOnce last checked for an application update.</summary>
    /// <returns>The <see cref="T:System.DateTime" /> of the last update check.</returns>
    public DateTime TimeOfLastUpdateCheck
    {
      [PermissionSet(SecurityAction.Assert, Name = "FullTrust")] get
      {
        this._subState.Invalidate();
        return this._subState.LastCheckTime;
      }
    }

    /// <summary>Gets the Web site or file share from which this application updates itself.</summary>
    /// <returns>The update path, expressed as an HTTP, HTTPS, or file URL; or as a Windows network file path (UNC).</returns>
    public Uri UpdateLocation
    {
      [PermissionSet(SecurityAction.Assert, Name = "FullTrust")] get
      {
        this._subState.Invalidate();
        return this._subState.DeploymentProviderUri;
      }
    }

    /// <summary>Gets the URL used to launch the deployment manifest of the application. </summary>
    /// <returns>A zero-length string if the TrustUrlParameters property in the deployment manifest is false, or if the user has supplied a UNC to open the deployment or has opened it locally. Otherwise, the return value is the full URL used to launch the application, including any parameters.</returns>
    public Uri ActivationUri
    {
      [PermissionSet(SecurityAction.Assert, Name = "FullTrust")] get
      {
        this._subState.Invalidate();
        if (!this._subState.CurrentDeploymentManifest.Deployment.TrustURLParameters)
          return (Uri) null;
        string[] activationData = AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData;
        if (activationData == null || activationData[0] == null)
          return (Uri) null;
        Uri uri = new Uri(activationData[0]);
        if (uri.IsFile || uri.IsUnc)
          return (Uri) null;
        return uri;
      }
    }

    /// <summary>Gets the path to the ClickOnce data directory.</summary>
    /// <returns>A string containing the path to the application's data directory on the local disk.</returns>
    public string DataDirectory
    {
      get
      {
        object data = AppDomain.CurrentDomain.GetData("DataDirectory");
        if (data == null)
          return (string) null;
        return data.ToString();
      }
    }

    /// <summary>Gets a value indicating whether this is the first time this application has run on the client computer. </summary>
    /// <returns>true if this version of the application has never run on the client computer before; otherwise, false.</returns>
    public bool IsFirstRun
    {
      [PermissionSet(SecurityAction.Assert, Name = "FullTrust")] get
      {
        return InternalActivationContextHelper.IsFirstRun(AppDomain.CurrentDomain.ActivationContext);
      }
    }

    private EventHandlerList Events
    {
      get
      {
        return this._events;
      }
    }

    private DeploymentManager SyncGroupDeploymentManager
    {
      get
      {
        if (this._syncGroupDeploymentManager == null)
        {
          DeploymentManager deploymentManager = (DeploymentManager) null;
          bool flag = false;
          try
          {
            deploymentManager = new DeploymentManager(this._fullAppId.ToString(), true, true, (DownloadOptions) null, this.asyncOperation);
            deploymentManager.Callertype = DeploymentManager.CallerType.ApplicationDeployment;
            deploymentManager.Bind();
            flag = Interlocked.CompareExchange(ref this._syncGroupDeploymentManager, (object) deploymentManager, (object) null) == null;
          }
          finally
          {
            if (!flag && deploymentManager != null)
              deploymentManager.Dispose();
          }
          if (flag)
          {
            deploymentManager.ProgressChanged += new DeploymentProgressChangedEventHandler(this.DownloadFileGroupProgressChangedEventHandler);
            deploymentManager.SynchronizeCompleted += new SynchronizeCompletedEventHandler(this.SynchronizeGroupCompletedEventHandler);
          }
        }
        return (DeploymentManager) this._syncGroupDeploymentManager;
      }
    }

    /// <summary>Occurs when a progress update is available on a <see cref="M:System.Deployment.Application.ApplicationDeployment.CheckForUpdateAsync" /> call.</summary>
    public event DeploymentProgressChangedEventHandler CheckForUpdateProgressChanged
    {
      add
      {
        this.Events.AddHandler(ApplicationDeployment.checkForUpdateProgressChangedKey, (Delegate) value);
      }
      remove
      {
        this.Events.RemoveHandler(ApplicationDeployment.checkForUpdateProgressChangedKey, (Delegate) value);
      }
    }

    /// <summary>Occurs when <see cref="M:System.Deployment.Application.ApplicationDeployment.CheckForUpdateAsync" /> has completed.</summary>
    public event CheckForUpdateCompletedEventHandler CheckForUpdateCompleted
    {
      add
      {
        this.Events.AddHandler(ApplicationDeployment.checkForUpdateCompletedKey, (Delegate) value);
      }
      remove
      {
        this.Events.RemoveHandler(ApplicationDeployment.checkForUpdateCompletedKey, (Delegate) value);
      }
    }

    /// <summary>Occurs when ClickOnce has new status information for an update operation initiated by calling the <see cref="M:System.Deployment.Application.ApplicationDeployment.UpdateAsync" /> method.</summary>
    public event DeploymentProgressChangedEventHandler UpdateProgressChanged
    {
      add
      {
        this.Events.AddHandler(ApplicationDeployment.updateProgressChangedKey, (Delegate) value);
      }
      remove
      {
        this.Events.RemoveHandler(ApplicationDeployment.updateProgressChangedKey, (Delegate) value);
      }
    }

    /// <summary>Occurs when ClickOnce has finished upgrading the application as the result of a call to <see cref="M:System.Deployment.Application.ApplicationDeployment.UpdateAsync" />.</summary>
    public event AsyncCompletedEventHandler UpdateCompleted
    {
      add
      {
        this.Events.AddHandler(ApplicationDeployment.updateCompletedKey, (Delegate) value);
      }
      remove
      {
        this.Events.RemoveHandler(ApplicationDeployment.updateCompletedKey, (Delegate) value);
      }
    }

    /// <summary>Occurs when status information is available on a file download operation initiated by a call to <see cref="Overload:System.Deployment.Application.ApplicationDeployment.DownloadFileGroupAsync" />.</summary>
    public event DeploymentProgressChangedEventHandler DownloadFileGroupProgressChanged
    {
      add
      {
        this.Events.AddHandler(ApplicationDeployment.downloadFileGroupProgressChangedKey, (Delegate) value);
      }
      remove
      {
        this.Events.RemoveHandler(ApplicationDeployment.downloadFileGroupProgressChangedKey, (Delegate) value);
      }
    }

    /// <summary>Occurs on the main application thread when a file download is complete.</summary>
    public event DownloadFileGroupCompletedEventHandler DownloadFileGroupCompleted
    {
      add
      {
        this.Events.AddHandler(ApplicationDeployment.downloadFileGroupCompletedKey, (Delegate) value);
      }
      remove
      {
        this.Events.RemoveHandler(ApplicationDeployment.downloadFileGroupCompletedKey, (Delegate) value);
      }
    }

    private ApplicationDeployment(string fullAppId)
    {
      if (fullAppId.Length > 65536)
        throw new InvalidDeploymentException(Resources.GetString("Ex_AppIdTooLong"));
      try
      {
        this._fullAppId = new DefinitionAppId(fullAppId);
      }
      catch (COMException ex)
      {
        throw new InvalidDeploymentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_SubAppIdNotValid"), new object[1]
        {
          (object) fullAppId
        }), (Exception) ex);
      }
      catch (SEHException ex)
      {
        throw new InvalidDeploymentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_SubAppIdNotValid"), new object[1]
        {
          (object) fullAppId
        }), (Exception) ex);
      }
      DefinitionIdentity deploymentIdentity = this._fullAppId.DeploymentIdentity;
      this._currentVersion = deploymentIdentity.Version;
      DefinitionIdentity subscriptionId = deploymentIdentity.ToSubscriptionId();
      this._subStore = SubscriptionStore.CurrentUser;
      this._subState = this._subStore.GetSubscriptionState(subscriptionId);
      if (!this._subState.IsInstalled)
        throw new InvalidDeploymentException(Resources.GetString("Ex_SubNotInstalled"));
      if (!this._fullAppId.Equals((object) this._subState.CurrentBind))
        throw new InvalidDeploymentException(Resources.GetString("Ex_AppIdNotMatchInstalled"));
      Uri uri = new Uri(this._fullAppId.Codebase);
      this.accessPermission = !uri.IsFile ? (CodeAccessPermission) new WebPermission(NetworkAccess.Connect, this._fullAppId.Codebase) : (CodeAccessPermission) new FileIOPermission(FileIOPermissionAccess.Read, uri.LocalPath);
      this.accessPermission.Demand();
      this._events = new EventHandlerList();
      this.asyncOperation = AsyncOperationManager.CreateOperation((object) null);
    }

    /// <summary>Performs the same operation as <see cref="M:System.Deployment.Application.ApplicationDeployment.CheckForUpdate" />, but returns extended information about the available update.</summary>
    /// <returns>An <see cref="T:System.Deployment.Application.UpdateCheckInfo" /> for the available update.</returns>
    /// <exception cref="T:System.InvalidOperationException">The current application is either not configured to support updates, or there is another update check operation already in progress.</exception>
    /// <exception cref="T:System.Deployment.Application.DeploymentDownloadException">The deployment manifest cannot be downloaded. This exception will appear in the <see cref="P:System.ComponentModel.AsyncCompletedEventArgs.Error" /> property of the <see cref="E:System.Deployment.Application.ApplicationDeployment.CheckForUpdateCompleted" /> event.</exception>
    /// <exception cref="T:System.Deployment.Application.InvalidDeploymentException">The deployment manifest is corrupted. Regenerate the application's manifest before you attempt to deploy this application to users. This exception will appear in the <see cref="P:System.ComponentModel.AsyncCompletedEventArgs.Error" /> property of the <see cref="E:System.Deployment.Application.ApplicationDeployment.CheckForUpdateCompleted" /> event.</exception>
    public UpdateCheckInfo CheckForDetailedUpdate()
    {
      return this.CheckForDetailedUpdate(true);
    }

    /// <summary>Performs the same operation as <see cref="M:System.Deployment.Application.ApplicationDeployment.CheckForUpdate" />, but returns extended information about the available update.</summary>
    /// <returns>An <see cref="T:System.Deployment.Application.UpdateCheckInfo" /> for the available update.</returns>
    /// <param name="persistUpdateCheckResult">If false, the update will be applied silently and no dialog box will be displayed.</param>
    public UpdateCheckInfo CheckForDetailedUpdate(bool persistUpdateCheckResult)
    {
      new NamedPermissionSet("FullTrust").Demand();
      if (Interlocked.CompareExchange(ref this._guard, 2, 0) != 0)
        throw new InvalidOperationException(Resources.GetString("Ex_SingleOperation"));
      this._cancellationPending = false;
      UpdateCheckInfo info = (UpdateCheckInfo) null;
      try
      {
        DeploymentManager deploymentManager = this.CreateDeploymentManager();
        try
        {
          deploymentManager.Bind();
          info = this.DetermineUpdateCheckResult(deploymentManager.ActivationDescription);
          if (info.UpdateAvailable)
          {
            deploymentManager.DeterminePlatformRequirements();
            try
            {
              deploymentManager.DetermineTrust(new TrustParams()
              {
                NoPrompt = true
              });
            }
            catch (TrustNotGrantedException ex)
            {
              if (!deploymentManager.ActivationDescription.IsUpdateInPKTGroup)
                throw;
            }
          }
          if (persistUpdateCheckResult)
            this.ProcessUpdateCheckResult(info, deploymentManager.ActivationDescription);
        }
        finally
        {
          deploymentManager.Dispose();
        }
      }
      finally
      {
        Interlocked.Exchange(ref this._guard, 0);
      }
      return info;
    }

    /// <summary>Checks <see cref="P:System.Deployment.Application.ApplicationDeployment.UpdateLocation" /> to determine whether a new update is available.</summary>
    /// <returns>true if a new update is available; otherwise, false.</returns>
    /// <exception cref="T:System.InvalidOperationException">ClickOnce throws this exception immediately if you call the <see cref="M:System.Deployment.Application.ApplicationDeployment.CheckForUpdate" />  method while an update is already in progress.</exception>
    /// <exception cref="T:System.Deployment.Application.DeploymentDownloadException">The deployment manifest cannot be downloaded. </exception>
    /// <exception cref="T:System.Deployment.Application.InvalidDeploymentException">The deployment manifest is corrupted. You will likely need to redeploy the application to fix this problem. </exception>
    public bool CheckForUpdate()
    {
      return this.CheckForUpdate(true);
    }

    /// <summary>Checks <see cref="P:System.Deployment.Application.ApplicationDeployment.UpdateLocation" /> to determine whether a new update is available.</summary>
    /// <returns>true if a new update is available; otherwise, false.</returns>
    /// <param name="persistUpdateCheckResult">If false, the update will be applied silently and no dialog box will be displayed.</param>
    public bool CheckForUpdate(bool persistUpdateCheckResult)
    {
      return this.CheckForDetailedUpdate(persistUpdateCheckResult).UpdateAvailable;
    }

    /// <summary>Checks <see cref="P:System.Deployment.Application.ApplicationDeployment.UpdateLocation" /> asynchronously to determine whether a new update is available.</summary>
    /// <exception cref="T:System.InvalidOperationException">ClickOnce throws this exception immediately if you call the <see cref="M:System.Deployment.Application.ApplicationDeployment.CheckForUpdateAsync" />  method while an update is already in progress.</exception>
    /// <exception cref="T:System.Deployment.Application.DeploymentDownloadException">The deployment manifest cannot be downloaded. This exception appears in the <see cref="P:System.ComponentModel.AsyncCompletedEventArgs.Error" /> property of the <see cref="E:System.Deployment.Application.ApplicationDeployment.CheckForUpdateCompleted" /> event.</exception>
    /// <exception cref="T:System.Deployment.Application.InvalidDeploymentException">The deployment manifest is corrupted. You will likely need to redeploy the application to fix this problem. This exception appears in the <see cref="P:System.ComponentModel.AsyncCompletedEventArgs.Error" /> property of the <see cref="E:System.Deployment.Application.ApplicationDeployment.CheckForUpdateCompleted" /> event.</exception>
    public void CheckForUpdateAsync()
    {
      new NamedPermissionSet("FullTrust").Demand();
      if (Interlocked.CompareExchange(ref this._guard, 1, 0) != 0)
        throw new InvalidOperationException(Resources.GetString("Ex_SingleOperation"));
      this._cancellationPending = false;
      DeploymentManager deploymentManager = this.CreateDeploymentManager();
      deploymentManager.ProgressChanged += new DeploymentProgressChangedEventHandler(this.CheckForUpdateProgressChangedEventHandler);
      deploymentManager.BindCompleted += new BindCompletedEventHandler(this.CheckForUpdateBindCompletedEventHandler);
      deploymentManager.BindAsync();
    }

    /// <summary>Cancels the asynchronous update check.</summary>
    public void CheckForUpdateAsyncCancel()
    {
      if (this._guard != 1)
        return;
      this._cancellationPending = true;
    }

    /// <summary>Starts a synchronous download and installation of the latest version of this application. </summary>
    /// <returns>true if an application has been updated; otherwise, false.</returns>
    /// <exception cref="T:System.Deployment.Application.TrustNotGrantedException">The local computer did not grant the application the permission level it requested to execute.</exception>
    /// <exception cref="T:System.Deployment.Application.InvalidDeploymentException">Your ClickOnce deployment is corrupted. For tips on how to diagnose and correct the problem, see Troubleshooting ClickOnce Deployments.</exception>
    /// <exception cref="T:System.Deployment.Application.DeploymentDownloadException">The new deployment could not be downloaded from its location on the network.</exception>
    /// <exception cref="T:System.InvalidOperationException">The application is currently being updated.</exception>
    public bool Update()
    {
      new NamedPermissionSet("FullTrust").Demand();
      if (Interlocked.CompareExchange(ref this._guard, 2, 0) != 0)
        throw new InvalidOperationException(Resources.GetString("Ex_SingleOperation"));
      this._cancellationPending = false;
      try
      {
        DeploymentManager deploymentManager = this.CreateDeploymentManager();
        try
        {
          deploymentManager.Bind();
          UpdateCheckInfo updateCheckResult = this.DetermineUpdateCheckResult(deploymentManager.ActivationDescription);
          if (updateCheckResult.UpdateAvailable)
          {
            deploymentManager.DeterminePlatformRequirements();
            try
            {
              deploymentManager.DetermineTrust(new TrustParams()
              {
                NoPrompt = true
              });
            }
            catch (TrustNotGrantedException ex)
            {
              if (!deploymentManager.ActivationDescription.IsUpdateInPKTGroup)
                throw;
            }
          }
          this.ProcessUpdateCheckResult(updateCheckResult, deploymentManager.ActivationDescription);
          if (!updateCheckResult.UpdateAvailable)
            return false;
          deploymentManager.Synchronize();
          if (deploymentManager.ActivationDescription.IsUpdateInPKTGroup)
            this._subState = this._subStore.GetSubscriptionState(deploymentManager.ActivationDescription.DeployManifest);
        }
        finally
        {
          deploymentManager.Dispose();
        }
      }
      finally
      {
        Interlocked.Exchange(ref this._guard, 0);
      }
      return true;
    }

    /// <summary>Starts an asynchronous download and installation of the latest version of this application.</summary>
    /// <exception cref="T:System.Deployment.Application.TrustNotGrantedException">The local computer did not grant this application the permission level it requested to execute.</exception>
    /// <exception cref="T:System.Deployment.Application.InvalidDeploymentException">Your ClickOnce deployment is corrupted. For tips on how to diagnose and correct the problem, see Troubleshooting ClickOnce Deployments.</exception>
    /// <exception cref="T:System.Deployment.Application.DeploymentDownloadException">The new deployment could not be downloaded from its location on the network.</exception>
    public void UpdateAsync()
    {
      new NamedPermissionSet("FullTrust").Demand();
      if (Interlocked.CompareExchange(ref this._guard, 1, 0) != 0)
        throw new InvalidOperationException(Resources.GetString("Ex_SingleOperation"));
      this._cancellationPending = false;
      DeploymentManager deploymentManager = this.CreateDeploymentManager();
      deploymentManager.ProgressChanged += new DeploymentProgressChangedEventHandler(this.UpdateProgressChangedEventHandler);
      deploymentManager.BindCompleted += new BindCompletedEventHandler(this.UpdateBindCompletedEventHandler);
      deploymentManager.SynchronizeCompleted += new SynchronizeCompletedEventHandler(this.SynchronizeNullCompletedEventHandler);
      deploymentManager.BindAsync();
    }

    /// <summary>Cancels an asynchronous update initiated by <see cref="M:System.Deployment.Application.ApplicationDeployment.UpdateAsync" />.</summary>
    public void UpdateAsyncCancel()
    {
      if (this._guard != 1)
        return;
      this._cancellationPending = true;
    }

    /// <summary>Downloads a set of optional files on demand.</summary>
    /// <param name="groupName">The named group of files to download. All files marked "optional" in a ClickOnce application require a group name.</param>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="groupName" /> parameter is null or zero-length.</exception>
    [PermissionSet(SecurityAction.Assert, Name = "FullTrust")]
    public void DownloadFileGroup(string groupName)
    {
      if (groupName == null)
        throw new ArgumentNullException("groupName");
      this._subState.Invalidate();
      if (!this._fullAppId.Equals((object) this._subState.CurrentBind))
        throw new InvalidOperationException(Resources.GetString("Ex_DownloadGroupAfterUpdate"));
      this.SyncGroupDeploymentManager.Synchronize(groupName);
    }

    /// <summary>Downloads, on demand, a set of optional files in the background.</summary>
    /// <param name="groupName">The named group of files to download. All files marked "optional" in a ClickOnce application require a group name.</param>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="groupName" /> parameter is null or zero-length.</exception>
    /// <exception cref="T:System.InvalidOperationException">You cannot initiate more than one download of <paramref name="groupName" /> at a time.</exception>
    public void DownloadFileGroupAsync(string groupName)
    {
      this.DownloadFileGroupAsync(groupName, (object) null);
    }

    /// <summary>Downloads, on demand, a set of optional files in the background, and passes a piece of application state to the event callbacks.</summary>
    /// <param name="groupName">The named group of files to download. All files marked "optional" in a ClickOnce application require a group name.</param>
    /// <param name="userState">An arbitrary object containing state information for the asynchronous operation.</param>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="groupName" /> parameter is null or zero-length.</exception>
    /// <exception cref="T:System.InvalidOperationException">You cannot initiate more than one download of <paramref name="groupName" /> at a time.</exception>
    [PermissionSet(SecurityAction.Assert, Name = "FullTrust")]
    public void DownloadFileGroupAsync(string groupName, object userState)
    {
      if (groupName == null)
        throw new ArgumentNullException("groupName");
      this._subState.Invalidate();
      if (!this._fullAppId.Equals((object) this._subState.CurrentBind))
        throw new InvalidOperationException(Resources.GetString("Ex_DownloadGroupAfterUpdate"));
      this.SyncGroupDeploymentManager.SynchronizeAsync(groupName, userState);
    }

    /// <summary>Checks whether the named file group has already been downloaded to the client computer.</summary>
    /// <returns>true if the file group has already been downloaded for the current version of this application; otherwise, false. If a new version of the application has been installed, and the new version has not added, removed, or altered files in the file group, <see cref="M:System.Deployment.Application.ApplicationDeployment.IsFileGroupDownloaded(System.String)" /> returns true.</returns>
    /// <param name="groupName">The named group of files to download. All files marked "optional" in a ClickOnce application require a group name.</param>
    /// <exception cref="T:System.Deployment.Application.InvalidDeploymentException">
    /// <paramref name="groupName" /> is not a file group defined in the application manifest.</exception>
    [PermissionSet(SecurityAction.Assert, Name = "FullTrust")]
    public bool IsFileGroupDownloaded(string groupName)
    {
      return this._subStore.CheckGroupInstalled(this._subState, this._fullAppId, groupName);
    }

    /// <summary>Cancels an asynchronous file download.</summary>
    /// <param name="groupName">The named group of files to download. All files marked "optional" in a ClickOnce application require a group name.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="groupName" /> cannot be null.</exception>
    [PermissionSet(SecurityAction.Assert, Name = "FullTrust")]
    public void DownloadFileGroupAsyncCancel(string groupName)
    {
      if (groupName == null)
        throw new ArgumentNullException("groupName");
      this.SyncGroupDeploymentManager.CancelAsync(groupName);
    }

    private DeploymentManager CreateDeploymentManager()
    {
      this._subState.Invalidate();
      return new DeploymentManager(this._subState.DeploymentProviderUri, true, true, (DownloadOptions) null, this.asyncOperation)
      {
        Callertype = DeploymentManager.CallerType.ApplicationDeployment
      };
    }

    private void CheckForUpdateProgressChangedEventHandler(object sender, DeploymentProgressChangedEventArgs e)
    {
      if (this._cancellationPending)
        ((DeploymentManager) sender).CancelAsync();
      DeploymentProgressChangedEventHandler changedEventHandler = (DeploymentProgressChangedEventHandler) this.Events[ApplicationDeployment.checkForUpdateProgressChangedKey];
      if (changedEventHandler == null)
        return;
      changedEventHandler((object) this, e);
    }

    private void UpdateProgressChangedEventHandler(object sender, DeploymentProgressChangedEventArgs e)
    {
      if (this._cancellationPending)
        ((DeploymentManager) sender).CancelAsync();
      DeploymentProgressChangedEventHandler changedEventHandler = (DeploymentProgressChangedEventHandler) this.Events[ApplicationDeployment.updateProgressChangedKey];
      if (changedEventHandler == null)
        return;
      changedEventHandler((object) this, e);
    }

    private void DownloadFileGroupProgressChangedEventHandler(object sender, DeploymentProgressChangedEventArgs e)
    {
      DeploymentProgressChangedEventHandler changedEventHandler = (DeploymentProgressChangedEventHandler) this.Events[ApplicationDeployment.downloadFileGroupProgressChangedKey];
      if (changedEventHandler == null)
        return;
      changedEventHandler((object) this, e);
    }

    private void CheckForUpdateBindCompletedEventHandler(object sender, BindCompletedEventArgs e)
    {
      Exception error = (Exception) null;
      DeploymentManager deploymentManager = (DeploymentManager) null;
      bool updateAvailable = false;
      Version availableVersion = (Version) null;
      bool isUpdateRequired = false;
      Version minimumRequiredVersion = (Version) null;
      long updateSize = 0;
      new NamedPermissionSet("FullTrust").Assert();
      try
      {
        deploymentManager = (DeploymentManager) sender;
        if (e.Error == null && !e.Cancelled)
        {
          UpdateCheckInfo updateCheckResult = this.DetermineUpdateCheckResult(deploymentManager.ActivationDescription);
          if (updateCheckResult.UpdateAvailable)
          {
            deploymentManager.DeterminePlatformRequirements();
            try
            {
              deploymentManager.DetermineTrust(new TrustParams()
              {
                NoPrompt = true
              });
            }
            catch (TrustNotGrantedException ex)
            {
              if (!deploymentManager.ActivationDescription.IsUpdateInPKTGroup)
                throw;
            }
          }
          this.ProcessUpdateCheckResult(updateCheckResult, deploymentManager.ActivationDescription);
          if (!updateCheckResult.UpdateAvailable)
            return;
          updateAvailable = true;
          availableVersion = updateCheckResult.AvailableVersion;
          isUpdateRequired = updateCheckResult.IsUpdateRequired;
          minimumRequiredVersion = updateCheckResult.MinimumRequiredVersion;
          updateSize = updateCheckResult.UpdateSizeBytes;
        }
        else
          error = e.Error;
      }
      catch (Exception ex)
      {
        if (ExceptionUtility.IsHardException(ex))
          throw;
        else
          error = ex;
      }
      finally
      {
        CodeAccessPermission.RevertAssert();
        Interlocked.Exchange(ref this._guard, 0);
        CheckForUpdateCompletedEventArgs e1 = new CheckForUpdateCompletedEventArgs(error, e.Cancelled, (object) null, updateAvailable, availableVersion, isUpdateRequired, minimumRequiredVersion, updateSize);
        CheckForUpdateCompletedEventHandler completedEventHandler = (CheckForUpdateCompletedEventHandler) this.Events[ApplicationDeployment.checkForUpdateCompletedKey];
        if (completedEventHandler != null)
          completedEventHandler((object) this, e1);
        if (deploymentManager != null)
        {
          deploymentManager.ProgressChanged -= new DeploymentProgressChangedEventHandler(this.CheckForUpdateProgressChangedEventHandler);
          deploymentManager.BindCompleted -= new BindCompletedEventHandler(this.CheckForUpdateBindCompletedEventHandler);
          new NamedPermissionSet("FullTrust").Assert();
          try
          {
            deploymentManager.Dispose();
          }
          finally
          {
            CodeAccessPermission.RevertAssert();
          }
        }
      }
    }

    private void UpdateBindCompletedEventHandler(object sender, BindCompletedEventArgs e)
    {
      Exception error = (Exception) null;
      DeploymentManager dm = (DeploymentManager) null;
      bool flag = false;
      new NamedPermissionSet("FullTrust").Assert();
      try
      {
        dm = (DeploymentManager) sender;
        if (e.Error == null && !e.Cancelled)
        {
          UpdateCheckInfo updateCheckResult = this.DetermineUpdateCheckResult(dm.ActivationDescription);
          if (updateCheckResult.UpdateAvailable)
          {
            dm.DeterminePlatformRequirements();
            try
            {
              dm.DetermineTrust(new TrustParams()
              {
                NoPrompt = true
              });
            }
            catch (TrustNotGrantedException ex)
            {
              if (!dm.ActivationDescription.IsUpdateInPKTGroup)
                throw;
            }
          }
          this.ProcessUpdateCheckResult(updateCheckResult, dm.ActivationDescription);
          if (updateCheckResult.UpdateAvailable)
          {
            flag = true;
            dm.SynchronizeAsync();
          }
          if (!dm.ActivationDescription.IsUpdateInPKTGroup)
            return;
          this._subState = this._subStore.GetSubscriptionState(dm.ActivationDescription.DeployManifest);
        }
        else
          error = e.Error;
      }
      catch (Exception ex)
      {
        if (ExceptionUtility.IsHardException(ex))
          throw;
        else
          error = ex;
      }
      finally
      {
        CodeAccessPermission.RevertAssert();
        if (!flag)
          this.EndUpdateAsync(dm, error, e.Cancelled);
      }
    }

    private void EndUpdateAsync(DeploymentManager dm, Exception error, bool cancelled)
    {
      Interlocked.Exchange(ref this._guard, 0);
      AsyncCompletedEventArgs e = new AsyncCompletedEventArgs(error, cancelled, (object) null);
      AsyncCompletedEventHandler completedEventHandler = (AsyncCompletedEventHandler) this.Events[ApplicationDeployment.updateCompletedKey];
      if (completedEventHandler != null)
        completedEventHandler((object) this, e);
      if (dm == null)
        return;
      dm.ProgressChanged -= new DeploymentProgressChangedEventHandler(this.UpdateProgressChangedEventHandler);
      dm.BindCompleted -= new BindCompletedEventHandler(this.UpdateBindCompletedEventHandler);
      dm.SynchronizeCompleted -= new SynchronizeCompletedEventHandler(this.SynchronizeNullCompletedEventHandler);
      new NamedPermissionSet("FullTrust").Assert();
      try
      {
        dm.Dispose();
      }
      finally
      {
        CodeAccessPermission.RevertAssert();
      }
    }

    private void SynchronizeNullCompletedEventHandler(object sender, SynchronizeCompletedEventArgs e)
    {
      Exception error = (Exception) null;
      DeploymentManager dm = (DeploymentManager) null;
      new NamedPermissionSet("FullTrust").Assert();
      try
      {
        dm = (DeploymentManager) sender;
        error = e.Error;
      }
      catch (Exception ex)
      {
        if (ExceptionUtility.IsHardException(ex))
          throw;
        else
          error = ex;
      }
      finally
      {
        CodeAccessPermission.RevertAssert();
        this.EndUpdateAsync(dm, error, e.Cancelled);
      }
    }

    private void SynchronizeGroupCompletedEventHandler(object sender, SynchronizeCompletedEventArgs e)
    {
      Exception exception = (Exception) null;
      DeploymentManager deploymentManager = (DeploymentManager) null;
      try
      {
        deploymentManager = (DeploymentManager) sender;
        exception = e.Error;
      }
      catch (Exception ex)
      {
        if (ExceptionUtility.IsHardException(ex))
          throw;
        else
          exception = ex;
      }
      finally
      {
        DownloadFileGroupCompletedEventArgs e1 = new DownloadFileGroupCompletedEventArgs(e.Error, e.Cancelled, e.UserState, e.Group);
        DownloadFileGroupCompletedEventHandler completedEventHandler = (DownloadFileGroupCompletedEventHandler) this.Events[ApplicationDeployment.downloadFileGroupCompletedKey];
        if (completedEventHandler != null)
          completedEventHandler((object) this, e1);
      }
    }

    private UpdateCheckInfo DetermineUpdateCheckResult(ActivationDescription actDesc)
    {
      bool updateAvailable = false;
      Version availableVersion = (Version) null;
      bool isUpdateRequired = false;
      Version minimumRequiredVersion = (Version) null;
      long updateSize = 0;
      bool bUpdateInPKTGroup = false;
      AssemblyManifest deployManifest = actDesc.DeployManifest;
      this._subState.Invalidate();
      Version version = this._subStore.CheckUpdateInManifest(this._subState, actDesc.DeploySourceUri, deployManifest, this._currentVersion, ref bUpdateInPKTGroup);
      if (version != (Version) null && !deployManifest.Identity.Equals((object) this._subState.ExcludedDeployment))
      {
        updateAvailable = true;
        availableVersion = version;
        minimumRequiredVersion = deployManifest.Deployment.MinimumRequiredVersion;
        if (minimumRequiredVersion != (Version) null && minimumRequiredVersion.CompareTo(this._currentVersion) > 0)
          isUpdateRequired = true;
        ulong dependenciesSize = actDesc.AppManifest.CalculateDependenciesSize();
        updateSize = dependenciesSize <= 9223372036854775807UL ? (long) dependenciesSize : long.MaxValue;
        actDesc.IsUpdateInPKTGroup = bUpdateInPKTGroup;
      }
      return new UpdateCheckInfo(updateAvailable, availableVersion, isUpdateRequired, minimumRequiredVersion, updateSize);
    }

    private void ProcessUpdateCheckResult(UpdateCheckInfo info, ActivationDescription actDesc)
    {
      if (!this._subState.IsShellVisible)
        return;
      AssemblyManifest deployManifest = actDesc.DeployManifest;
      this._subStore.SetPendingDeployment(this._subState, info.UpdateAvailable ? deployManifest.Identity : (DefinitionIdentity) null, DateTime.UtcNow);
    }

    private void DemandPermission()
    {
      this.accessPermission.Demand();
    }
  }
}
