// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.ApplicationActivator
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Collections;
using System.Deployment.Application.Manifest;
using System.Deployment.Internal;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;

namespace System.Deployment.Application
{
  internal class ApplicationActivator
  {
    private static Hashtable _activationsInProgress = new Hashtable();
    private static int _liveActivationLimitUIStatus = 0;
    private bool _remActivationInProgressEntry;
    private SubscriptionStore _subStore;
    private UserInterface _ui;
    private bool _fullTrust;
    private const int _liveActivationLimitUINotVisible = 0;
    private const int _liveActivationLimitUIVisible = 1;
    private const int ActivateArgumentCount = 5;

    private void DisplayActivationFailureReason(Exception exception, string errorPageUrl)
    {
      string message = Resources.GetString("ErrorMessage_GenericActivationFailure");
      string linkUrlMessage = Resources.GetString("ErrorMessage_GenericLinkUrlMessage");
      Exception innerMostException = this.GetInnerMostException(exception);
      if (exception is DeploymentDownloadException)
      {
        message = Resources.GetString("ErrorMessage_NetworkError");
        if (((DeploymentException) exception).SubType == ExceptionTypes.SizeLimitForPartialTrustOnlineAppExceeded)
          message = Resources.GetString("ErrorMessage_SizeLimitForPartialTrustOnlineAppExceeded");
        if (innerMostException is WebException)
        {
          WebException webException = (WebException) innerMostException;
          if (webException.Response != null && webException.Response is HttpWebResponse)
          {
            HttpWebResponse response = (HttpWebResponse) webException.Response;
            if (response.StatusCode == HttpStatusCode.NotFound)
              message = Resources.GetString("ErrorMessage_FileMissing");
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
              message = Resources.GetString("ErrorMessage_AuthenticationError");
            else if (response.StatusCode == HttpStatusCode.Forbidden)
              message = Resources.GetString("ErrorMessage_Forbidden");
          }
        }
        else if (innerMostException is FileNotFoundException || innerMostException is DirectoryNotFoundException)
          message = Resources.GetString("ErrorMessage_FileMissing");
        else if (innerMostException is UnauthorizedAccessException)
          message = Resources.GetString("ErrorMessage_AuthenticationError");
        else if (innerMostException is IOException && !this.IsWebExceptionInExceptionStack(exception))
          message = Resources.GetString("ErrorMessage_DownloadIOError");
      }
      else if (exception is InvalidDeploymentException)
      {
        InvalidDeploymentException deploymentException = (InvalidDeploymentException) exception;
        if (deploymentException.SubType == ExceptionTypes.ManifestLoad)
          message = Resources.GetString("ErrorMessage_ManifestCannotBeLoaded");
        else if (deploymentException.SubType == ExceptionTypes.Manifest || deploymentException.SubType == ExceptionTypes.ManifestParse || deploymentException.SubType == ExceptionTypes.ManifestSemanticValidation)
          message = Resources.GetString("ErrorMessage_InvalidManifest");
        else if (deploymentException.SubType == ExceptionTypes.Validation || deploymentException.SubType == ExceptionTypes.HashValidation || (deploymentException.SubType == ExceptionTypes.SignatureValidation || deploymentException.SubType == ExceptionTypes.RefDefValidation) || (deploymentException.SubType == ExceptionTypes.ClrValidation || deploymentException.SubType == ExceptionTypes.StronglyNamedAssemblyVerification || (deploymentException.SubType == ExceptionTypes.IdentityMatchValidationForMixedModeAssembly || deploymentException.SubType == ExceptionTypes.AppFileLocationValidation)) || deploymentException.SubType == ExceptionTypes.FileSizeValidation)
          message = Resources.GetString("ErrorMessage_ValidationFailed");
        else if (deploymentException.SubType == ExceptionTypes.UnsupportedElevetaionRequest)
          message = Resources.GetString("ErrorMessage_ManifestExecutionLevelNotSupported");
      }
      else if (exception is DeploymentException)
      {
        if (((DeploymentException) exception).SubType == ExceptionTypes.ComponentStore)
          message = Resources.GetString("ErrorMessage_StoreError");
        else if (((DeploymentException) exception).SubType == ExceptionTypes.ActivationLimitExceeded)
          message = Resources.GetString("ErrorMessage_ConcurrentActivationLimitExceeded");
        else if (((DeploymentException) exception).SubType == ExceptionTypes.DiskIsFull)
          message = Resources.GetString("ErrorMessage_DiskIsFull");
        else if (((DeploymentException) exception).SubType == ExceptionTypes.DeploymentUriDifferent)
          message = exception.Message;
        else if (((DeploymentException) exception).SubType == ExceptionTypes.GroupMultipleMatch)
          message = exception.Message;
        else if (((DeploymentException) exception).SubType == ExceptionTypes.TrustFailDependentPlatform)
          message = exception.Message;
      }
      string logFileLocation = Logger.GetLogFilePath();
      if (!Logger.FlushCurrentThreadLogs())
        logFileLocation = (string) null;
      string linkUrl = (string) null;
      if (errorPageUrl != null)
      {
        linkUrl = string.Format("{0}?outer={1}&&inner={2}&&msg={3}", new object[4]
        {
          (object) errorPageUrl,
          (object) exception.GetType().ToString(),
          (object) innerMostException.GetType().ToString(),
          (object) innerMostException.Message
        });
        if (linkUrl.Length > 2048)
          linkUrl = linkUrl.Substring(0, 2048);
      }
      this._ui.ShowError(Resources.GetString("UI_ErrorTitle"), message, logFileLocation, linkUrl, linkUrlMessage);
    }

