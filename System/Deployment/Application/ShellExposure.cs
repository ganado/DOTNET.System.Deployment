// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.ShellExposure
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using Microsoft.Win32;
using System.Deployment.Application.Manifest;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;

namespace System.Deployment.Application
{
  internal static class ShellExposure
  {
    private static RegistryKey UninstallRoot
    {
      get
      {
        if (!PlatformSpecific.OnWin9x)
          return Registry.CurrentUser;
        return Registry.LocalMachine;
      }
    }

    public static void UpdateSubscriptionShellExposure(SubscriptionState subState)
    {
      using (subState.SubscriptionStore.AcquireStoreWriterLock())
      {
        ShellExposure.ShellExposureInformation exposureInformation = ShellExposure.ShellExposureInformation.CreateShellExposureInformation(subState.SubscriptionId);
        ShellExposure.UpdateShortcuts(subState, ref exposureInformation);
        ShellExposure.UpdateShellExtensions(subState, ref exposureInformation);
        ShellExposure.UpdateArpEntry(subState, exposureInformation);
      }
    }

    public static void RemoveSubscriptionShellExposure(SubscriptionState subState)
    {
      using (subState.SubscriptionStore.AcquireStoreWriterLock())
      {
        DefinitionIdentity subscriptionId = subState.SubscriptionId;
        bool flag = false;
        ShellExposure.ShellExposureInformation exposureInformation = ShellExposure.ShellExposureInformation.CreateShellExposureInformation(subscriptionId);
        if (exposureInformation == null)
        {
          flag = true;
        }
        else
        {
          for (int index = 1; index <= 2; ++index)
          {
            try
            {
              ShellExposure.RemoveShortcuts(exposureInformation);
              break;
            }
            catch (DeploymentException ex)
            {
              Logger.AddInternalState("Remove shortcut entries Failed: " + exposureInformation.ApplicationShortcutPath + "," + exposureInformation.SupportShortcutPath + "," + exposureInformation.DesktopShortcutPath + "," + exposureInformation.ApplicationFolderPath + "," + exposureInformation.ApplicationRootFolderPath);
              if (index < 2)
                Thread.Sleep(1000);
              else if (!(ex.InnerException is UnauthorizedAccessException))
                throw;
            }
          }
        }
        ShellExposure.RemoveArpEntry(subscriptionId);
        if (flag)
          throw new DeploymentException(ExceptionTypes.Subscription, Resources.GetString("Ex_ShortcutRemovalFailureDueToInvalidPublisherProduct"));
      }
    }

    public static void RemoveShellExtensions(DefinitionIdentity subId, AssemblyManifest appManifest, string productName)
    {
      foreach (FileAssociation fileAssociation in appManifest.FileAssociations)
        ShellExposure.RemoveFileAssociation(fileAssociation, subId, productName);
      NativeMethods.SHChangeNotify(134217728, 0U, IntPtr.Zero, IntPtr.Zero);
    }

    public static void ParseAppShortcut(string shortcutFile, out DefinitionIdentity subId, out Uri providerUri)
    {
      if (new FileInfo(shortcutFile).Length > 65536L)
        throw new DeploymentException(ExceptionTypes.InvalidShortcut, Resources.GetString("Ex_ShortcutTooLarge"));
      using (StreamReader streamReader = new StreamReader(shortcutFile, Encoding.Unicode))
      {
        string end;
        try
        {
          end = streamReader.ReadToEnd();
        }
        catch (IOException ex)
        {
          throw new DeploymentException(ExceptionTypes.InvalidShortcut, Resources.GetString("Ex_InvalidShortcutFormat"), (Exception) ex);
        }
        Logger.AddInternalState("Shortcut Text=" + end);
        if (end == null)
          throw new DeploymentException(ExceptionTypes.InvalidShortcut, Resources.GetString("Ex_InvalidShortcutFormat"));
        int length = end.IndexOf('#');
        if (length < 0)
          throw new DeploymentException(ExceptionTypes.InvalidShortcut, Resources.GetString("Ex_InvalidShortcutFormat"));
        try
        {
          subId = new DefinitionIdentity(end.Substring(length + 1));
        }
        catch (COMException ex)
        {
          throw new DeploymentException(ExceptionTypes.InvalidShortcut, Resources.GetString("Ex_InvalidShortcutFormat"), (Exception) ex);
        }
        catch (SEHException ex)
        {
          throw new DeploymentException(ExceptionTypes.InvalidShortcut, Resources.GetString("Ex_InvalidShortcutFormat"), (Exception) ex);
        }
        try
        {
          providerUri = new Uri(end.Substring(0, length));
        }
        catch (UriFormatException ex)
        {
          throw new DeploymentException(ExceptionTypes.InvalidShortcut, Resources.GetString("Ex_InvalidShortcutFormat"), (Exception) ex);
        }
      }
    }

