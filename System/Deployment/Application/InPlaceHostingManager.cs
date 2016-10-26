// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.InPlaceHostingManager
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Security.Permissions;

namespace System.Deployment.Application
{
  /// <summary>Install or update a ClickOnce deployment on a computer.</summary>
  [SecurityPermission(SecurityAction.Demand, Unrestricted = true)]
  public class InPlaceHostingManager : IDisposable
  {
    private DeploymentManager _deploymentManager;
    private InPlaceHostingManager.State _state;
    private bool _isCached;
    private bool _isLaunchInHostProcess;
    private object _lock;
    private AppType _appType;
    private Logger.LogIdentity _log;

    /// <summary>Occurs when the deployment manifest has been downloaded to the local computer.</summary>
    public event EventHandler<GetManifestCompletedEventArgs> GetManifestCompleted;

    /// <summary>Occurs when there is a change in the status of an application or manifest download.</summary>
    public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;

    /// <summary>Occurs when the application has finished downloading to the local computer.</summary>
    public event EventHandler<DownloadApplicationCompletedEventArgs> DownloadApplicationCompleted;

    /// <summary>Creates a new instance of <see cref="T:System.Deployment.Application.InPlaceHostingManager" /> to download and install the specified application, which can be either a stand-alone Windows Forms-based application or an application hosted in a Web browser.</summary>
    /// <param name="deploymentManifest">The Uniform Resource Identifier (URI) to the deployment manifest of the application that will be installed.</param>
    /// <param name="launchInHostProcess">Whether this application will be run in a host, such as a Web browser. For a stand-alone application, set this value to false.</param>
    /// <exception cref="T:System.PlatformNotSupportedException">
    /// <see cref="T:System.Deployment.Application.InPlaceHostingManager" /> can be used only in Windows XP or in later versions of the Windows operating system.</exception>
    /// <exception cref="T:System.ArgumentNullException">Cannot pass null for the <paramref name="deploymentManifest" /> argument.</exception>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="deploymentManifest" /> uses a URI scheme that is not supported by ClickOnce.</exception>
    public InPlaceHostingManager(Uri deploymentManifest, bool launchInHostProcess)
    {
      if (!PlatformSpecific.OnXPOrAbove)
        throw new PlatformNotSupportedException(Resources.GetString("Ex_RequiresXPOrHigher"));
      if (deploymentManifest == (Uri) null)
        throw new ArgumentNullException("deploymentManifest");
      UriHelper.ValidateSupportedSchemeInArgument(deploymentManifest, "deploymentSource");
      this._deploymentManager = new DeploymentManager(deploymentManifest, false, true, (DownloadOptions) null, (AsyncOperation) null);
      this._log = this._deploymentManager.LogId;
      this._isLaunchInHostProcess = launchInHostProcess;
      this._Initialize();
      Logger.AddInternalState(this._log, "Activation through IPHM APIs started.");
      Logger.AddMethodCall(this._log, "InPlaceHostingManager(" + (object) deploymentManifest + "," + launchInHostProcess.ToString() + ") called.");
    }

    /// <summary>Creates a new instance of <see cref="T:System.Deployment.Application.InPlaceHostingManager" /> to download and install the specified browser-hosted application.</summary>
    /// <param name="deploymentManifest">A Uniform Resource Identifier (<see cref="T:System.Uri" />) to a ClickOnce application's deployment manifest.</param>
    /// <exception cref="T:System.PlatformNotSupportedException">
    /// <see cref="T:System.Deployment.Application.InPlaceHostingManager" /> can be used only in Windows XP or in later versions of the Windows operating system.</exception>
    /// <exception cref="T:System.ArgumentNullException">Cannot pass null for the <paramref name="deploymentManifest" /> argument.</exception>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="deploymentManifest" /> uses a URI scheme that is not supported by ClickOnce.</exception>
    public InPlaceHostingManager(Uri deploymentManifest)
      : this(deploymentManifest, true)
    {
    }

