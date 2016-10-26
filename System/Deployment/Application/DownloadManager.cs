// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.DownloadManager
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using Microsoft.Win32;
using System.Deployment.Application.Manifest;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;

namespace System.Deployment.Application
{
  internal static class DownloadManager
  {
    private static X509Certificate2 ClientCertificate;

    public static AssemblyManifest DownloadDeploymentManifest(SubscriptionStore subStore, ref Uri sourceUri, out TempFile tempFile)
    {
      return DownloadManager.DownloadDeploymentManifest(subStore, ref sourceUri, out tempFile, (IDownloadNotification) null, (DownloadOptions) null);
    }

    public static AssemblyManifest DownloadDeploymentManifest(SubscriptionStore subStore, ref Uri sourceUri, out TempFile tempFile, IDownloadNotification notification, DownloadOptions options)
    {
      Logger.AddMethodCall("DownloadDeploymentManifest called.");
      Logger.AddInternalState("SourceUri=" + (object) sourceUri);
      Logger.AddInternalState("DownloadOptions=" + (options != null ? options.ToString() : "null"));
      tempFile = (TempFile) null;
      TempFile tempFile1 = (TempFile) null;
      TempFile tempFile2 = (TempFile) null;
      DownloadManager.ClientCertificate = (X509Certificate2) null;
      AssemblyManifest deployment;
      try
      {
        ServerInformation serverInformation;
        deployment = DownloadManager.DownloadDeploymentManifestDirect(subStore, ref sourceUri, out tempFile1, notification, options, out serverInformation);
        Logger.SetSubscriptionServerInformation(serverInformation);
        bool flag = DownloadManager.FollowDeploymentProviderUri(subStore, ref deployment, ref sourceUri, out tempFile2, notification, options);
        tempFile = flag ? tempFile2 : tempFile1;
      }
      finally
      {
        if (tempFile1 != null && tempFile1 != tempFile)
          tempFile1.Dispose();
        if (tempFile2 != null && tempFile2 != tempFile)
          tempFile2.Dispose();
      }
      return deployment;
    }