    private static void MoveDeleteFile(string filePath)
    {
      if (!System.IO.File.Exists(filePath))
        return;
      string path = filePath;
      string destFileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
      try
      {
        System.IO.File.Move(filePath, destFileName);
        path = destFileName;
      }
      catch (IOException ex)
      {
      }
      catch (UnauthorizedAccessException ex)
      {
      }
      try
      {
        System.IO.File.Delete(path);
      }
      catch (IOException ex)
      {
      }
      catch (UnauthorizedAccessException ex)
      {
      }
    }

    private static void MoveDeleteEmptyFolder(string folderPath)
    {
      if (!Directory.Exists(folderPath) || Directory.GetFiles(folderPath).Length != 0)
        return;
      string path = folderPath;
      string destDirName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
      try
      {
        Directory.Move(folderPath, destDirName);
        path = destDirName;
      }
      catch (IOException ex)
      {
      }
      catch (UnauthorizedAccessException ex)
      {
      }
      try
      {
        Directory.Delete(path);
        Logger.AddInternalState("Deleted successfully: " + path);
      }
      catch (IOException ex)
      {
        Logger.AddInternalState("Exception thrown deleting " + path + ":" + ex.GetType().ToString() + ":" + ex.Message);
      }
      catch (UnauthorizedAccessException ex)
      {
        Logger.AddInternalState("Exception thrown deleting " + path + ":" + ex.GetType().ToString() + ":" + ex.Message);
      }
    }

    private static void UpdateShortcuts(SubscriptionState subState, ref ShellExposure.ShellExposureInformation shellExposureInformation)
    {
      string str = string.Format("{0}#{1}", (object) subState.DeploymentProviderUri.AbsoluteUri, (object) subState.SubscriptionId.ToString());
      Description effectiveDescription = subState.EffectiveDescription;
      if (shellExposureInformation != null)
      {
        bool flag1 = true;
        bool flag2 = true;
        bool flag3 = true;
        bool flag4 = true;
        if (string.Compare(effectiveDescription.FilteredPublisher, shellExposureInformation.AppVendor, StringComparison.Ordinal) == 0)
        {
          flag1 = false;
          if (Utilities.CompareWithNullEqEmpty(effectiveDescription.FilteredSuiteName, shellExposureInformation.AppSuiteName, StringComparison.Ordinal) == 0)
          {
            flag2 = false;
            if (string.Compare(effectiveDescription.FilteredProduct, shellExposureInformation.AppProduct, StringComparison.Ordinal) == 0)
            {
              flag3 = false;
              if (string.Compare(str, shellExposureInformation.ShortcutAppId, StringComparison.Ordinal) == 0)
                flag4 = false;
            }
          }
        }
        if (!flag1 && !flag2 && (!flag3 && !flag4) && System.IO.File.Exists(shellExposureInformation.ApplicationShortcutPath))
        {
          Logger.AddInternalState("Shortcut folder and files are not updated and application shortcut file already exists: " + shellExposureInformation.ApplicationShortcutPath);
          return;
        }
        if (flag3)
        {
          ShellExposure.UnpinShortcut(shellExposureInformation.ApplicationShortcutPath);
          ShellExposure.MoveDeleteFile(shellExposureInformation.ApplicationShortcutPath);
          ShellExposure.MoveDeleteFile(shellExposureInformation.SupportShortcutPath);
          ShellExposure.MoveDeleteFile(shellExposureInformation.DesktopShortcutPath);
          Logger.AddInternalState("Shortcut files deleted:" + shellExposureInformation.ApplicationShortcutPath + "," + shellExposureInformation.SupportShortcutPath + "," + shellExposureInformation.DesktopShortcutPath);
        }
        if (flag2)
        {
          Logger.AddInternalState("Attempt deleting shortcut folder:" + shellExposureInformation.ApplicationFolderPath);
          ShellExposure.MoveDeleteEmptyFolder(shellExposureInformation.ApplicationFolderPath);
        }
        if (flag1)
        {
          Logger.AddInternalState("Attempt deleting shortcut root folder:" + shellExposureInformation.ApplicationRootFolderPath);
          ShellExposure.MoveDeleteEmptyFolder(shellExposureInformation.ApplicationRootFolderPath);
        }
        if (flag1 | flag2 | flag3)
        {
          shellExposureInformation = ShellExposure.ShellExposureInformation.CreateShellExposureInformation(effectiveDescription.FilteredPublisher, effectiveDescription.FilteredSuiteName, effectiveDescription.FilteredProduct, str);
        }
        else
        {
          Logger.AddInternalState("Shortcut app id has changed. Old value=" + shellExposureInformation.ShortcutAppId + ",New value=" + str);
          shellExposureInformation.ShortcutAppId = str;
        }
      }
      else
        shellExposureInformation = ShellExposure.ShellExposureInformation.CreateShellExposureInformation(effectiveDescription.FilteredPublisher, effectiveDescription.FilteredSuiteName, effectiveDescription.FilteredProduct, str);
      try
      {
        Logger.AddInternalState("Create the shortcut directory : " + shellExposureInformation.ApplicationFolderPath);
        Directory.CreateDirectory(shellExposureInformation.ApplicationFolderPath);
        ShellExposure.GenerateAppShortcut(subState, shellExposureInformation);
        ShellExposure.GenerateSupportShortcut(subState, shellExposureInformation);
      }
      catch (Exception ex)
      {
        ShellExposure.RemoveShortcuts(shellExposureInformation);
        throw;
      }
    }