    private void _Initialize()
    {
      this._lock = new object();
      this._deploymentManager.BindCompleted += new BindCompletedEventHandler(this.OnBindCompleted);
      this._deploymentManager.SynchronizeCompleted += new SynchronizeCompletedEventHandler(this.OnSynchronizeCompleted);
      this._deploymentManager.ProgressChanged += new DeploymentProgressChangedEventHandler(this.OnProgressChanged);
      this._state = InPlaceHostingManager.State.Ready;
    }

    /// <summary>Downloads the deployment manifest of the ClickOnce application in the background, and raises an event when the operation has either completed or encountered an error.</summary>
    public void GetManifestAsync()
    {
      lock (this._lock)
      {
        try
        {
          this.AssertState(InPlaceHostingManager.State.Ready);
          Logger.AddMethodCall(this._log, "InPlaceHostingManager.GetManifestAsync() called.");
          this.ChangeState(InPlaceHostingManager.State.GettingManifest);
          this._deploymentManager.BindAsync();
        }
        catch (Exception exception_0)
        {
          Logger.AddInternalState(this._log, "Exception thrown in  GetManifestAsync(): " + exception_0.GetType().ToString() + " : " + exception_0.Message + "\r\n" + exception_0.StackTrace);
          this.ChangeState(InPlaceHostingManager.State.Done);
          throw;
        }
      }
    }

    /// <summary>Determines whether the ClickOnce application has the appropriate permissions and platform dependencies to run on the local computer.</summary>
    /// <exception cref="T:System.InvalidOperationException">Raised if this method is called before the <see cref="M:System.Deployment.Application.InPlaceHostingManager.GetManifestAsync" /> method.</exception>
    public void AssertApplicationRequirements()
    {
      lock (this._lock)
      {
        try
        {
          Logger.AddMethodCall(this._log, "InPlaceHostingManager.AssertApplicationRequirements() called.");
          if (this._appType == AppType.CustomHostSpecified)
            throw new InvalidOperationException(Resources.GetString("Ex_CannotCallAssertApplicationRequirements"));
          this.AssertApplicationRequirements(false);
        }
        catch (Exception exception_0)
        {
          Logger.AddInternalState(this._log, "Exception thrown in AssertApplicationRequirements(): " + exception_0.GetType().ToString() + " : " + exception_0.Message + "\r\n" + exception_0.StackTrace);
          throw;
        }
      }
    }

    /// <summary>Determines whether the ClickOnce application has the appropriate permissions and platform dependencies to run on the local computer.</summary>
    /// <param name="grantApplicationTrust">If true, the application will attempt to elevate its permissions to the required level.</param>
    public void AssertApplicationRequirements(bool grantApplicationTrust)
    {
      lock (this._lock)
      {
        try
        {
          Logger.AddMethodCall(this._log, "InPlaceHostingManager.AssertApplicationRequirements(" + grantApplicationTrust.ToString() + ") called.");
          if (this._appType == AppType.CustomHostSpecified)
            throw new InvalidOperationException(Resources.GetString("Ex_CannotCallAssertApplicationRequirements"));
          this.AssertState(InPlaceHostingManager.State.GetManifestSucceeded, InPlaceHostingManager.State.DownloadingApplication);
          this.ChangeState(InPlaceHostingManager.State.VerifyingRequirements);
          this._deploymentManager.DeterminePlatformRequirements();
          if (grantApplicationTrust)
          {
            Logger.AddMethodCall(this._log, "Persisting trust without evaluation.");
            this._deploymentManager.PersistTrustWithoutEvaluation();
          }
          else
            this._deploymentManager.DetermineTrust(new TrustParams()
            {
              NoPrompt = true
            });
          this.ChangeState(InPlaceHostingManager.State.VerifyRequirementsSucceeded);
        }
        catch (Exception exception_0)
        {
          Logger.AddInternalState(this._log, "Exception thrown in AssertApplicationRequirements(bool grantApplicationTrust): " + exception_0.GetType().ToString() + " : " + exception_0.Message + "\r\n" + exception_0.StackTrace);
          this.ChangeState(InPlaceHostingManager.State.Done);
          throw;
        }
      }
    }

