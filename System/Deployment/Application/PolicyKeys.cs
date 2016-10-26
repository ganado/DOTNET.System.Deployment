// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.PolicyKeys
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using Microsoft.Win32;
using System.Globalization;
using System.IO;

namespace System.Deployment.Application
{
  internal static class PolicyKeys
  {
    public static bool RequireSignedManifests()
    {
      return PolicyKeys.CheckDeploymentBoolString("RequireSignedManifests", true, false);
    }

    public static bool RequireHashInManifests()
    {
      return PolicyKeys.CheckDeploymentBoolString("RequireHashInManifests", true, false);
    }

    public static bool SkipDeploymentProvider()
    {
      if (!PolicyKeys.CheckDeploymentBoolString("SkipDeploymentProvider", true, false))
        return false;
      Logger.AddWarningInformation(Resources.GetString("SkipDeploymentProvider"));
      return true;
    }

    public static bool SkipApplicationDependencyHashCheck()
    {
      if (!PolicyKeys.CheckDeploymentBoolString("SkipApplicationDependencyHashCheck", true, false))
        return false;
      Logger.AddWarningInformation(Resources.GetString("SkipApplicationDependencyHashCheck"));
      return true;
    }

    public static bool SkipSignatureValidation()
    {
      if (!PolicyKeys.CheckDeploymentBoolString("SkipSignatureValidation", true, false))
        return false;
      Logger.AddWarningInformation(Resources.GetString("SkipAllSigValidation"));
      return true;
    }

    public static bool SkipSchemaValidation()
    {
      if (!PolicyKeys.CheckDeploymentBoolString("SkipSchemaValidation", true, false))
        return false;
      Logger.AddWarningInformation(Resources.GetString("SkipSchemaValidation"));
      return true;
    }

    public static bool SkipSemanticValidation()
    {
      if (!PolicyKeys.CheckDeploymentBoolString("SkipSemanticValidation", true, false))
        return false;
      Logger.AddWarningInformation(Resources.GetString("SkipAllSemanticValidation"));
      return true;
    }

    public static bool SuppressLimitOnNumberOfActivations()
    {
      if (!PolicyKeys.CheckDeploymentBoolString("SuppressLimitOnNumberOfActivations", true, false))
        return false;
      Logger.AddWarningInformation(Resources.GetString("SuppressLimitOnNumberOfActivations"));
      return true;
    }

    public static bool DisableGenericExceptionHandler()
    {
      if (!PolicyKeys.CheckDeploymentBoolString("DisableGenericExceptionHandler", true, false))
        return false;
      Logger.AddWarningInformation(Resources.GetString("DisableGenericExceptionHandler"));
      return true;
    }

    public static ushort GetLogVerbosityLevel()
    {
      ushort num = 0;
      try
      {
        using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Classes\\Software\\Microsoft\\Windows\\CurrentVersion\\Deployment", false))
        {
          if (registryKey != null)
          {
            object obj = registryKey.GetValue("LogVerbosityLevel");
            if (obj is string)
            {
              Logger.AddWarningInformation(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogVerbosityLevelSet"), new object[1]{ obj }));
              num = Convert.ToUInt16(obj, (IFormatProvider) CultureInfo.InvariantCulture);
            }
          }
        }
      }
      catch (Exception ex)
      {
        if (ExceptionUtility.IsHardException(ex))
          throw;
        else
          num = (ushort) 0;
      }
      return num;
    }

    public static bool ProduceDetailedExecutionSectionInLog()
    {
      return (int) PolicyKeys.GetLogVerbosityLevel() > 0;
    }

    public static PolicyKeys.HostType ClrHostType()
    {
      int num = 0;
      using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\.NETFramework\\DeploymentFramework", false))
      {
        if (registryKey != null)
        {
          object obj = registryKey.GetValue("ClickOnceHost");
          num = obj != null ? (int) obj : 0;
        }
      }
      switch ((PolicyKeys.HostType) num)
      {
        case PolicyKeys.HostType.AppLaunch:
          Logger.AddWarningInformation(Resources.GetString("ForceAppLaunch"));
          break;
        case PolicyKeys.HostType.Cor:
          Logger.AddWarningInformation(Resources.GetString("ForceCor"));
          break;
      }
      return (PolicyKeys.HostType) num;
    }

    private static bool CheckDeploymentBoolString(string keyName, bool compare, bool defaultIfNotSet)
    {
      bool flag1 = false;
      bool flag2 = false;
      using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\.NETFramework\\DeploymentFramework", false))
      {
        if (registryKey != null)
        {
          string string1 = registryKey.GetValue(keyName) as string;
          if (string1 != null)
          {
            flag2 = true;
            CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
            if (compareInfo.Compare(string1, "true", CompareOptions.IgnoreCase) == 0)
              flag1 = true;
            else if (compareInfo.Compare(string1, "false", CompareOptions.IgnoreCase) == 0)
              flag1 = false;
          }
        }
      }
      if (!flag2)
        return defaultIfNotSet;
      return flag1 == compare;
    }

    private static bool CheckRegistryBoolString(RegistryKey registryKey, string valueName, bool compare, bool defaultIfNotSet)
    {
      bool flag1 = false;
      bool flag2 = false;
      if (registryKey != null)
      {
        string string1 = registryKey.GetValue(valueName) as string;
        if (string1 != null)
        {
          flag2 = true;
          CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
          if (compareInfo.Compare(string1, "true", CompareOptions.IgnoreCase) == 0)
            flag1 = true;
          else if (compareInfo.Compare(string1, "false", CompareOptions.IgnoreCase) == 0)
            flag1 = false;
        }
      }
      if (!flag2)
        return defaultIfNotSet;
      return flag1 == compare;
    }

    public static bool SkipSKUDetection()
    {
      bool flag = false;
      try
      {
        using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Fusion", false))
        {
          if (registryKey != null)
          {
            object obj = registryKey.GetValue("NoClientChecks");
            if (obj != null)
            {
              if (Convert.ToUInt32(obj) > 0U)
              {
                Logger.AddWarningInformation(Resources.GetString("SkippedSKUDetection"));
                flag = true;
              }
            }
          }
        }
      }
      catch (OverflowException ex)
      {
        flag = false;
      }
      catch (InvalidCastException ex)
      {
        flag = false;
      }
      catch (IOException ex)
      {
        flag = false;
      }
      return flag;
    }

    public enum HostType
    {
      Default,
      AppLaunch,
      Cor,
    }
  }
}