    private void DisplayPlatformDetectionFailureUI(DependentPlatformMissingException ex)
    {
      Uri supportUrl = (Uri) null;
      if (this._fullTrust)
        supportUrl = ex.SupportUrl;
      this._ui.ShowPlatform(ex.Message, supportUrl);
    }

    public void ActivateDeployment(string activationUrl, bool isShortcut)
    {
      LifetimeManager.StartOperation();
      bool flag = false;
      try
      {
        flag = ThreadPool.QueueUserWorkItem(new WaitCallback(this.ActivateDeploymentWorker), (object) new object[5]
        {
          (object) activationUrl,
          (object) isShortcut,
          (object) null,
          (object) null,
          (object) null
        });
        if (!flag)
          throw new OutOfMemoryException();
      }
      finally
      {
        if (!flag)
          LifetimeManager.EndOperation();
      }
    }

    public void ActivateDeploymentEx(string activationUrl, int unsignedPolicy, int signedPolicy)
    {
      LifetimeManager.StartOperation();
      bool flag = false;
      try
      {
        flag = ThreadPool.QueueUserWorkItem(new WaitCallback(this.ActivateDeploymentWorker), (object) new object[5]
        {
          (object) activationUrl,
          (object) false,
          (object) null,
          (object) null,
          (object) new ApplicationActivator.BrowserSettings()
          {
            ManagedSignedFlag = ApplicationActivator.BrowserSettings.GetManagedFlagValue(signedPolicy),
            ManagedUnSignedFlag = ApplicationActivator.BrowserSettings.GetManagedFlagValue(unsignedPolicy)
          }
        });
        if (!flag)
          throw new OutOfMemoryException();
      }
      finally
      {
        if (!flag)
          LifetimeManager.EndOperation();
      }
    }

    public void ActivateApplicationExtension(string textualSubId, string deploymentProviderUrl, string targetAssociatedFile)
    {
      LifetimeManager.StartOperation();
      bool flag = false;
      try
      {
        flag = ThreadPool.QueueUserWorkItem(new WaitCallback(this.ActivateDeploymentWorker), (object) new object[5]
        {
          (object) targetAssociatedFile,
          (object) false,
          (object) textualSubId,
          (object) deploymentProviderUrl,
          (object) null
        });
        if (!flag)
          throw new OutOfMemoryException();
      }
      finally
      {
        if (!flag)
          LifetimeManager.EndOperation();
      }
    }

    private void ActivateDeploymentWorker(object state)
    {
      string str = (string) null;
      string textualSubId = (string) null;
      string deploymentProviderUrlFromExtension = (string) null;
      try
      {
        CodeMarker_Singleton.Instance.CodeMarker(525);
        object[] objArray = (object[]) state;
        str = (string) objArray[0] ?? string.Empty;
        Logger.StartCurrentThreadLogging();
        Logger.SetSubscriptionUrl(str);
        Logger.AddInternalState("Activation through dfsvc.exe started.");
        Logger.AddMethodCall("ActivateDeploymentWorker({0},{1},{2},{3},{4}) called.", objArray);
        bool isShortcut = (bool) objArray[1];
        if (objArray[2] != null)
          textualSubId = (string) objArray[2];
        if (objArray[3] != null)
          deploymentProviderUrlFromExtension = (string) objArray[3];
        ApplicationActivator.BrowserSettings browserSettings = (ApplicationActivator.BrowserSettings) null;
        if (objArray[4] != null)
          browserSettings = (ApplicationActivator.BrowserSettings) objArray[4];
        string errorPageUrl = (string) null;
        try
        {
          int num = this.CheckActivationInProgress(str);
          this._ui = new UserInterface(false);
          if (!PolicyKeys.SuppressLimitOnNumberOfActivations() && num > 8)
            throw new DeploymentException(ExceptionTypes.ActivationLimitExceeded, Resources.GetString("Ex_TooManyLiveActivation"));
          if (str.Length > 16384)
            throw new DeploymentException(ExceptionTypes.Activation, Resources.GetString("Ex_UrlTooLong"));
          Uri uri = new Uri(str);
          try
          {
            UriHelper.ValidateSupportedSchemeInArgument(uri, "activationUrl");
          }
          catch (ArgumentException ex)
          {
            throw new InvalidDeploymentException(ExceptionTypes.UriSchemeNotSupported, Resources.GetString("Ex_NotSupportedUriScheme"), (Exception) ex);
          }
          Logger.AddPhaseInformation(Resources.GetString("PhaseLog_StartOfActivation"), new object[1]{ (object) str });
          this.PerformDeploymentActivation(uri, isShortcut, textualSubId, deploymentProviderUrlFromExtension, browserSettings, ref errorPageUrl);
          Logger.AddPhaseInformation(Resources.GetString("ActivateManifestSucceeded"), new object[1]{ (object) str });
        }
        catch (DependentPlatformMissingException ex)
        {
          Logger.AddErrorInformation((Exception) ex, Resources.GetString("ActivateManifestException"), new object[1]{ (object) str });
          if (this._ui == null)
            this._ui = new UserInterface();
          if (this._ui.SplashCancelled())
            return;
          this.DisplayPlatformDetectionFailureUI(ex);
        }
        catch (DownloadCancelledException ex)
        {
          Logger.AddErrorInformation((Exception) ex, Resources.GetString("ActivateManifestException"), new object[1]{ (object) str });
        }
        catch (TrustNotGrantedException ex)
        {
          Logger.AddErrorInformation((Exception) ex, Resources.GetString("ActivateManifestException"), new object[1]{ (object) str });
        }
        catch (DeploymentException ex)
        {
          Logger.AddErrorInformation((Exception) ex, Resources.GetString("ActivateManifestException"), new object[1]{ (object) str });
          if (ex.SubType == ExceptionTypes.ActivationInProgress)
            return;
          if (this._ui == null)
            this._ui = new UserInterface();
          if (this._ui.SplashCancelled())
            return;
          if (ex.SubType == ExceptionTypes.ActivationLimitExceeded)
          {
            if (Interlocked.CompareExchange(ref ApplicationActivator._liveActivationLimitUIStatus, 1, 0) != 0)
              return;
            this.DisplayActivationFailureReason((Exception) ex, errorPageUrl);
            Interlocked.CompareExchange(ref ApplicationActivator._liveActivationLimitUIStatus, 0, 1);
          }
          else
            this.DisplayActivationFailureReason((Exception) ex, errorPageUrl);
        }
        catch (Exception ex)
        {
          if (ex is AccessViolationException || ex is OutOfMemoryException)
            throw;
          else if (PolicyKeys.DisableGenericExceptionHandler())
          {
            throw;
          }
          else
          {
            Logger.AddErrorInformation(ex, Resources.GetString("ActivateManifestException"), new object[1]{ (object) str });
            if (this._ui == null)
              this._ui = new UserInterface();
            if (this._ui.SplashCancelled())
              return;
            this.DisplayActivationFailureReason(ex, errorPageUrl);
          }
        }
      }
      finally
      {
        this.RemoveActivationInProgressEntry(str);
        if (this._ui != null)
        {
          this._ui.Dispose();
          this._ui = (UserInterface) null;
        }
        CodeMarker_Singleton.Instance.CodeMarker(526);
        Logger.EndCurrentThreadLogging();
        LifetimeManager.EndOperation();
      }
    }