    /// <summary>Downloads an application update in the background.</summary>
    /// <exception cref="T:System.InvalidOperationException">Raised if this method is called before the <see cref="M:System.Deployment.Application.InPlaceHostingManager.GetManifestAsync" /> and <see cref="M:System.Deployment.Application.InPlaceHostingManager.AssertApplicationRequirements" /> methods.</exception>
    public void DownloadApplicationAsync()
    {
      lock (this._lock)
      {
        try
        {
          Logger.AddMethodCall(this._log, "InPlaceHostingManager.DownloadApplicationAsync() called.");
          if (this._appType == AppType.CustomHostSpecified)
            this.AssertState(InPlaceHostingManager.State.GetManifestSucceeded);
          else if (this._isCached)
            this.AssertState(InPlaceHostingManager.State.GetManifestSucceeded, InPlaceHostingManager.State.VerifyRequirementsSucceeded);
          else
            this.AssertState(InPlaceHostingManager.State.GetManifestSucceeded, InPlaceHostingManager.State.VerifyRequirementsSucceeded);
          this.ChangeState(InPlaceHostingManager.State.DownloadingApplication);
          this._deploymentManager.SynchronizeAsync();
        }
        catch (Exception exception_0)
        {
          Logger.AddInternalState(this._log, "Exception thrown DownloadApplicationAsync(): " + exception_0.GetType().ToString() + " : " + exception_0.Message + "\r\n" + exception_0.StackTrace);
          this.ChangeState(InPlaceHostingManager.State.Done);
          throw;
        }
      }
    }

    /// <summary>Launches the ClickOnce application, if and only if it is a Windows Presentation Foundation-based application running in a Web browser.</summary>
    /// <returns>An <see cref="T:System.Runtime.Remoting.ObjectHandle" /> corresponding to the launched application.</returns>
    public ObjectHandle Execute()
    {
      lock (this._lock)
      {
        try
        {
          Logger.AddMethodCall(this._log, "InPlaceHostingManager.Execute() called.");
          this.AssertState(InPlaceHostingManager.State.DownloadApplicationSucceeded);
          this.ChangeState(InPlaceHostingManager.State.Done);
          return this._deploymentManager.ExecuteNewDomain();
        }
        catch (Exception exception_0)
        {
          Logger.AddInternalState(this._log, "Exception thrown in Execute(): " + exception_0.GetType().ToString() + " : " + exception_0.Message + "\r\n" + exception_0.StackTrace);
          throw;
        }
      }
    }

    /// <summary>Cancels an asynchronous download operation.</summary>
    public void CancelAsync()
    {
      lock (this._lock)
      {
        try
        {
          Logger.AddMethodCall(this._log, "InPlaceHostingManager.CancelAsync() called.");
          this.ChangeState(InPlaceHostingManager.State.Done);
          this._deploymentManager.CancelAsync();
        }
        catch (Exception exception_0)
        {
          Logger.AddInternalState(this._log, "Exception thrown in CancelAsync(): " + exception_0.GetType().ToString() + " : " + exception_0.Message + "\r\n" + exception_0.StackTrace);
          throw;
        }
      }
    }