    public static bool FollowDeploymentProviderUri(SubscriptionStore subStore, ref AssemblyManifest deployment, ref Uri sourceUri, out TempFile tempFile, IDownloadNotification notification, DownloadOptions options)
    {
      Logger.AddMethodCall("FollowDeploymentProviderUri called.");
      tempFile = (TempFile) null;
      bool flag1 = false;
      Zone fromUrl1 = Zone.CreateFromUrl(sourceUri.AbsoluteUri);
      bool flag2 = false;
      if (fromUrl1.SecurityZone != SecurityZone.MyComputer)
      {
        Logger.AddInternalState("Deployment manifest zone is not local machine. Zone = " + (object) fromUrl1.SecurityZone);
        flag2 = true;
      }
      else
      {
        Logger.AddInternalState("Deployment manifest zone is local machine. Zone = " + (object) fromUrl1.SecurityZone);
        DependentAssembly dependentAssembly = deployment.MainDependentAssembly;
        if (dependentAssembly == null || dependentAssembly.Codebase == null)
          throw new InvalidDeploymentException(ExceptionTypes.ManifestSemanticValidation, Resources.GetString("Ex_NoAppInDeploymentManifest"));
        Uri uri = new Uri(sourceUri, dependentAssembly.Codebase);
        Zone fromUrl2 = Zone.CreateFromUrl(uri.AbsoluteUri);
        if (fromUrl2.SecurityZone == SecurityZone.MyComputer)
        {
          Logger.AddInternalState("Application manifest zone is local machine. Zone = " + (object) fromUrl2.SecurityZone);
          if (!System.IO.File.Exists(uri.LocalPath))
          {
            Logger.AddInternalState(uri.LocalPath + " does not exist in local machine.");
            flag2 = true;
          }
        }
      }
      if (flag2)
      {
        Uri providerCodebaseUri = deployment.Deployment.ProviderCodebaseUri;
        Logger.SetDeploymentProviderUrl(providerCodebaseUri);
        Logger.AddInternalState("providerUri=" + (object) providerCodebaseUri + ",sourceUri=" + (object) sourceUri);
        if (!PolicyKeys.SkipDeploymentProvider() && providerCodebaseUri != (Uri) null && !providerCodebaseUri.Equals((object) sourceUri))
        {
          ServerInformation serverInformation;
          AssemblyManifest deployment1;
          try
          {
            deployment1 = DownloadManager.DownloadDeploymentManifestDirect(subStore, ref providerCodebaseUri, out tempFile, notification, options, out serverInformation);
          }
          catch (InvalidDeploymentException ex)
          {
            if (ex.SubType == ExceptionTypes.Manifest || ex.SubType == ExceptionTypes.ManifestLoad || (ex.SubType == ExceptionTypes.ManifestParse || ex.SubType == ExceptionTypes.ManifestSemanticValidation))
              throw new InvalidDeploymentException(ExceptionTypes.Manifest, Resources.GetString("Ex_InvalidProviderManifest"), (Exception) ex);
            throw;
          }
          Logger.SetDeploymentProviderServerInformation(serverInformation);
          SubscriptionState subscriptionState = subStore.GetSubscriptionState(deployment);
          if (!subStore.GetSubscriptionState(deployment1).SubscriptionId.Equals((object) subscriptionState.SubscriptionId))
            throw new InvalidDeploymentException(ExceptionTypes.SubscriptionSemanticValidation, Resources.GetString("Ex_ProviderNotInSubscription"));
          Logger.AddInternalState("Deployment provider followed: " + (object) providerCodebaseUri);
          deployment = deployment1;
          sourceUri = providerCodebaseUri;
          flag1 = true;
        }
      }
      if (!flag1)
        Logger.AddInternalState("Deployment provider not followed.");
      return flag1;
    }

    public static AssemblyManifest DownloadDeploymentManifestBypass(SubscriptionStore subStore, ref Uri sourceUri, out TempFile tempFile, out SubscriptionState subState, IDownloadNotification notification, DownloadOptions options)
    {
      Logger.AddMethodCall("DownloadDeploymentManifestBypass called.");
      tempFile = (TempFile) null;
      subState = (SubscriptionState) null;
      TempFile tempFile1 = (TempFile) null;
      TempFile tempFile2 = (TempFile) null;
      DownloadManager.ClientCertificate = (X509Certificate2) null;
      AssemblyManifest deployment;
      try
      {
        ServerInformation serverInformation;
        deployment = DownloadManager.DownloadDeploymentManifestDirectBypass(subStore, ref sourceUri, out tempFile1, out subState, notification, options, out serverInformation);
        Logger.SetSubscriptionServerInformation(serverInformation);
        if (subState != null)
        {
          tempFile = tempFile1;
          return deployment;
        }
        bool flag = DownloadManager.FollowDeploymentProviderUri(subStore, ref deployment, ref sourceUri, out tempFile2, notification, options);
        tempFile = flag ? tempFile2 : tempFile1;
      }
      finally
      {
        if (tempFile1 != null && tempFile1 != tempFile)
          tempFile1.Dispose();
        if (tempFile2 != null && tempFile2 != tempFile)
          tempFile2.Dispose();
      }
      return deployment;
    }

    public static AssemblyManifest DownloadApplicationManifest(AssemblyManifest deploymentManifest, string targetDir, Uri deploymentUri, out Uri appSourceUri, out string appManifestPath)
    {
      return DownloadManager.DownloadApplicationManifest(deploymentManifest, targetDir, deploymentUri, (IDownloadNotification) null, (DownloadOptions) null, out appSourceUri, out appManifestPath);
    }