    private static void GenerateAppShortcut(SubscriptionState subState, ShellExposure.ShellExposureInformation shellExposureInformation)
    {
      using (StreamWriter streamWriter = new StreamWriter(shellExposureInformation.ApplicationShortcutPath, false, Encoding.Unicode))
        streamWriter.Write("{0}#{1}", (object) subState.DeploymentProviderUri.AbsoluteUri, (object) subState.SubscriptionId.ToString());
      Logger.AddInternalState("Shortcut file created: " + shellExposureInformation.ApplicationShortcutPath);
      if (!subState.CurrentDeploymentManifest.Deployment.CreateDesktopShortcut)
        return;
      using (StreamWriter streamWriter = new StreamWriter(shellExposureInformation.DesktopShortcutPath, false, Encoding.Unicode))
        streamWriter.Write("{0}#{1}", (object) subState.DeploymentProviderUri.AbsoluteUri, (object) subState.SubscriptionId.ToString());
      Logger.AddInternalState("Desktop Shortcut file created: " + shellExposureInformation.DesktopShortcutPath);
    }

    private static void GenerateSupportShortcut(SubscriptionState subState, ShellExposure.ShellExposureInformation shellExposureInformation)
    {
      Description effectiveDescription = subState.EffectiveDescription;
      if (!(effectiveDescription.SupportUri != (Uri) null))
        return;
      using (StreamWriter streamWriter = new StreamWriter(shellExposureInformation.SupportShortcutPath, false, Encoding.ASCII))
      {
        streamWriter.WriteLine("[Default]");
        streamWriter.WriteLine("BASEURL=" + effectiveDescription.SupportUri.AbsoluteUri);
        streamWriter.WriteLine("[InternetShortcut]");
        streamWriter.WriteLine("URL=" + effectiveDescription.SupportUri.AbsoluteUri);
        streamWriter.WriteLine();
        streamWriter.WriteLine("IconFile=" + PathHelper.ShortShimDllPath);
        streamWriter.WriteLine("IconIndex=" + 0.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        streamWriter.WriteLine();
      }
      Logger.AddInternalState("Support shortcut file created: " + shellExposureInformation.SupportShortcutPath);
    }

    private static void RemoveShortcuts(ShellExposure.ShellExposureInformation shellExposureInformation)
    {
      try
      {
        if (System.IO.File.Exists(shellExposureInformation.ApplicationShortcutPath))
          System.IO.File.Delete(shellExposureInformation.ApplicationShortcutPath);
        if (System.IO.File.Exists(shellExposureInformation.SupportShortcutPath))
          System.IO.File.Delete(shellExposureInformation.SupportShortcutPath);
        if (System.IO.File.Exists(shellExposureInformation.DesktopShortcutPath))
          System.IO.File.Delete(shellExposureInformation.DesktopShortcutPath);
        if (Directory.Exists(shellExposureInformation.ApplicationFolderPath) && (Directory.GetFiles(shellExposureInformation.ApplicationFolderPath).Length == 0 && Directory.GetDirectories(shellExposureInformation.ApplicationFolderPath).Length == 0))
          Directory.Delete(shellExposureInformation.ApplicationFolderPath);
        if (Directory.Exists(shellExposureInformation.ApplicationRootFolderPath) && (Directory.GetFiles(shellExposureInformation.ApplicationRootFolderPath).Length == 0 && Directory.GetDirectories(shellExposureInformation.ApplicationRootFolderPath).Length == 0))
          Directory.Delete(shellExposureInformation.ApplicationRootFolderPath);
        Logger.AddInternalState("Removed shortcut entries : " + shellExposureInformation.ApplicationShortcutPath + "," + shellExposureInformation.SupportShortcutPath + "," + shellExposureInformation.DesktopShortcutPath + "," + shellExposureInformation.ApplicationFolderPath + "," + shellExposureInformation.ApplicationRootFolderPath);
      }
      catch (IOException ex)
      {
        throw new DeploymentException(ExceptionTypes.InvalidShortcut, Resources.GetString("Ex_ShortcutRemovalFailure"), (Exception) ex);
      }
      catch (UnauthorizedAccessException ex)
      {
        throw new DeploymentException(ExceptionTypes.InvalidShortcut, Resources.GetString("Ex_ShortcutRemovalFailure"), (Exception) ex);
      }
    }

    internal static void RemovePins(SubscriptionState subState)
    {
      Logger.AddInternalState("Attempting to remove shell pins.");
      ShellExposure.ShellExposureInformation exposureInformation = ShellExposure.ShellExposureInformation.CreateShellExposureInformation(subState.SubscriptionId);
      if (exposureInformation == null)
      {
        Logger.AddInternalState("shellExposureInformation is null.");
      }
      else
      {
        if (!System.IO.File.Exists(exposureInformation.ApplicationShortcutPath))
          return;
        ShellExposure.UnpinShortcut(exposureInformation.ApplicationShortcutPath);
      }
    }

    public static void UpdateShellExtensions(SubscriptionState subState, ref ShellExposure.ShellExposureInformation shellExposureInformation)
    {
      string productName = (string) null;
      if (shellExposureInformation != null)
        productName = shellExposureInformation.AppProduct;
      if (productName == null)
        productName = subState.SubscriptionId.Name;
      if (subState.PreviousBind != null)
      {
        Logger.AddInternalState("Removing file associations if existed for the previous version.");
        ShellExposure.RemoveShellExtensions(subState.SubscriptionId, subState.PreviousApplicationManifest, productName);
      }
      Logger.AddInternalState("Registering file associations if there is any in the manifest for the new version. ");
      ShellExposure.AddShellExtensions(subState.SubscriptionId, subState.DeploymentProviderUri, subState.CurrentApplicationManifest);
      NativeMethods.SHChangeNotify(134217728, 0U, IntPtr.Zero, IntPtr.Zero);
    }

    private static void UnpinShortcut(string shortcutPath)
    {
      NativeMethods.IShellItem psi = (NativeMethods.IShellItem) null;
      NativeMethods.IStartMenuPinnedList startMenuPinnedList = (NativeMethods.IStartMenuPinnedList) null;
      try
      {
        object ppv = (object) null;
        object o = (object) null;
        if ((int) NativeMethods.SHCreateItemFromParsingName(shortcutPath, IntPtr.Zero, Constants.uuid, out ppv) != 0)
          return;
        psi = ppv as NativeMethods.IShellItem;
        if ((int) NativeMethods.CoCreateInstance(ref Constants.CLSID_StartMenuPin, (object) null, 1, ref Constants.IID_IUnknown, out o) != 0)
          return;
        startMenuPinnedList = o as NativeMethods.IStartMenuPinnedList;
        startMenuPinnedList.RemoveFromList(psi);
      }
      catch (EntryPointNotFoundException ex)
      {
      }
      catch (UnauthorizedAccessException ex)
      {
      }
      finally
      {
        if (psi != null)
          Marshal.ReleaseComObject((object) psi);
        if (startMenuPinnedList != null)
          Marshal.ReleaseComObject((object) startMenuPinnedList);
      }
    }

    private static void AddShellExtensions(DefinitionIdentity subId, Uri deploymentProviderUri, AssemblyManifest appManifest)
    {
      foreach (FileAssociation fileAssociation in appManifest.FileAssociations)
        ShellExposure.AddFileAssociation(fileAssociation, subId, deploymentProviderUri);
    }

    private static bool CanAddFileAssociation(FileAssociation fileAssociation)
    {
      try
      {
        using (RegistryKey registryKey1 = Registry.CurrentUser.OpenSubKey("Software\\Classes"))
        {
          using (RegistryKey registryKey2 = registryKey1.OpenSubKey(fileAssociation.Extension))
          {
            using (RegistryKey registryKey3 = registryKey1.OpenSubKey(fileAssociation.ProgID))
            {
              if (registryKey2 == null)
              {
                if (registryKey3 == null)
                  goto label_16;
              }
              Logger.AddWarningInformation(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("SkippedFileAssoc"), new object[1]
              {
                (object) fileAssociation.Extension
              }));
              Logger.AddInternalState("File association for " + fileAssociation.Extension + " skipped, since another application is using it.");
              return false;
            }
          }
        }
      }
      catch (SecurityException ex)
      {
        Logger.AddInternalState("Exception reading registry key : " + ex.StackTrace);
        Logger.AddInternalState("File association for " + fileAssociation.Extension + " skipped");
        return false;
      }
label_16:
      return true;
    }