    /// <summary>Releases all resources used by the <see cref="T:System.Deployment.Application.InPlaceHostingManager" />. </summary>
    public void Dispose()
    {
      lock (this._lock)
      {
        try
        {
          Logger.AddMethodCall(this._log, "InPlaceHostingManager.Dispose() called.");
          this.ChangeState(InPlaceHostingManager.State.Done);
          this._deploymentManager.BindCompleted -= new BindCompletedEventHandler(this.OnBindCompleted);
          this._deploymentManager.SynchronizeCompleted -= new SynchronizeCompletedEventHandler(this.OnSynchronizeCompleted);
          this._deploymentManager.ProgressChanged -= new DeploymentProgressChangedEventHandler(this.OnProgressChanged);
          this._deploymentManager.Dispose();
          GC.SuppressFinalize((object) this);
        }
        catch (Exception exception_0)
        {
          Logger.AddInternalState(this._log, "Exception thrown in Dispose(): " + exception_0.GetType().ToString() + " : " + exception_0.Message + "\r\n" + exception_0.StackTrace);
          throw;
        }
      }
    }

    /// <summary>Removes a ClickOnce application that includes the &lt;customUX&gt; element.</summary>
    /// <param name="subscriptionId">A string that contains a subscription identifier, which indicates the ClickOnce application to remove.</param>
    public static void UninstallCustomUXApplication(string subscriptionId)
    {
      DefinitionIdentity subIdAndValidate = InPlaceHostingManager.GetSubIdAndValidate(subscriptionId);
      SubscriptionStore currentUser = SubscriptionStore.CurrentUser;
      currentUser.RefreshStorePointer();
      SubscriptionState subscriptionState = currentUser.GetSubscriptionState(subIdAndValidate);
      subscriptionState.SubscriptionStore.UninstallCustomUXSubscription(subscriptionState);
    }

    /// <summary>Removes a previously installed user-defined component of an application.</summary>
    /// <param name="subscriptionId">A string that contains a subscription identifier, which indicates the add-in to remove.</param>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="subscriptionId" /> is not a valid subscription identity, or does not include a name, public key token, processor architecture, and version number.</exception>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="subscriptionId" /> is null.</exception>
    public static void UninstallCustomAddIn(string subscriptionId)
    {
      DefinitionIdentity subIdAndValidate = InPlaceHostingManager.GetSubIdAndValidate(subscriptionId);
      SubscriptionStore currentUser = SubscriptionStore.CurrentUser;
      currentUser.RefreshStorePointer();
      SubscriptionState subscriptionState = currentUser.GetSubscriptionState(subIdAndValidate);
      subscriptionState.SubscriptionStore.UninstallCustomHostSpecifiedSubscription(subscriptionState);
    }