    public static AssemblyManifest DownloadApplicationManifest(AssemblyManifest deploymentManifest, string targetDir, Uri deploymentUri, IDownloadNotification notification, DownloadOptions options, out Uri appSourceUri, out string appManifestPath)
    {
      Logger.AddMethodCall("DownloadApplicationManifest called.");
      DependentAssembly dependentAssembly = deploymentManifest.MainDependentAssembly;
      if (dependentAssembly == null || dependentAssembly.Codebase == null)
        throw new InvalidDeploymentException(ExceptionTypes.ManifestSemanticValidation, Resources.GetString("Ex_NoAppInDeploymentManifest"));
      appSourceUri = new Uri(deploymentUri, dependentAssembly.Codebase);
      Zone fromUrl1 = Zone.CreateFromUrl(deploymentUri.AbsoluteUri);
      Zone fromUrl2 = Zone.CreateFromUrl(appSourceUri.AbsoluteUri);
      if (!fromUrl1.Equals((object) fromUrl2))
      {
        Logger.AddInternalState("Deployment and application does not have matching security zones. deploymentZone=" + (object) fromUrl1 + ",applicationZone=" + (object) fromUrl2);
        throw new InvalidDeploymentException(ExceptionTypes.Zone, Resources.GetString("Ex_DeployAppZoneMismatch"));
      }
      appManifestPath = Path.Combine(targetDir, dependentAssembly.Identity.Name + ".manifest");
      ServerInformation serverInformation;
      AssemblyManifest assemblyManifest = DownloadManager.DownloadManifest(ref appSourceUri, appManifestPath, notification, options, AssemblyManifest.ManifestType.Application, out serverInformation);
      Logger.SetApplicationUrl(appSourceUri);
      Logger.SetApplicationServerInformation(serverInformation);
      Zone fromUrl3 = Zone.CreateFromUrl(appSourceUri.AbsoluteUri);
      if (!fromUrl1.Equals((object) fromUrl3))
      {
        Logger.AddInternalState("Deployment and application does not have matching security zones. deploymentZone=" + (object) fromUrl1 + ",applicationZone=" + (object) fromUrl3);
        throw new InvalidDeploymentException(ExceptionTypes.Zone, Resources.GetString("Ex_DeployAppZoneMismatch"));
      }
      if (assemblyManifest.Identity.Equals((object) deploymentManifest.Identity))
        throw new InvalidDeploymentException(ExceptionTypes.ManifestSemanticValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_DepSameDeploymentAndApplicationIdentity"), new object[1]
        {
          (object) assemblyManifest.Identity.ToString()
        }));
      if (!assemblyManifest.Identity.Matches(dependentAssembly.Identity, assemblyManifest.Application))
        throw new InvalidDeploymentException(ExceptionTypes.SubscriptionSemanticValidation, Resources.GetString("Ex_RefDefMismatch"));
      if (!PolicyKeys.SkipApplicationDependencyHashCheck())
      {
        try
        {
          ComponentVerifier.VerifyFileHash(appManifestPath, dependentAssembly.HashCollection);
        }
        catch (InvalidDeploymentException ex)
        {
          if (ex.SubType == ExceptionTypes.HashValidation)
            throw new InvalidDeploymentException(ExceptionTypes.HashValidation, Resources.GetString("Ex_AppManInvalidHash"), (Exception) ex);
          throw;
        }
      }
      if (assemblyManifest.RequestedExecutionLevel != null)
      {
        Logger.AddInternalState("Application manifest has RequestedExecutionLevel specified. Check requested privileges.");
        DownloadManager.VerifyRequestedPrivilegesSupport(assemblyManifest.RequestedExecutionLevel);
      }
      return assemblyManifest;
    }