    private static void AddFileAssociation(FileAssociation fileAssociation, DefinitionIdentity subId, Uri deploymentProviderUri)
    {
      if (!ShellExposure.CanAddFileAssociation(fileAssociation))
        return;
      string subkey = Guid.NewGuid().ToString("B");
      string str = subId.ToString();
      using (RegistryKey subKey1 = Registry.CurrentUser.CreateSubKey("Software\\Classes"))
      {
        using (RegistryKey subKey2 = subKey1.CreateSubKey(fileAssociation.Extension))
        {
          subKey2.SetValue((string) null, (object) fileAssociation.ProgID);
          subKey2.SetValue("AppId", (object) str);
          subKey2.SetValue("Guid", (object) subkey);
          subKey2.SetValue("DeploymentProviderUrl", (object) deploymentProviderUri.AbsoluteUri);
        }
        using (RegistryKey subKey2 = subKey1.CreateSubKey(fileAssociation.ProgID))
        {
          subKey2.SetValue((string) null, (object) fileAssociation.Description);
          subKey2.SetValue("AppId", (object) str);
          subKey2.SetValue("Guid", (object) subkey);
          subKey2.SetValue("DeploymentProviderUrl", (object) deploymentProviderUri.AbsoluteUri);
          using (RegistryKey subKey3 = subKey2.CreateSubKey("shell"))
          {
            subKey3.SetValue((string) null, (object) "open");
            using (RegistryKey subKey4 = subKey3.CreateSubKey("open\\command"))
            {
              subKey4.SetValue((string) null, (object) ("rundll32.exe dfshim.dll, ShOpenVerbExtension " + subkey + " %1"));
              Logger.AddInternalState("File association created. Extension=" + fileAssociation.Extension + " value=rundll32.exe dfshim.dll, ShOpenVerbExtension " + subkey + " %1");
            }
            using (RegistryKey subKey4 = subKey2.CreateSubKey("shellex\\IconHandler"))
            {
              subKey4.SetValue((string) null, (object) subkey);
              Logger.AddInternalState("File association icon handler created. Extension=" + fileAssociation.Extension + " value=" + subkey);
            }
          }
        }
        using (RegistryKey subKey2 = subKey1.CreateSubKey("CLSID"))
        {
          using (RegistryKey subKey3 = subKey2.CreateSubKey(subkey))
          {
            subKey3.SetValue((string) null, (object) ("Shell Icon Handler For " + fileAssociation.Description));
            subKey3.SetValue("AppId", (object) str);
            subKey3.SetValue("DeploymentProviderUrl", (object) deploymentProviderUri.AbsoluteUri);
            subKey3.SetValue("IconFile", (object) fileAssociation.DefaultIcon);
            using (RegistryKey subKey4 = subKey3.CreateSubKey("InProcServer32"))
            {
              subKey4.SetValue((string) null, (object) "dfshim.dll");
              subKey4.SetValue("ThreadingModel", (object) "Apartment");
            }
          }
        }
      }
    }