    private static DefinitionIdentity GetSubIdAndValidate(string subscriptionId)
    {
      if (subscriptionId == null)
        throw new ArgumentNullException("subscriptionId", Resources.GetString("Ex_ComArgSubIdentityNull"));
      DefinitionIdentity definitionIdentity;
      try
      {
        definitionIdentity = new DefinitionIdentity(subscriptionId);
      }
      catch (COMException ex)
      {
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.GetString("Ex_ComArgSubIdentityNotValid"), new object[1]
        {
          (object) subscriptionId
        }), (Exception) ex);
      }
      catch (SEHException ex)
      {
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.GetString("Ex_ComArgSubIdentityNotValid"), new object[1]
        {
          (object) subscriptionId
        }), (Exception) ex);
      }
      catch (ArgumentException ex)
      {
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.GetString("Ex_ComArgSubIdentityNotValid"), new object[1]
        {
          (object) subscriptionId
        }), (Exception) ex);
      }
      if (definitionIdentity.Name == null)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.GetString("Ex_ComArgSubIdentityNotValid"), new object[1]
        {
          (object) subscriptionId
        }));
      if (definitionIdentity.PublicKeyToken == null)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.GetString("Ex_ComArgSubIdentityNotValid"), new object[1]
        {
          (object) subscriptionId
        }));
      if (definitionIdentity.ProcessorArchitecture == null)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.GetString("Ex_ComArgSubIdentityNotValid"), new object[1]
        {
          (object) subscriptionId
        }));
      if (definitionIdentity.Version != (Version) null)
        throw new ArgumentException(Resources.GetString("Ex_ComArgSubIdentityWithVersion"));
      return definitionIdentity;
    }

    private void OnBindCompleted(object sender, BindCompletedEventArgs e)
    {
      lock (this._lock)
      {
        GetManifestCompletedEventArgs local_2_1;
        try
        {
          this.AssertState(InPlaceHostingManager.State.GettingManifest, InPlaceHostingManager.State.Done);
          if (this._state != InPlaceHostingManager.State.Done)
          {
            if (e.Cancelled || e.Error != null)
              this.ChangeState(InPlaceHostingManager.State.Done);
            else
              this.ChangeState(InPlaceHostingManager.State.GetManifestSucceeded, (AsyncCompletedEventArgs) e);
          }
          // ISSUE: reference to a compiler-generated field
          if (this.GetManifestCompleted == null)
            return;
          if (e.Error != null || e.Cancelled)
          {
            if (e.Cancelled)
              Logger.AddInternalState(this._log, "GetManifestAsync call cancelled.");
            local_2_1 = new GetManifestCompletedEventArgs(e, this._deploymentManager.LogFilePath);
          }
          else
          {
            this._isCached = e.IsCached;
            bool local_3 = this._deploymentManager.ActivationDescription.DeployManifest.Deployment.Install;
            bool local_4 = this._deploymentManager.ActivationDescription.AppManifest.EntryPoints[0].HostInBrowser;
            this._appType = this._deploymentManager.ActivationDescription.appType;
            bool local_5 = this._deploymentManager.ActivationDescription.AppManifest.UseManifestForTrust;
            Uri local_6 = this._deploymentManager.ActivationDescription.DeployManifest.Deployment.ProviderCodebaseUri;
            local_2_1 = !this._isLaunchInHostProcess || this._appType == AppType.CustomHostSpecified || local_4 ? (!local_3 || !this._isLaunchInHostProcess && this._appType != AppType.CustomHostSpecified ? (!local_5 || this._appType != AppType.CustomHostSpecified ? (!(local_6 != (Uri) null) || this._appType != AppType.CustomHostSpecified ? (!local_4 || this._appType != AppType.CustomUX ? new GetManifestCompletedEventArgs(e, this._deploymentManager.ActivationDescription, this._deploymentManager.LogFilePath, this._log) : new GetManifestCompletedEventArgs(e, (Exception) new InvalidOperationException(Resources.GetString("Ex_CannotHaveCustomUXFlag")), this._deploymentManager.LogFilePath)) : new GetManifestCompletedEventArgs(e, (Exception) new InvalidOperationException(Resources.GetString("Ex_CannotHaveDeploymentProvider")), this._deploymentManager.LogFilePath)) : new GetManifestCompletedEventArgs(e, (Exception) new InvalidOperationException(Resources.GetString("Ex_CannotHaveUseManifestForTrustFlag")), this._deploymentManager.LogFilePath)) : new GetManifestCompletedEventArgs(e, (Exception) new InvalidOperationException(Resources.GetString("Ex_InstallFlagMustBeFalse")), this._deploymentManager.LogFilePath)) : new GetManifestCompletedEventArgs(e, (Exception) new InvalidOperationException(Resources.GetString("Ex_HostInBrowserFlagMustBeTrue")), this._deploymentManager.LogFilePath);
            if (local_2_1.Error != null)
              Logger.AddInternalState(this._log, "Exception thrown after binding: " + local_2_1.Error.GetType().ToString() + " : " + local_2_1.Error.Message + "\r\n" + local_2_1.Error.StackTrace);
          }
        }
        catch (Exception exception_0)
        {
          Logger.AddInternalState(this._log, "Exception thrown:" + exception_0.GetType().ToString() + " : " + exception_0.Message);
          this.ChangeState(InPlaceHostingManager.State.Done);
          throw;
        }
        // ISSUE: reference to a compiler-generated field
        this.GetManifestCompleted((object) this, local_2_1);
      }
    }

    private void OnSynchronizeCompleted(object sender, SynchronizeCompletedEventArgs e)
    {
      lock (this._lock)
      {
        try
        {
          this.AssertState(InPlaceHostingManager.State.DownloadingApplication, InPlaceHostingManager.State.VerifyRequirementsSucceeded, InPlaceHostingManager.State.Done);
          if (this._state != InPlaceHostingManager.State.Done)
          {
            if (e.Cancelled || e.Error != null)
              this.ChangeState(InPlaceHostingManager.State.Done);
            else
              this.ChangeState(InPlaceHostingManager.State.DownloadApplicationSucceeded, (AsyncCompletedEventArgs) e);
          }
          if ((!this._isLaunchInHostProcess || this._appType == AppType.CustomHostSpecified) && this._appType != AppType.CustomUX)
            this.ChangeState(InPlaceHostingManager.State.Done);
          // ISSUE: reference to a compiler-generated field
          if (this.DownloadApplicationCompleted == null)
            return;
          // ISSUE: reference to a compiler-generated field
          this.DownloadApplicationCompleted((object) this, new DownloadApplicationCompletedEventArgs((AsyncCompletedEventArgs) e, this._deploymentManager.LogFilePath, this._deploymentManager.ShortcutAppId));
        }
        catch (Exception exception_0)
        {
          Logger.AddInternalState(this._log, "Exception thrown:" + exception_0.GetType().ToString() + " : " + exception_0.Message);
          throw;
        }
      }
    }

    private void OnProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
    {
      lock (this._lock)
      {
        // ISSUE: reference to a compiler-generated field
        if (this.DownloadProgressChanged == null)
          return;
        // ISSUE: reference to a compiler-generated field
        this.DownloadProgressChanged((object) this, new DownloadProgressChangedEventArgs(e.ProgressPercentage, e.UserState, e.BytesCompleted, e.BytesTotal, e.State));
      }
    }

    private void AssertState(InPlaceHostingManager.State validState)
    {
      if (this._state == InPlaceHostingManager.State.Done)
        throw new InvalidOperationException(Resources.GetString("Ex_NoFurtherOperations"));
      if (validState != this._state)
        throw new InvalidOperationException(Resources.GetString("Ex_InvalidSequence"));
    }

    private void AssertState(InPlaceHostingManager.State validState0, InPlaceHostingManager.State validState1)
    {
      if (this._state == InPlaceHostingManager.State.Done && validState0 != this._state && validState1 != this._state)
        throw new InvalidOperationException(Resources.GetString("Ex_NoFurtherOperations"));
      if (validState0 != this._state && validState1 != this._state)
        throw new InvalidOperationException(Resources.GetString("Ex_InvalidSequence"));
    }

    private void AssertState(InPlaceHostingManager.State validState0, InPlaceHostingManager.State validState1, InPlaceHostingManager.State validState2)
    {
      if (this._state == InPlaceHostingManager.State.Done && validState0 != this._state && (validState1 != this._state && validState2 != this._state))
        throw new InvalidOperationException(Resources.GetString("Ex_NoFurtherOperations"));
      if (validState0 != this._state && validState1 != this._state && validState2 != this._state)
        throw new InvalidOperationException(Resources.GetString("Ex_InvalidSequence"));
    }

    private void ChangeState(InPlaceHostingManager.State nextState, AsyncCompletedEventArgs e)
    {
      if (e.Cancelled || e.Error != null)
        this._state = InPlaceHostingManager.State.Done;
      else
        this._state = nextState;
    }

    private void ChangeState(InPlaceHostingManager.State nextState)
    {
      this._state = nextState;
      Logger.AddInternalState(this._log, "Internal state=" + (object) this._state);
    }

    private enum State
    {
      Ready,
      GettingManifest,
      GetManifestSucceeded,
      VerifyingRequirements,
      VerifyRequirementsSucceeded,
      DownloadingApplication,
      DownloadApplicationSucceeded,
      Done,
    }
  }
}