    public static void DownloadDependencies(SubscriptionState subState, AssemblyManifest deployManifest, AssemblyManifest appManifest, Uri sourceUriBase, string targetDirectory, string group, IDownloadNotification notification, DownloadOptions options)
    {
      Logger.AddMethodCall("DownloadDependencies called.");
      Logger.AddInternalState("sourceUriBase=" + (object) sourceUriBase);
      Logger.AddInternalState("targetDirectory=" + targetDirectory);
      Logger.AddInternalState("group=" + group);
      Logger.AddInternalState("DownloadOptions=" + (object) options);
      FileDownloader downloader = FileDownloader.Create();
      downloader.Options = options;
      if (group == null)
        downloader.CheckForSizeLimit(appManifest.CalculateDependenciesSize(), false);
      DownloadManager.AddDependencies(downloader, deployManifest, appManifest, sourceUriBase, targetDirectory, group);
      downloader.DownloadModified += new FileDownloader.DownloadModifiedEventHandler(DownloadManager.ProcessDownloadedFile);
      if (notification != null)
        downloader.AddNotification(notification);
      try
      {
        downloader.Download(subState, DownloadManager.ClientCertificate);
        downloader.ComponentVerifier.VerifyComponents();
        DownloadManager.VerifyRequestedPrivilegesSupport(appManifest, targetDirectory);
      }
      finally
      {
        if (notification != null)
          downloader.RemoveNotification(notification);
        downloader.DownloadModified -= new FileDownloader.DownloadModifiedEventHandler(DownloadManager.ProcessDownloadedFile);
      }
    }

    private static void VerifyRequestedPrivilegesSupport(AssemblyManifest appManifest, string targetDirectory)
    {
      if (appManifest.EntryPoints[0].CustomHostSpecified)
        return;
      string str = Path.Combine(targetDirectory, appManifest.EntryPoints[0].Assembly.Codebase);
      if (System.IO.File.Exists(str))
      {
        AssemblyManifest assemblyManifest = new AssemblyManifest(str);
        if (!assemblyManifest.Id1ManifestPresent || assemblyManifest.Id1RequestedExecutionLevel == null)
          return;
        DownloadManager.VerifyRequestedPrivilegesSupport(assemblyManifest.Id1RequestedExecutionLevel);
      }
      else
        Logger.AddInternalState("Main exe=" + str + " does not exist. No Requested Priviliges Verification done.");
    }