    private void PerformDeploymentActivation(Uri activationUri, bool isShortcut, string textualSubId, string deploymentProviderUrlFromExtension, ApplicationActivator.BrowserSettings browserSettings, ref string errorPageUrl)
    {
      TempFile tempFile = (TempFile) null;
      Logger.AddMethodCall("PerformDeploymentActivation called.");
      try
      {
        string str = (string) null;
        Uri uri = (Uri) null;
        bool flag1 = false;
        this._subStore = SubscriptionStore.CurrentUser;
        this._subStore.RefreshStorePointer();
        Uri sourceUri = activationUri;
        bool flag2 = false;
        ActivationDescription actDesc;
        if (textualSubId != null)
        {
          Logger.AddInternalState("Activating through file association.");
          flag2 = true;
          actDesc = this.ProcessOrFollowExtension(activationUri, textualSubId, deploymentProviderUrlFromExtension, ref errorPageUrl, out tempFile);
          if (actDesc == null)
            return;
        }
        else if (isShortcut)
        {
          Logger.AddInternalState("Activating through shortcut.");
          str = activationUri.LocalPath;
          actDesc = this.ProcessOrFollowShortcut(str, ref errorPageUrl, out tempFile);
          if (actDesc == null)
            return;
        }
        else
        {
          Logger.AddInternalState("Activating through deployment manifest.");
          Logger.AddInternalState("Start processing deployment manifest.");
          SubscriptionState subState;
          AssemblyManifest assemblyManifest = DownloadManager.DownloadDeploymentManifestBypass(this._subStore, ref sourceUri, out tempFile, out subState, (IDownloadNotification) null, (DownloadOptions) null);
          if (browserSettings != null && tempFile != null)
            browserSettings.Validate(tempFile.Path);
          if (assemblyManifest.Description != null)
            errorPageUrl = assemblyManifest.Description.ErrorReportUrl;
          actDesc = new ActivationDescription();
          if (subState != null)
          {
            str = (string) null;
            actDesc.SetApplicationManifest(subState.CurrentApplicationManifest, (Uri) null, (string) null);
            actDesc.AppId = subState.CurrentBind;
            Logger.AddInternalState("Running from the store. Bypass further downloads and verifications.");
            flag1 = true;
          }
          else
            str = tempFile.Path;
          Logger.SetDeploymentManifest(assemblyManifest);
          Logger.AddPhaseInformation(Resources.GetString("PhaseLog_ProcessingDeploymentManifestComplete"));
          Logger.AddInternalState("Processing of deployment manifest has successfully completed.");
          actDesc.SetDeploymentManifest(assemblyManifest, sourceUri, str);
          actDesc.IsUpdate = false;
          actDesc.ActType = ActivationType.InstallViaDotApplication;
          uri = activationUri;
        }
        if (this._ui.SplashCancelled())
          throw new DownloadCancelledException();
        if (actDesc.DeployManifest.Deployment == null)
          throw new DeploymentException(ExceptionTypes.Activation, Resources.GetString("Ex_NotDeploymentOrShortcut"));
        bool flag3 = false;
        SubscriptionState subscriptionState = this._subStore.GetSubscriptionState(actDesc.DeployManifest);
        this.CheckDeploymentProviderValidity(actDesc, subscriptionState);
        if (!flag1)
        {
          Logger.AddInternalState("Could not find application in store. Continue with downloading application manifest.");
          flag3 = this.InstallApplication(ref subscriptionState, actDesc);
          Logger.AddPhaseInformation(Resources.GetString("PhaseLog_InstallationComplete"));
          Logger.AddInternalState("Installation of application has successfully completed.");
        }
        else
          this._subStore.SetLastCheckTimeToNow(subscriptionState);
        if (actDesc.DeployManifest.Deployment.DisallowUrlActivation && !isShortcut && (!activationUri.IsFile || activationUri.IsUnc))
        {
          if (flag3)
            this._ui.ShowMessage(Resources.GetString("Activation_DisallowUrlActivationMessageAfterInstall"), Resources.GetString("Activation_DisallowUrlActivationCaptionAfterInstall"));
          else
            this._ui.ShowMessage(Resources.GetString("Activation_DisallowUrlActivationMessage"), Resources.GetString("Activation_DisallowUrlActivationCaption"));
        }
        else if (flag2)
          this.Activate(actDesc.AppId, actDesc.AppManifest, activationUri.AbsoluteUri, true);
        else if (isShortcut)
        {
          string activationParameter = (string) null;
          int num = str.IndexOf('|', 0);
          if (num > 0 && num + 1 < str.Length)
            activationParameter = str.Substring(num + 1);
          if (activationParameter == null)
            this.Activate(actDesc.AppId, actDesc.AppManifest, (string) null, false);
          else
            this.Activate(actDesc.AppId, actDesc.AppManifest, activationParameter, true);
        }
        else
          this.Activate(actDesc.AppId, actDesc.AppManifest, uri.AbsoluteUri, false);
      }
      finally
      {
        if (tempFile != null)
          tempFile.Dispose();
      }
    }