    private static void RemoveFileAssociation(FileAssociation fileAssociation, DefinitionIdentity subId, string productName)
    {
      using (RegistryKey classesKey = Registry.CurrentUser.OpenSubKey("Software\\Classes", true))
      {
        if (classesKey == null)
          return;
        Logger.AddMethodCall("RemoveFileAssociation(" + fileAssociation.ToString() + ") called.");
        ShellExposure.RemoveFileAssociationExtentionInfo(fileAssociation, subId, classesKey, productName);
        string clsIdString = ShellExposure.RemoveFileAssociationProgIDInfo(fileAssociation, subId, classesKey, productName);
        if (clsIdString == null)
          return;
        ShellExposure.RemoveFileAssociationCLSIDInfo(fileAssociation, subId, classesKey, clsIdString, productName);
      }
    }

    private static void RemoveFileAssociationExtentionInfo(FileAssociation fileAssociation, DefinitionIdentity subId, RegistryKey classesKey, string productName)
    {
      using (RegistryKey registryKey = classesKey.OpenSubKey(fileAssociation.Extension, true))
      {
        if (registryKey == null)
          return;
        object obj = registryKey.GetValue("AppId");
        if (!(obj is string))
          return;
        if (!string.Equals((string) obj, subId.ToString(), StringComparison.Ordinal))
          return;
        try
        {
          classesKey.DeleteSubKeyTree(fileAssociation.Extension);
        }
        catch (ArgumentException ex)
        {
          throw new DeploymentException(ExceptionTypes.InvalidARPEntry, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_FileAssocExtDeleteFailed"), new object[2]
          {
            (object) fileAssociation.Extension,
            (object) productName
          }), (Exception) ex);
        }
      }
    }