    private static void VerifyRequestedPrivilegesSupport(string requestedExecutionLevel)
    {
      Logger.AddMethodCall("VerifyRequestedPrivilegesSupport(" + requestedExecutionLevel + ") called.");
      if (!PlatformSpecific.OnVistaOrAbove)
        return;
      bool flag = false;
      RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
      if (registryKey != null && registryKey.GetValue("EnableLUA") != null)
      {
        Logger.AddInternalState("LUA policy key = " + registryKey.Name);
        if ((int) registryKey.GetValue("EnableLUA") != 0)
        {
          flag = true;
          Logger.AddInternalState("LUA is enabled.");
        }
      }
      if (flag && (string.Compare(requestedExecutionLevel, "requireAdministrator", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(requestedExecutionLevel, "highestAvailable", StringComparison.OrdinalIgnoreCase) == 0))
        throw new InvalidDeploymentException(ExceptionTypes.UnsupportedElevetaionRequest, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_ManifestExecutionLevelNotSupported"), new object[0]));
    }

    private static AssemblyManifest DownloadDeploymentManifestDirect(SubscriptionStore subStore, ref Uri sourceUri, out TempFile tempFile, IDownloadNotification notification, DownloadOptions options, out ServerInformation serverInformation)
    {
      Logger.AddMethodCall("DownloadDeploymentManifestDirect(" + (object) sourceUri + ") called.");
      tempFile = subStore.AcquireTempFile(".application");
      AssemblyManifest assemblyManifest = DownloadManager.DownloadManifest(ref sourceUri, tempFile.Path, notification, options, AssemblyManifest.ManifestType.Deployment, out serverInformation);
      if (assemblyManifest.Identity.Version == (Version) null)
        throw new InvalidDeploymentException(ExceptionTypes.ManifestSemanticValidation, Resources.GetString("Ex_DeploymentManifestNoVersion"));
      if (assemblyManifest.Deployment == null)
        throw new InvalidDeploymentException(ExceptionTypes.ManifestSemanticValidation, Resources.GetString("Ex_InvalidDeploymentManifest"));
      return assemblyManifest;
    }

    private static AssemblyManifest DownloadDeploymentManifestDirectBypass(SubscriptionStore subStore, ref Uri sourceUri, out TempFile tempFile, out SubscriptionState subState, IDownloadNotification notification, DownloadOptions options, out ServerInformation serverInformation)
    {
      Logger.AddMethodCall("DownloadDeploymentManifestDirectBypass called.");
      subState = (SubscriptionState) null;
      tempFile = subStore.AcquireTempFile(".application");
      DownloadManager.DownloadManifestAsRawFile(ref sourceUri, tempFile.Path, notification, options, out serverInformation);
      bool flag1 = false;
      AssemblyManifest deployment = (AssemblyManifest) null;
      DefinitionAppId appId = (DefinitionAppId) null;
      try
      {
        deployment = ManifestReader.FromDocumentNoValidation(tempFile.Path);
        DefinitionIdentity identity = deployment.Identity;
        DefinitionIdentity definitionIdentity = new DefinitionIdentity(deployment.MainDependentAssembly.Identity);
        appId = new DefinitionAppId((sourceUri.Query == null || sourceUri.Query.Length <= 0 ? sourceUri : new Uri(sourceUri.GetLeftPart(UriPartial.Path))).AbsoluteUri, new DefinitionIdentity[2]
        {
          identity,
          definitionIdentity
        });
        Logger.AddInternalState("expectedAppId=" + appId.ToString());
      }
      catch (InvalidDeploymentException ex)
      {
        flag1 = true;
      }
      catch (COMException ex)
      {
        flag1 = true;
      }
      catch (SEHException ex)
      {
        flag1 = true;
      }
      catch (IndexOutOfRangeException ex)
      {
        flag1 = true;
      }
      if (!flag1)
      {
        SubscriptionState subscriptionState = subStore.GetSubscriptionState(deployment);
        bool flag2 = false;
        long transactionId;
        using (subStore.AcquireReferenceTransaction(out transactionId))
          flag2 = subStore.CheckAndReferenceApplication(subscriptionState, appId, transactionId);
        if (!flag2 || !appId.Equals((object) subscriptionState.CurrentBind))
        {
          if (flag2)
            Logger.AddInternalState("Application is found in store and but it is not the CurrentBind.");
        }
        else
        {
          Logger.AddInternalState("Application is found in store and it is the CurrentBind, bypass validation and further downloads.");
          subState = subscriptionState;
          return subState.CurrentDeploymentManifest;
        }
      }
      else
        Logger.AddInternalState("Application is not found in store.");
      Logger.AddInternalState("Reparse the deployment manifest for validations.");
      AssemblyManifest assemblyManifest = ManifestReader.FromDocument(tempFile.Path, AssemblyManifest.ManifestType.Deployment, sourceUri);
      if (assemblyManifest.Identity.Version == (Version) null)
        throw new InvalidDeploymentException(ExceptionTypes.ManifestSemanticValidation, Resources.GetString("Ex_DeploymentManifestNoVersion"));
      if (assemblyManifest.Deployment == null)
        throw new InvalidDeploymentException(ExceptionTypes.ManifestSemanticValidation, Resources.GetString("Ex_InvalidDeploymentManifest"));
      return assemblyManifest;
    }

    private static AssemblyManifest DownloadManifest(ref Uri sourceUri, string targetPath, IDownloadNotification notification, DownloadOptions options, AssemblyManifest.ManifestType manifestType, out ServerInformation serverInformation)
    {
      Logger.AddMethodCall("DownloadManifest called.");
      DownloadManager.DownloadManifestAsRawFile(ref sourceUri, targetPath, notification, options, out serverInformation);
      return ManifestReader.FromDocument(targetPath, manifestType, sourceUri);
    }

    private static void DownloadManifestAsRawFile(ref Uri sourceUri, string targetPath, IDownloadNotification notification, DownloadOptions options, out ServerInformation serverInformation)
    {
      Logger.AddMethodCall("DownloadManifestAsRawFile called.");
      FileDownloader fileDownloader = FileDownloader.Create();
      fileDownloader.Options = options;
      if (notification != null)
        fileDownloader.AddNotification(notification);
      try
      {
        fileDownloader.AddFile(sourceUri, targetPath, 16777216);
        fileDownloader.Download((SubscriptionState) null, DownloadManager.ClientCertificate);
        sourceUri = fileDownloader.DownloadResults[0].ResponseUri;
        serverInformation = fileDownloader.DownloadResults[0].ServerInformation;
        DownloadManager.ClientCertificate = fileDownloader.ClientCertificate;
      }
      finally
      {
        if (notification != null)
          fileDownloader.RemoveNotification(notification);
      }
    }

    private static void AddDependencies(FileDownloader downloader, AssemblyManifest deployManifest, AssemblyManifest appManifest, Uri sourceUriBase, string targetDirectory, string group)
    {
      long total = 0;
      System.Deployment.Application.Manifest.File[] filesInGroup = appManifest.GetFilesInGroup(group, true);
      DownloadManager.ReorderFilesForIconFile(appManifest, filesInGroup);
      foreach (System.Deployment.Application.Manifest.File file in filesInGroup)
      {
        Uri fileSourceUri = DownloadManager.MapFileSourceUri(deployManifest, sourceUriBase, file.Name);
        DownloadManager.AddFileToDownloader(downloader, deployManifest, appManifest, (object) file, fileSourceUri, targetDirectory, file.NameFS, file.HashCollection);
        total += (long) file.Size;
      }
      DependentAssembly[] assembliesInGroup = appManifest.GetPrivateAssembliesInGroup(group, true);
      foreach (DependentAssembly dependentAssembly in assembliesInGroup)
      {
        Uri fileSourceUri = DownloadManager.MapFileSourceUri(deployManifest, sourceUriBase, dependentAssembly.Codebase);
        DownloadManager.AddFileToDownloader(downloader, deployManifest, appManifest, (object) dependentAssembly, fileSourceUri, targetDirectory, dependentAssembly.CodebaseFS, dependentAssembly.HashCollection);
        total += (long) dependentAssembly.Size;
      }
      downloader.SetExpectedBytesTotal(total);
      if (filesInGroup.Length == 0 && assembliesInGroup.Length == 0)
        throw new InvalidDeploymentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_NoSuchDownloadGroup"), new object[1]
        {
          (object) group
        }));
    }