    private ActivationDescription ProcessOrFollowExtension(Uri associatedFile, string textualSubId, string deploymentProviderUrlFromExtension, ref string errorPageUrl, out TempFile deployFile)
    {
      deployFile = (TempFile) null;
      Logger.AddMethodCall("ProcessOrFollowExtension(" + (object) associatedFile + "," + textualSubId + "," + deploymentProviderUrlFromExtension + "," + errorPageUrl + ") called.");
      SubscriptionState subscriptionState = this._subStore.GetSubscriptionState(new DefinitionIdentity(textualSubId));
      ActivationDescription activationDescription = (ActivationDescription) null;
      if (subscriptionState.IsInstalled && subscriptionState.IsShellVisible)
      {
        Logger.AddInternalState("Application family is already installed and Shell Visible.");
        this.PerformDeploymentUpdate(ref subscriptionState, ref errorPageUrl);
        this.Activate(subscriptionState.CurrentBind, subscriptionState.CurrentApplicationManifest, associatedFile.AbsoluteUri, true);
      }
      else
      {
        Logger.AddInternalState("Application family is not installed or is not Shell-Visible.  Try to deploy it from the deployment provider specified in the extension : " + deploymentProviderUrlFromExtension);
        if (string.IsNullOrEmpty(deploymentProviderUrlFromExtension))
          throw new DeploymentException(ExceptionTypes.Activation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_FileAssociationNoDpUrl"), new object[1]
          {
            (object) textualSubId
          }));
        Uri sourceUri = new Uri(deploymentProviderUrlFromExtension);
        Logger.AddInternalState("Start processing deployment manifest.");
        AssemblyManifest manifest = DownloadManager.DownloadDeploymentManifest(this._subStore, ref sourceUri, out deployFile);
        if (manifest.Description != null)
          errorPageUrl = manifest.Description.ErrorReportUrl;
        Logger.AddInternalState("Processing of deployment manifest has successfully completed.");
        if (!manifest.Deployment.Install)
          throw new DeploymentException(ExceptionTypes.Activation, Resources.GetString("Ex_FileAssociationRefOnline"));
        activationDescription = new ActivationDescription();
        activationDescription.SetDeploymentManifest(manifest, sourceUri, deployFile.Path);
        activationDescription.IsUpdate = false;
        activationDescription.ActType = ActivationType.InstallViaFileAssociation;
      }
      return activationDescription;
    }

    private ActivationDescription ProcessOrFollowShortcut(string shortcutFile, ref string errorPageUrl, out TempFile deployFile)
    {
      deployFile = (TempFile) null;
      Logger.AddMethodCall("ProcessOrFollowShortcut(shortcutFile=" + shortcutFile + ",errorPageUrl=" + errorPageUrl + ") called.");
      string shortcutFile1 = shortcutFile;
      string activationParameter = (string) null;
      int length = shortcutFile.IndexOf('|', 0);
      if (length > 0)
      {
        shortcutFile1 = shortcutFile.Substring(0, length);
        if (length + 1 < shortcutFile.Length)
          activationParameter = shortcutFile.Substring(length + 1);
      }
      Logger.AddInternalState("shortcutParameter=" + activationParameter);
      DefinitionIdentity subId;
      Uri providerUri;
      ShellExposure.ParseAppShortcut(shortcutFile1, out subId, out providerUri);
      SubscriptionState subscriptionState = this._subStore.GetSubscriptionState(subId);
      ActivationDescription activationDescription = (ActivationDescription) null;
      if (subscriptionState.IsInstalled && subscriptionState.IsShellVisible)
      {
        Logger.AddInternalState("Application family is already installed and Shell Visible.");
        this.PerformDeploymentUpdate(ref subscriptionState, ref errorPageUrl);
        if (activationParameter == null)
          this.Activate(subscriptionState.CurrentBind, subscriptionState.CurrentApplicationManifest, (string) null, false);
        else
          this.Activate(subscriptionState.CurrentBind, subscriptionState.CurrentApplicationManifest, activationParameter, true);
      }
      else
      {
        Uri sourceUri = providerUri;
        Logger.AddInternalState("Application family is not installed or is not Shell-Visible.  Try to deploy it from the deployment provider specified in the shortcut : " + (object) sourceUri);
        Logger.AddInternalState("Start processing deployment manifest.");
        AssemblyManifest manifest = DownloadManager.DownloadDeploymentManifest(this._subStore, ref sourceUri, out deployFile);
        Logger.AddInternalState("Processing of deployment manifest has successfully completed.");
        if (manifest.Description != null)
          errorPageUrl = manifest.Description.ErrorReportUrl;
        if (!manifest.Deployment.Install)
          throw new DeploymentException(ExceptionTypes.Activation, Resources.GetString("Ex_ShortcutRefOnlineOnly"));
        activationDescription = new ActivationDescription();
        activationDescription.SetDeploymentManifest(manifest, sourceUri, deployFile.Path);
        activationDescription.IsUpdate = false;
        activationDescription.ActType = ActivationType.InstallViaShortcut;
      }
      return activationDescription;
    }