    private static string RemoveFileAssociationProgIDInfo(FileAssociation fileAssociation, DefinitionIdentity subId, RegistryKey classesKey, string productName)
    {
      string str = (string) null;
      using (RegistryKey registryKey = classesKey.OpenSubKey(fileAssociation.ProgID, true))
      {
        if (registryKey == null)
          return (string) null;
        object obj = registryKey.GetValue("AppId");
        if (!(obj is string) || !string.Equals((string) obj, subId.ToString(), StringComparison.Ordinal))
          return (string) null;
        str = (string) registryKey.GetValue("Guid");
        try
        {
          classesKey.DeleteSubKeyTree(fileAssociation.ProgID);
        }
        catch (ArgumentException ex)
        {
          throw new DeploymentException(ExceptionTypes.InvalidARPEntry, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_FileAssocProgIdDeleteFailed"), new object[2]
          {
            (object) fileAssociation.ProgID,
            (object) productName
          }), (Exception) ex);
        }
      }
      return str;
    }

    private static void RemoveFileAssociationCLSIDInfo(FileAssociation fileAssociation, DefinitionIdentity subId, RegistryKey classesKey, string clsIdString, string productName)
    {
      using (RegistryKey registryKey1 = classesKey.OpenSubKey("CLSID", true))
      {
        if (registryKey1 == null)
          return;
        using (RegistryKey registryKey2 = registryKey1.OpenSubKey(clsIdString))
        {
          if (registryKey2 == null)
            return;
          object obj = registryKey2.GetValue("AppId");
          if (!(obj is string))
            return;
          if (!string.Equals((string) obj, subId.ToString(), StringComparison.Ordinal))
            return;
          try
          {
            registryKey1.DeleteSubKeyTree(clsIdString);
          }
          catch (ArgumentException ex)
          {
            throw new DeploymentException(ExceptionTypes.InvalidARPEntry, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_FileAssocCLSIDDeleteFailed"), new object[2]
            {
              (object) clsIdString,
              (object) productName
            }), (Exception) ex);
          }
        }
      }
    }

    private static void UpdateArpEntry(SubscriptionState subState, ShellExposure.ShellExposureInformation shellExposureInformation)
    {
      DefinitionIdentity subscriptionId = subState.SubscriptionId;
      string str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "rundll32.exe dfshim.dll,ShArpMaintain {0}", new object[1]
      {
        (object) subscriptionId.ToString()
      });
      string str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "dfshim.dll,2", new object[0]);
      AssemblyManifest deploymentManifest = subState.CurrentDeploymentManifest;
      Description effectiveDescription = subState.EffectiveDescription;
      using (RegistryKey subKey1 = ShellExposure.UninstallRoot.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall"))
      {
        using (RegistryKey subKey2 = subKey1.CreateSubKey(ShellExposure.GenerateArpKeyName(subscriptionId)))
        {
          string[] strArray = new string[24]
          {
            "DisplayName",
            shellExposureInformation.ARPDisplayName,
            "DisplayIcon",
            str2,
            "DisplayVersion",
            deploymentManifest.Identity.Version.ToString(),
            "Publisher",
            effectiveDescription.FilteredPublisher,
            "UninstallString",
            str1,
            "HelpLink",
            effectiveDescription.SupportUrl,
            "UrlUpdateInfo",
            subState.DeploymentProviderUri.AbsoluteUri,
            "ShortcutFolderName",
            shellExposureInformation.AppVendor,
            "ShortcutFileName",
            shellExposureInformation.AppProduct,
            "ShortcutSuiteName",
            shellExposureInformation.AppSuiteName,
            "SupportShortcutFileName",
            shellExposureInformation.AppSupportShortcut,
            "ShortcutAppId",
            shellExposureInformation.ShortcutAppId
          };
          Logger.AddInternalState("Updating ARP entry.");
          int index = strArray.Length - 2;
          while (index >= 0)
          {
            string name = strArray[index];
            string str3 = strArray[index + 1];
            if (str3 != null)
              subKey2.SetValue(name, (object) str3);
            else
              subKey2.DeleteValue(name, false);
            index -= 2;
          }
        }
      }
    }

    private static void RemoveArpEntry(DefinitionIdentity subId)
    {
      using (RegistryKey registryKey = ShellExposure.UninstallRoot.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall", true))
      {
        string subkey = (string) null;
        try
        {
          if (registryKey == null)
            return;
          subkey = ShellExposure.GenerateArpKeyName(subId);
          registryKey.DeleteSubKeyTree(subkey);
        }
        catch (ArgumentException ex)
        {
          throw new DeploymentException(ExceptionTypes.InvalidARPEntry, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_ArpEntryRemovalFailure"), new object[1]
          {
            (object) subkey
          }), (Exception) ex);
        }
      }
    }

    private static string GenerateArpKeyName(DefinitionIdentity subId)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:x16}", new object[1]
      {
        (object) subId.Hash
      });
    }

    public class ShellExposureInformation
    {
      private string _applicationFolderPath;
      private string _applicationRootFolderPath;
      private string _applicationShortcutPath;
      private string _desktopShortcutPath;
      private string _supportShortcutPath;
      private string _appVendor;
      private string _appProduct;
      private string _appSuiteName;
      private string _appSupportShortcut;
      private string _shortcutAppId;

      public string ApplicationFolderPath
      {
        get
        {
          return this._applicationFolderPath;
        }
      }

      public string ApplicationRootFolderPath
      {
        get
        {
          return this._applicationRootFolderPath;
        }
      }

      public string ApplicationShortcutPath
      {
        get
        {
          return this._applicationShortcutPath;
        }
      }

      public string SupportShortcutPath
      {
        get
        {
          return this._supportShortcutPath;
        }
      }

      public string DesktopShortcutPath
      {
        get
        {
          return this._desktopShortcutPath;
        }
      }

      public string ARPDisplayName
      {
        get
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append(this._appProduct);
          if (PlatformSpecific.OnWin9x && stringBuilder.Length > 63)
          {
            stringBuilder.Length = 60;
            stringBuilder.Append("...");
          }
          return stringBuilder.ToString();
        }
      }

      public string AppVendor
      {
        get
        {
          return this._appVendor;
        }
      }

      public string AppProduct
      {
        get
        {
          return this._appProduct;
        }
      }

      public string AppSuiteName
      {
        get
        {
          return this._appSuiteName;
        }
      }

      public string AppSupportShortcut
      {
        get
        {
          return this._appSupportShortcut;
        }
      }

      public string ShortcutAppId
      {
        get
        {
          return this._shortcutAppId;
        }
        set
        {
          this._shortcutAppId = value;
        }
      }

      protected ShellExposureInformation()
      {
      }

      public static ShellExposure.ShellExposureInformation CreateShellExposureInformation(DefinitionIdentity subscriptionIdentity)
      {
        ShellExposure.ShellExposureInformation exposureInformation = (ShellExposure.ShellExposureInformation) null;
        string path2_1 = (string) null;
        string str1 = (string) null;
        string path2_2 = (string) null;
        string str2 = (string) null;
        string str3 = "";
        using (RegistryKey registryKey1 = ShellExposure.UninstallRoot.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall"))
        {
          if (registryKey1 != null)
          {
            using (RegistryKey registryKey2 = registryKey1.OpenSubKey(ShellExposure.GenerateArpKeyName(subscriptionIdentity)))
            {
              if (registryKey2 != null)
              {
                path2_1 = registryKey2.GetValue("ShortcutFolderName") as string;
                str1 = registryKey2.GetValue("ShortcutFileName") as string;
                path2_2 = registryKey2.GetValue("ShortcutSuiteName") == null ? "" : registryKey2.GetValue("ShortcutSuiteName") as string;
                str2 = registryKey2.GetValue("SupportShortcutFileName") as string;
                str3 = registryKey2.GetValue("ShortcutAppId") == null ? "" : registryKey2.GetValue("ShortcutAppId") as string;
              }
            }
          }
        }
        if (path2_1 != null && str1 != null && str2 != null)
        {
          exposureInformation = new ShellExposure.ShellExposureInformation();
          exposureInformation._applicationRootFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), path2_1);
          exposureInformation._applicationFolderPath = !string.IsNullOrEmpty(path2_2) ? Path.Combine(exposureInformation._applicationRootFolderPath, path2_2) : exposureInformation._applicationRootFolderPath;
          exposureInformation._applicationShortcutPath = Path.Combine(exposureInformation._applicationFolderPath, str1 + ".appref-ms");
          exposureInformation._supportShortcutPath = Path.Combine(exposureInformation._applicationFolderPath, str2 + ".url");
          exposureInformation._desktopShortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), str1 + ".appref-ms");
          exposureInformation._appVendor = path2_1;
          exposureInformation._appProduct = str1;
          exposureInformation._appSupportShortcut = str2;
          exposureInformation._shortcutAppId = str3;
          exposureInformation._appSuiteName = path2_2;
        }
        return exposureInformation;
      }

      public static ShellExposure.ShellExposureInformation CreateShellExposureInformation(string publisher, string suiteName, string product, string shortcutAppId)
      {
        ShellExposure.ShellExposureInformation exposureInformation = new ShellExposure.ShellExposureInformation();
        string path1_1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), publisher);
        string path1_2 = path1_1;
        if (!string.IsNullOrEmpty(suiteName))
          path1_2 = Path.Combine(path1_1, suiteName);
        string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        for (int index = 0; index != int.MaxValue; ++index)
        {
          string str;
          if (index == 0)
            str = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("ShellExposure_DisplayStringNoIndex"), new object[1]
            {
              (object) product
            });
          else
            str = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("ShellExposure_DisplayStringWithIndex"), new object[2]
            {
              (object) product,
              (object) index
            });
          string path1 = Path.Combine(path1_2, str + ".appref-ms");
          string path2 = Path.Combine(folderPath, str + ".appref-ms");
          if (!System.IO.File.Exists(path1) && !System.IO.File.Exists(path2))
          {
            exposureInformation._appVendor = publisher;
            exposureInformation._appProduct = str;
            exposureInformation._appSuiteName = suiteName;
            exposureInformation._applicationFolderPath = path1_2;
            exposureInformation._applicationRootFolderPath = path1_1;
            exposureInformation._applicationShortcutPath = path1;
            exposureInformation._desktopShortcutPath = path2;
            exposureInformation._appSupportShortcut = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("SupportUrlFormatter"), new object[1]{ (object) str });
            exposureInformation._supportShortcutPath = Path.Combine(path1_2, exposureInformation._appSupportShortcut + ".url");
            exposureInformation._shortcutAppId = shortcutAppId;
            return exposureInformation;
          }
        }
        throw new OverflowException();
      }
    }
  }
}