    private static Uri MapFileSourceUri(AssemblyManifest deployManifest, Uri sourceUriBase, string fileName)
    {
      return UriHelper.UriFromRelativeFilePath(sourceUriBase, deployManifest.Deployment.MapFileExtensions ? fileName + ".deploy" : fileName);
    }

    private static void AddFileToDownloader(FileDownloader downloader, AssemblyManifest deployManifest, AssemblyManifest appManifest, object manifestElement, Uri fileSourceUri, string targetDirectory, string targetFileName, HashCollection hashCollection)
    {
      string targetFilePath = Path.Combine(targetDirectory, targetFileName);
      DownloadManager.DependencyDownloadCookie dependencyDownloadCookie = new DownloadManager.DependencyDownloadCookie(manifestElement, deployManifest, appManifest);
      downloader.AddFile(fileSourceUri, targetFilePath, (object) dependencyDownloadCookie, hashCollection);
    }

    private static void ProcessDownloadedFile(object sender, DownloadEventArgs e)
    {
      if (e.Cookie == null)
        return;
      string fileName = Path.GetFileName(e.FileLocalPath);
      FileDownloader downloader = (FileDownloader) sender;
      if (e.FileResponseUri != (Uri) null && !e.FileResponseUri.Equals((object) e.FileSourceUri))
        throw new InvalidDeploymentException(ExceptionTypes.AppFileLocationValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_DownloadAppFileAsmRedirected"), new object[1]
        {
          (object) fileName
        }));
      DownloadManager.DependencyDownloadCookie cookie = (DownloadManager.DependencyDownloadCookie) e.Cookie;
      if (cookie.ManifestElement is DependentAssembly)
      {
        DependentAssembly manifestElement = (DependentAssembly) cookie.ManifestElement;
        AssemblyManifest deployManifest = cookie.DeployManifest;
        AssemblyManifest appManifest = cookie.AppManifest;
        AssemblyManifest assemblyManifest = new AssemblyManifest(e.FileLocalPath);
        if (!assemblyManifest.Identity.Matches(manifestElement.Identity, true))
          throw new InvalidDeploymentException(ExceptionTypes.RefDefValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_DownloadRefDefMismatch"), new object[1]
          {
            (object) fileName
          }));
        if (assemblyManifest.Identity.Equals((object) deployManifest.Identity) || assemblyManifest.Identity.Equals((object) appManifest.Identity))
          throw new InvalidDeploymentException(ExceptionTypes.ManifestSemanticValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_AppPrivAsmIdSameAsDeployOrApp"), new object[1]
          {
            (object) assemblyManifest.Identity.ToString()
          }));
        System.Deployment.Application.Manifest.File[] files = assemblyManifest.Files;
        for (int index = 0; index < files.Length; ++index)
        {
          Uri fileSourceUri = DownloadManager.MapFileSourceUri(deployManifest, e.FileSourceUri, files[index].Name);
          if (!fileSourceUri.AbsoluteUri.Equals(e.FileSourceUri.AbsoluteUri, StringComparison.OrdinalIgnoreCase))
          {
            string directoryName = Path.GetDirectoryName(e.FileLocalPath);
            DownloadManager.AddFileToDownloader(downloader, deployManifest, appManifest, (object) files[index], fileSourceUri, directoryName, files[index].NameFS, files[index].HashCollection);
          }
        }
        downloader.ComponentVerifier.AddFileForVerification(e.FileLocalPath, manifestElement.HashCollection);
        if (assemblyManifest.Identity.PublicKeyToken == null)
          downloader.ComponentVerifier.AddSimplyNamedAssemblyForVerification(e.FileLocalPath, assemblyManifest);
        else
          downloader.ComponentVerifier.AddStrongNameAssemblyForVerification(e.FileLocalPath, assemblyManifest);
      }
      else
      {
        if (!(cookie.ManifestElement is System.Deployment.Application.Manifest.File))
          return;
        System.Deployment.Application.Manifest.File manifestElement = (System.Deployment.Application.Manifest.File) cookie.ManifestElement;
        downloader.ComponentVerifier.AddFileForVerification(e.FileLocalPath, manifestElement.HashCollection);
      }
    }

    private static void ReorderFilesForIconFile(AssemblyManifest manifest, System.Deployment.Application.Manifest.File[] files)
    {
      if (manifest.Description == null || manifest.Description.IconFile == null)
        return;
      for (int index = 0; index < files.Length; ++index)
      {
        if (string.Compare(files[index].NameFS, manifest.Description.IconFileFS, StringComparison.OrdinalIgnoreCase) == 0)
        {
          if (index == 0)
            break;
          System.Deployment.Application.Manifest.File file = files[0];
          files[0] = files[index];
          files[index] = file;
          break;
        }
      }
    }

    private class DependencyDownloadCookie
    {
      public readonly object ManifestElement;
      public readonly AssemblyManifest DeployManifest;
      public readonly AssemblyManifest AppManifest;

      public DependencyDownloadCookie(object manifestElement, AssemblyManifest deployManifest, AssemblyManifest appManifest)
      {
        this.ManifestElement = manifestElement;
        this.DeployManifest = deployManifest;
        this.AppManifest = appManifest;
      }
    }
  }
}