    private void Activate(DefinitionAppId appId, AssemblyManifest appManifest, string activationParameter, bool useActivationParameter)
    {
      using (ActivationContext activationContext = ActivationContext.CreatePartialActivationContext(appId.ToApplicationIdentity()))
      {
        InternalActivationContextHelper.PrepareForExecution(activationContext);
        this._subStore.ActivateApplication(appId, activationParameter, useActivationParameter);
      }
    }

    private void PerformDeploymentUpdate(ref SubscriptionState subState, ref string errorPageUrl)
    {
      DeploymentUpdate deploymentUpdate = subState.CurrentDeploymentManifest.Deployment.DeploymentUpdate;
      bool flag = deploymentUpdate != null && deploymentUpdate.BeforeApplicationStartup;
      Logger.AddPhaseInformation(Resources.GetString("PhaseLog_DeploymentUpdateCheck"));
      Logger.AddMethodCall("PerformDeploymentUpdate called.");
      Logger.AddInternalState("UpdateOnStart=" + flag.ToString() + ",PendingDeployment=" + (object) subState.PendingDeployment);
      if (!flag && (subState.PendingDeployment == null || ApplicationActivator.SkipUpdate(subState, subState.PendingDeployment)))
        return;
      TempFile tempFile = (TempFile) null;
      try
      {
        Uri deploymentProviderUri = subState.DeploymentProviderUri;
        AssemblyManifest assemblyManifest;
        try
        {
          Logger.AddInternalState("Start processing deployment manifest for update check : " + (object) deploymentProviderUri);
          assemblyManifest = DownloadManager.DownloadDeploymentManifest(this._subStore, ref deploymentProviderUri, out tempFile);
          Logger.AddInternalState("End processing deployment manifest.");
          if (assemblyManifest.Description != null)
            errorPageUrl = assemblyManifest.Description.ErrorReportUrl;
        }
        catch (DeploymentDownloadException ex)
        {
          Logger.AddErrorInformation((Exception) ex, Resources.GetString("Upd_UpdateCheckDownloadFailed"), new object[1]
          {
            (object) subState.SubscriptionId.ToString()
          });
          return;
        }
        if (this._ui.SplashCancelled())
          throw new DownloadCancelledException();
        if (ApplicationActivator.SkipUpdate(subState, assemblyManifest.Identity) || !(this._subStore.CheckUpdateInManifest(subState, deploymentProviderUri, assemblyManifest, subState.CurrentDeployment.Version) != (Version) null) || assemblyManifest.Identity.Equals((object) subState.ExcludedDeployment))
          return;
        Logger.AddInternalState("Update available in the deployment server.");
        ActivationDescription actDesc = new ActivationDescription();
        actDesc.SetDeploymentManifest(assemblyManifest, deploymentProviderUri, tempFile.Path);
        actDesc.IsUpdate = true;
        actDesc.IsRequiredUpdate = false;
        actDesc.ActType = ActivationType.UpdateViaShortcutOrFA;
        if (assemblyManifest.Deployment.MinimumRequiredVersion != (Version) null && assemblyManifest.Deployment.MinimumRequiredVersion.CompareTo(subState.CurrentDeployment.Version) > 0)
          actDesc.IsRequiredUpdate = true;
        this.CheckDeploymentProviderValidity(actDesc, subState);
        this.ConsumeUpdatedDeployment(ref subState, actDesc);
      }
      finally
      {
        if (tempFile != null)
          tempFile.Dispose();
      }
    }

    private void CheckDeploymentProviderValidity(ActivationDescription actDesc, SubscriptionState subState)
    {
      if (!actDesc.DeployManifest.Deployment.Install || !(actDesc.DeployManifest.Deployment.ProviderCodebaseUri == (Uri) null) || (subState == null || !(subState.DeploymentProviderUri != (Uri) null)))
        return;
      Uri uri = subState.DeploymentProviderUri.Query == null || subState.DeploymentProviderUri.Query.Length <= 0 ? subState.DeploymentProviderUri : new Uri(subState.DeploymentProviderUri.GetLeftPart(UriPartial.Path));
      Logger.AddInternalState("Checking deployment provider validity.");
      Logger.AddInternalState("providerCodebaseUri=" + (object) uri);
      Logger.AddInternalState("actDesc.ToAppCodebase()=" + actDesc.ToAppCodebase());
      if (!uri.Equals((object) actDesc.ToAppCodebase()))
        throw new DeploymentException(ExceptionTypes.DeploymentUriDifferent, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("ErrorMessage_DeploymentUriDifferent"), new object[1]
        {
          (object) actDesc.DeployManifest.Description.FilteredProduct
        }), (Exception) new DeploymentException(ExceptionTypes.DeploymentUriDifferent, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_DeploymentUriDifferentExText"), new object[3]
        {
          (object) actDesc.DeployManifest.Description.FilteredProduct,
          (object) actDesc.DeploySourceUri.AbsoluteUri,
          (object) subState.DeploymentProviderUri.AbsoluteUri
        })));
    }

    private void ConsumeUpdatedDeployment(ref SubscriptionState subState, ActivationDescription actDesc)
    {
      DefinitionIdentity identity = actDesc.DeployManifest.Identity;
      Uri deploySourceUri = actDesc.DeploySourceUri;
      Logger.AddPhaseInformation(Resources.GetString("PhaseLog_ConsumeUpdatedDeployment"));
      Logger.AddInternalState("Consuming new update.");
      if (!actDesc.IsRequiredUpdate)
      {
        Logger.AddInternalState("Update is not a required update.");
        Description effectiveDescription = subState.EffectiveDescription;
        switch (this._ui.ShowUpdate(new UserInterfaceInfo()
        {
          formTitle = Resources.GetString("UI_UpdateTitle"),
          productName = effectiveDescription.Product,
          supportUrl = effectiveDescription.SupportUrl,
          sourceSite = UserInterface.GetDisplaySite(deploySourceUri)
        }))
        {
          case UserInterfaceModalResult.Skip:
            DateTime updateSkipTime = DateTime.UtcNow + new TimeSpan(7, 0, 0, 0);
            this._subStore.SetUpdateSkipTime(subState, identity, updateSkipTime);
            Logger.AddPhaseInformation(Resources.GetString("Upd_DeployUpdateSkipping"));
            Logger.AddInternalState("User has decided to skip the update.");
            return;
          case UserInterfaceModalResult.Cancel:
            Logger.AddInternalState("Do not update now, but prompt for update on next activation.");
            return;
        }
      }
      this.InstallApplication(ref subState, actDesc);
      Logger.AddPhaseInformation(Resources.GetString("Upd_Consumed"), (object) identity.ToString(), (object) deploySourceUri);
      Logger.AddInternalState("Update consumed.");
    }

    private bool InstallApplication(ref SubscriptionState subState, ActivationDescription actDesc)
    {
      bool flag = false;
      Logger.AddMethodCall("InstallApplication called.");
      Logger.AddPhaseInformation(Resources.GetString("PhaseLog_InstallApplication"));
      this._subStore.CheckDeploymentSubscriptionState(subState, actDesc.DeployManifest);
      long transactionId;
      using (this._subStore.AcquireReferenceTransaction(out transactionId))
      {
        TempDirectory downloadTemp = (TempDirectory) null;
        try
        {
          flag = this.DownloadApplication(subState, actDesc, transactionId, out downloadTemp);
          actDesc.CommitDeploy = true;
          actDesc.IsConfirmed = true;
          actDesc.TimeStamp = DateTime.UtcNow;
          if (actDesc.CommitApp)
            this.SetMarkOfTheWebIfNeeded((CommitApplicationParams) actDesc);
          Logger.AddPhaseInformation(Resources.GetString("PhaseLog_CommitApplication"));
          this._subStore.CommitApplication(ref subState, (CommitApplicationParams) actDesc);
        }
        finally
        {
          if (downloadTemp != null)
            downloadTemp.Dispose();
        }
      }
      return flag;
    }

    private void SetMarkOfTheWebIfNeeded(CommitApplicationParams p)
    {
      string deployManifestPath = p.DeployManifestPath;
      string absoluteUri = p.AppSourceUri.AbsoluteUri;
      if (p.AppPayloadPath == null)
        return;
      string path = Path.Combine(p.AppPayloadPath, p.AppId.ApplicationIdentity.Name);
      if (!System.IO.File.Exists(path) || !PlatformDetector.IsWin8orLater() || (!p.Trust.DefaultGrantSet.PermissionSet.IsUnrestricted() || AssemblyManifest.AnalyzeManifestCertificate(deployManifestPath) == AssemblyManifest.CertificateStatus.TrustedPublisher) || !Utilities.IsAppRepCheckRequired(absoluteUri))
        return;
      Utilities.SetMarkOfTheWeb(path);
    }

    private bool DownloadApplication(SubscriptionState subState, ActivationDescription actDesc, long transactionId, out TempDirectory downloadTemp)
    {
      bool flag = false;
      Logger.AddMethodCall("DownloadApplication called.");
      downloadTemp = this._subStore.AcquireTempDirectory();
      Logger.AddInternalState("Start processing application manifest.");
      Uri appSourceUri;
      string appManifestPath;
      AssemblyManifest assemblyManifest = DownloadManager.DownloadApplicationManifest(actDesc.DeployManifest, downloadTemp.Path, actDesc.DeploySourceUri, out appSourceUri, out appManifestPath);
      AssemblyManifest.ReValidateManifestSignatures(actDesc.DeployManifest, assemblyManifest);
      if (assemblyManifest.EntryPoints[0].HostInBrowser)
        throw new DeploymentException(ExceptionTypes.ManifestSemanticValidation, Resources.GetString("Ex_HostInBrowserAppNotSupported"));
      if (assemblyManifest.EntryPoints[0].CustomHostSpecified)
        throw new DeploymentException(ExceptionTypes.ManifestSemanticValidation, Resources.GetString("Ex_CustomHostSpecifiedAppNotSupported"));
      if (assemblyManifest.EntryPoints[0].CustomUX && (actDesc.ActType == ActivationType.InstallViaDotApplication || actDesc.ActType == ActivationType.InstallViaFileAssociation || (actDesc.ActType == ActivationType.InstallViaShortcut || actDesc.ActType == ActivationType.None)))
        throw new DeploymentException(ExceptionTypes.ManifestSemanticValidation, Resources.GetString("Ex_CustomUXAppNotSupported"));
      Logger.AddPhaseInformation(Resources.GetString("PhaseLog_ProcessingApplicationManifestComplete"));
      Logger.AddInternalState("Processing of application manifest has successfully completed.");
      actDesc.SetApplicationManifest(assemblyManifest, appSourceUri, appManifestPath);
      Logger.SetApplicationManifest(assemblyManifest);
      this._subStore.CheckCustomUXFlag(subState, actDesc.AppManifest);
      actDesc.AppId = new DefinitionAppId(actDesc.ToAppCodebase(), new DefinitionIdentity[2]
      {
        actDesc.DeployManifest.Identity,
        actDesc.AppManifest.Identity
      });
      Logger.AddInternalState("Start request of trust and detection of platform.");
      if (assemblyManifest.EntryPoints[0].CustomUX)
      {
        Logger.AddInternalState("This is a CustomUX application. Calling PersistTrustWithoutEvaluation.");
        actDesc.Trust = ApplicationTrust.PersistTrustWithoutEvaluation(actDesc.ToActivationContext());
      }
      else
      {
        this._ui.Hide();
        if (this._ui.SplashCancelled())
          throw new DownloadCancelledException();
        if (subState.IsInstalled && !string.Equals(subState.EffectiveCertificatePublicKeyToken, actDesc.EffectiveCertificatePublicKeyToken, StringComparison.Ordinal))
        {
          Logger.AddInternalState("EffectiveCertificatePublicKeyToken has changed between versions: subState.EffectiveCertificatePublicKeyToken=" + subState.EffectiveCertificatePublicKeyToken + ",actDesc.EffectiveCertificatePublicKeyToken=" + actDesc.EffectiveCertificatePublicKeyToken);
          Logger.AddInternalState("Removing the cached trust decision for CurrentBind.");
          ApplicationTrust.RemoveCachedTrust(subState.CurrentBind);
        }
        try
        {
          actDesc.Trust = ApplicationTrust.RequestTrust(subState, actDesc.DeployManifest.Deployment.Install, actDesc.IsUpdate, actDesc.ToActivationContext());
        }
        catch (Exception ex1)
        {
          Logger.AddErrorInformation(Resources.GetString("Ex_DetermineTrustFailed"), ex1);
          if (!(ex1 is TrustNotGrantedException))
          {
            try
            {
              PlatformDetector.VerifyPlatformDependencies(actDesc.AppManifest, actDesc.DeployManifest, downloadTemp.Path);
            }
            catch (Exception ex2)
            {
              if (ex2 is DependentPlatformMissingException)
                throw new DeploymentException(ExceptionTypes.TrustFailDependentPlatform, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("ErrorMessage_TrustFailDependentPlatformMissing"), new object[1]
                {
                  (object) ex2.Message
                }), ex1);
            }
          }
          throw;
        }
      }
      this._fullTrust = actDesc.Trust.DefaultGrantSet.PermissionSet.IsUnrestricted();
      Logger.AddInternalState("_fullTrust = " + this._fullTrust.ToString());
      if (!this._fullTrust && actDesc.AppManifest.FileAssociations.Length != 0)
        throw new DeploymentException(ExceptionTypes.ManifestSemanticValidation, Resources.GetString("Ex_FileExtensionNotSupported"));
      PlatformDetector.VerifyPlatformDependencies(actDesc.AppManifest, actDesc.DeployManifest, downloadTemp.Path);
      Logger.AddPhaseInformation(Resources.GetString("PhaseLog_PlatformDetectAndTrustGrantComplete"));
      Logger.AddInternalState("Request of trust and detection of platform is complete.");
      Logger.AddInternalState("Start downloading  and verifying dependencies.");
      if (!this._subStore.CheckAndReferenceApplication(subState, actDesc.AppId, transactionId))
      {
        flag = true;
        Description effectiveDescription = actDesc.EffectiveDescription;
        UserInterfaceInfo info = new UserInterfaceInfo();
        info.productName = effectiveDescription.Product;
        if (actDesc.IsUpdate)
        {
          if (actDesc.IsRequiredUpdate)
            info.formTitle = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("UI_ProgressTitleRequiredUpdate"), new object[1]
            {
              (object) info.productName
            });
          else
            info.formTitle = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("UI_ProgressTitleUpdate"), new object[1]
            {
              (object) info.productName
            });
        }
        else if (!actDesc.DeployManifest.Deployment.Install)
          info.formTitle = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("UI_ProgressTitleDownload"), new object[1]
          {
            (object) info.productName
          });
        else
          info.formTitle = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("UI_ProgressTitleInstall"), new object[1]
          {
            (object) info.productName
          });
        info.supportUrl = effectiveDescription.SupportUrl;
        info.sourceSite = UserInterface.GetDisplaySite(actDesc.DeploySourceUri);
        if (assemblyManifest.Description != null && assemblyManifest.Description.IconFileFS != null)
          info.iconFilePath = Path.Combine(downloadTemp.Path, assemblyManifest.Description.IconFileFS);
        ProgressPiece progressPiece = this._ui.ShowProgress(info);
        DownloadOptions options = (DownloadOptions) null;
        if (!this._fullTrust & !actDesc.DeployManifest.Deployment.Install)
        {
          options = new DownloadOptions();
          options.EnforceSizeLimit = true;
          options.SizeLimit = this._subStore.GetSizeLimitInBytesForSemiTrustApps();
          options.Size = actDesc.DeployManifest.SizeInBytes + actDesc.AppManifest.SizeInBytes;
        }
        DownloadManager.DownloadDependencies(subState, actDesc.DeployManifest, actDesc.AppManifest, actDesc.AppSourceUri, downloadTemp.Path, (string) null, (IDownloadNotification) progressPiece, options);
        Logger.AddPhaseInformation(Resources.GetString("PhaseLog_DownloadDependenciesComplete"));
        actDesc.CommitApp = true;
        actDesc.AppPayloadPath = downloadTemp.Path;
        actDesc.AppGroup = (string) null;
      }
      return flag;
    }

    private static bool SkipUpdate(SubscriptionState subState, DefinitionIdentity targetIdentity)
    {
      Logger.AddMethodCall("SkipUpdate called.");
      if (subState.UpdateSkippedDeployment != null && targetIdentity != null && (subState.UpdateSkippedDeployment.Equals((object) targetIdentity) && subState.UpdateSkipTime > DateTime.UtcNow))
      {
        Logger.AddInternalState("Skipped Update. UpdateSkipTime was " + (object) subState.UpdateSkipTime);
        return true;
      }
      Logger.AddInternalState("Update is not skipped.");
      return false;
    }

    private Exception GetInnerMostException(Exception exception)
    {
      if (exception.InnerException != null)
        return this.GetInnerMostException(exception.InnerException);
      return exception;
    }

    private bool IsWebExceptionInExceptionStack(Exception exception)
    {
      if (exception == null)
        return false;
      if (exception is WebException)
        return true;
      return this.IsWebExceptionInExceptionStack(exception.InnerException);
    }

    private int CheckActivationInProgress(string activationUrl)
    {
      lock (ApplicationActivator._activationsInProgress.SyncRoot)
      {
        if (ApplicationActivator._activationsInProgress.Contains((object) activationUrl))
        {
          ((ApplicationActivator) ApplicationActivator._activationsInProgress[(object) activationUrl]).ActivateUI();
          this._remActivationInProgressEntry = false;
          throw new DeploymentException(ExceptionTypes.ActivationInProgress, Resources.GetString("Ex_ActivationInProgressException"));
        }
        ApplicationActivator._activationsInProgress.Add((object) activationUrl, (object) this);
        this._remActivationInProgressEntry = true;
        return ApplicationActivator._activationsInProgress.Count;
      }
    }

    private void RemoveActivationInProgressEntry(string activationUrl)
    {
      if (!this._remActivationInProgressEntry || activationUrl == null)
        return;
      lock (ApplicationActivator._activationsInProgress.SyncRoot)
        ApplicationActivator._activationsInProgress.Remove((object) activationUrl);
    }

    private void ActivateUI()
    {
      if (this._ui == null)
        return;
      this._ui.Activate();
    }

    private class BrowserSettings
    {
      public ApplicationActivator.BrowserSettings.ManagedFlags ManagedSignedFlag = ApplicationActivator.BrowserSettings.ManagedFlags.URLPOLICY_DISALLOW;
      public ApplicationActivator.BrowserSettings.ManagedFlags ManagedUnSignedFlag = ApplicationActivator.BrowserSettings.ManagedFlags.URLPOLICY_DISALLOW;

      public void Validate(string manifestPath)
      {
        Logger.AddMethodCall("BrowserSettings.Validate(" + manifestPath + ") called.");
        switch (AssemblyManifest.AnalyzeManifestCertificate(manifestPath))
        {
          case AssemblyManifest.CertificateStatus.TrustedPublisher:
          case AssemblyManifest.CertificateStatus.AuthenticodedNotInTrustedList:
            if (this.ManagedSignedFlag != ApplicationActivator.BrowserSettings.ManagedFlags.URLPOLICY_ALLOW && this.ManagedSignedFlag != ApplicationActivator.BrowserSettings.ManagedFlags.URLPOLICY_QUERY)
              throw new InvalidDeploymentException(ExceptionTypes.Manifest, Resources.GetString("Ex_SignedManifestDisallow"));
            break;
          default:
            if (this.ManagedUnSignedFlag != ApplicationActivator.BrowserSettings.ManagedFlags.URLPOLICY_ALLOW && this.ManagedUnSignedFlag != ApplicationActivator.BrowserSettings.ManagedFlags.URLPOLICY_QUERY)
              throw new InvalidDeploymentException(ExceptionTypes.Manifest, Resources.GetString("Ex_UnSignedManifestDisallow"));
            break;
        }
        Logger.AddInternalState("Browser settings allow activation. ManagedSignedFlag=" + (object) this.ManagedSignedFlag + ",ManagedUnSignedFlag=" + (object) this.ManagedUnSignedFlag);
      }

      public static ApplicationActivator.BrowserSettings.ManagedFlags GetManagedFlagValue(int policyValue)
      {
        switch (policyValue)
        {
          case 0:
            return ApplicationActivator.BrowserSettings.ManagedFlags.URLPOLICY_ALLOW;
          case 1:
            return ApplicationActivator.BrowserSettings.ManagedFlags.URLPOLICY_QUERY;
          case 3:
            return ApplicationActivator.BrowserSettings.ManagedFlags.URLPOLICY_DISALLOW;
          default:
            return ApplicationActivator.BrowserSettings.ManagedFlags.URLPOLICY_DISALLOW;
        }
      }

      public enum ManagedFlags
      {
        URLPOLICY_ALLOW = 0,
        URLPOLICY_QUERY = 1,
        URLPOLICY_DISALLOW = 3,
      }
    }
  }
}
