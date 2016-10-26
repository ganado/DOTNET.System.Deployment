// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.PlatformDetector
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using Microsoft.Win32;
using System.ComponentModel;
using System.Deployment.Application.Manifest;
using System.Deployment.Application.Win32InterOp;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Deployment.Application
{
  internal static class PlatformDetector
  {
    private static PlatformDetector.Suite[] Suites = new PlatformDetector.Suite[14]
    {
      new PlatformDetector.Suite("server", 2147483648U),
      new PlatformDetector.Suite("workstation", 1073741824U),
      new PlatformDetector.Suite("smallbusiness", 1U),
      new PlatformDetector.Suite("enterprise", 2U),
      new PlatformDetector.Suite("backoffice", 4U),
      new PlatformDetector.Suite("communications", 8U),
      new PlatformDetector.Suite("terminal", 16U),
      new PlatformDetector.Suite("smallbusinessRestricted", 32U),
      new PlatformDetector.Suite("embeddednt", 64U),
      new PlatformDetector.Suite("datacenter", 128U),
      new PlatformDetector.Suite("singleuserts", 256U),
      new PlatformDetector.Suite("personal", 512U),
      new PlatformDetector.Suite("blade", 1024U),
      new PlatformDetector.Suite("embeddedrestricted", 2048U)
    };
    private static PlatformDetector.Product[] Products = new PlatformDetector.Product[3]
    {
      new PlatformDetector.Product("workstation", 1U),
      new PlatformDetector.Product("domainController", 2U),
      new PlatformDetector.Product("server", 3U)
    };
    private const int MAX_PATH = 260;
    private const byte VER_EQUAL = 1;
    private const byte VER_GREATER = 2;
    private const byte VER_GREATER_EQUAL = 3;
    private const byte VER_LESS = 4;
    private const byte VER_LESS_EQUAL = 5;
    private const byte VER_AND = 6;
    private const byte VER_OR = 7;
    private const uint VER_MINORVERSION = 1;
    private const uint VER_MAJORVERSION = 2;
    private const uint VER_BUILDNUMBER = 4;
    private const uint VER_PLATFORMID = 8;
    private const uint VER_SERVICEPACKMINOR = 16;
    private const uint VER_SERVICEPACKMAJOR = 32;
    private const uint VER_SUITENAME = 64;
    private const uint VER_PRODUCT_TYPE = 128;
    private const uint VER_SERVER_NT = 2147483648;
    private const uint VER_WORKSTATION_NT = 1073741824;
    private const uint VER_SUITE_SMALLBUSINESS = 1;
    private const uint VER_SUITE_ENTERPRISE = 2;
    private const uint VER_SUITE_BACKOFFICE = 4;
    private const uint VER_SUITE_COMMUNICATIONS = 8;
    private const uint VER_SUITE_TERMINAL = 16;
    private const uint VER_SUITE_SMALLBUSINESS_RESTRICTED = 32;
    private const uint VER_SUITE_EMBEDDEDNT = 64;
    private const uint VER_SUITE_DATACENTER = 128;
    private const uint VER_SUITE_SINGLEUSERTS = 256;
    private const uint VER_SUITE_PERSONAL = 512;
    private const uint VER_SUITE_BLADE = 1024;
    private const uint VER_SUITE_EMBEDDED_RESTRICTED = 2048;
    private const uint VER_NT_WORKSTATION = 1;
    private const uint VER_NT_DOMAIN_CONTROLLER = 2;
    private const uint VER_NT_SERVER = 3;
    private const uint Windows9XMajorVersion = 4;
    private const uint RUNTIME_INFO_UPGRADE_VERSION = 1;
    private const uint RUNTIME_INFO_REQUEST_IA64 = 2;
    private const uint RUNTIME_INFO_REQUEST_AMD64 = 4;
    private const uint RUNTIME_INFO_REQUEST_X86 = 8;
    private const uint RUNTIME_INFO_DONT_RETURN_DIRECTORY = 16;
    private const uint RUNTIME_INFO_DONT_RETURN_VERSION = 32;
    private const uint RUNTIME_INFO_DONT_SHOW_ERROR_DIALOG = 64;
    private const uint RUNTIME_INFO_CONSIDER_POST_2_0 = 128;
    private const uint RUNTIME_INFO_EMULATE_EXE_LAUNCH = 256;
    private const uint RUNTIME_INFO_DONT_SHOW_INSTALL_DIALOG = 65536;

    public static string FormatFrameworkString(CompatibleFramework framework)
    {
      if (string.IsNullOrEmpty(framework.Profile))
        return string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("FrameworkNameNoProfile"), new object[1]
        {
          (object) framework.TargetVersion
        });
      return string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("FrameworkNameWithProfile"), new object[2]
      {
        (object) framework.TargetVersion,
        (object) framework.Profile
      });
    }

    public static bool DetectFrameworkInRegistry(string setupKeyPath, string setupValueName, Version versionRequired, bool detectInstallValue)
    {
      Logger.AddMethodCall("DetectFrameworkInRegistry(" + setupKeyPath + ", " + setupValueName + ", " + versionRequired.ToString() + ", " + detectInstallValue.ToString() + ") called");
      using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(setupKeyPath))
      {
        if (registryKey != null)
        {
          object obj = registryKey.GetValue(setupValueName);
          if (detectInstallValue)
          {
            if (obj is int)
            {
              if ((int) obj != 0)
                return true;
            }
          }
          else if (obj is string)
          {
            if (new Version((string) obj) >= versionRequired)
              return true;
          }
        }
      }
      return false;
    }

    public static bool DetectTFMInRegistry(string clrVersion, string frameworkVersion, string profile)
    {
      string regKey = "SOFTWARE\\Microsoft\\.NETFramework\\v" + clrVersion + "\\SKUs\\" + Utilities.BuildTFM(frameworkVersion, profile);
      return Utilities.DoesRegistryKeyExist(Registry.LocalMachine, regKey);
    }

    public static bool CheckCompatibleFramework(CompatibleFramework framework, ref Version clrVersion, ref string clrVersionString, string clrProcArch)
    {
      Logger.AddMethodCall("CheckCompatibleFramework called targetVersion:" + framework.TargetVersion + " profile:" + framework.Profile);
      Version versionRequired = new Version(framework.TargetVersion);
      string setupKeyPath1 = (string) null;
      bool detectInstallValue = false;
      string setupKeyPath2;
      string setupValueName;
      switch (versionRequired.Major)
      {
        case 2:
          setupKeyPath2 = "SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v" + framework.TargetVersion;
          setupValueName = "Install";
          detectInstallValue = true;
          break;
        case 3:
          if (versionRequired.Minor == 0)
          {
            setupKeyPath2 = "SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v3.0\\Setup";
            setupValueName = "InstallSuccess";
            detectInstallValue = true;
            break;
          }
          setupKeyPath2 = "SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v" + versionRequired.ToString(2);
          setupValueName = "Version";
          if ("Client".Equals(framework.Profile, StringComparison.OrdinalIgnoreCase))
          {
            setupKeyPath1 = "SOFTWARE\\Microsoft\\NET Framework Setup\\DotNetClient\\v" + versionRequired.ToString(2);
            break;
          }
          break;
        default:
          setupKeyPath2 = "SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v" + versionRequired.ToString(1);
          if (!string.IsNullOrEmpty(framework.Profile))
            setupKeyPath2 = setupKeyPath2 + "\\" + framework.Profile;
          setupValueName = "TargetVersion";
          break;
      }
      bool flag = PlatformDetector.DetectFrameworkInRegistry(setupKeyPath2, setupValueName, versionRequired, detectInstallValue) || setupKeyPath1 != null && PlatformDetector.DetectFrameworkInRegistry(setupKeyPath1, setupValueName, versionRequired, detectInstallValue);
      if (!flag && versionRequired.Major >= 4)
        flag = PlatformDetector.DetectTFMInRegistry(framework.SupportedRuntime, framework.TargetVersion, framework.Profile);
      if (!flag)
        return false;
      if (!NativeMethods.VerifyCLRVersionInfo(new Version(framework.SupportedRuntime), clrProcArch))
      {
        Logger.AddWarningInformation(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("CLRMissingForFoundFramework"), new object[2]
        {
          (object) framework.SupportedRuntime,
          (object) PlatformDetector.FormatFrameworkString(framework)
        }));
        return false;
      }
      clrVersionString = framework.SupportedRuntime;
      clrVersion = new Version(clrVersionString);
      return true;
    }

    public static bool IsCLRDependencyText(string clrTextName)
    {
      return string.Compare(clrTextName, "Microsoft-Windows-CLRCoreComp", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(clrTextName, "Microsoft.Windows.CommonLanguageRuntime", StringComparison.OrdinalIgnoreCase) == 0;
    }

    public static bool IsSupportedProcessorArchitecture(string arch)
    {
      if (string.Compare(arch, "msil", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(arch, "x86", StringComparison.OrdinalIgnoreCase) == 0)
        return true;
      NativeMethods.SYSTEM_INFO sysInfo = new NativeMethods.SYSTEM_INFO();
      bool flag;
      try
      {
        NativeMethods.GetNativeSystemInfo(ref sysInfo);
        flag = true;
      }
      catch (EntryPointNotFoundException ex)
      {
        Logger.AddInternalState("In IsSupportedProcessorArchitecture: GetNativeSystemInfo API from kernel32.dll is not found.");
        flag = false;
      }
      if (!flag)
      {
        NativeMethods.GetSystemInfo(ref sysInfo);
        Logger.AddInternalState("In IsSupportedProcessorArchitecture: GetSystemInfo called.");
      }
      switch (sysInfo.uProcessorInfo.wProcessorArchitecture)
      {
        case 6:
          return string.Compare(arch, "ia64", StringComparison.OrdinalIgnoreCase) == 0;
        case 9:
          return string.Compare(arch, "amd64", StringComparison.OrdinalIgnoreCase) == 0;
        default:
          return false;
      }
    }

    public static bool VerifyOSDependency(ref PlatformDetector.OSDependency osd)
    {
      OperatingSystem osVersion = Environment.OSVersion;
      if ((long) osVersion.Version.Major == 4L)
        return (long) osVersion.Version.Major >= (long) osd.dwMajorVersion;
      NativeMethods.OSVersionInfoEx osvi = new NativeMethods.OSVersionInfoEx();
      osvi.dwOSVersionInfoSize = (uint) Marshal.SizeOf((object) osvi);
      osvi.dwMajorVersion = osd.dwMajorVersion;
      osvi.dwMinorVersion = osd.dwMinorVersion;
      osvi.dwBuildNumber = osd.dwBuildNumber;
      osvi.dwPlatformId = 0U;
      osvi.szCSDVersion = (string) null;
      osvi.wServicePackMajor = osd.wServicePackMajor;
      osvi.wServicePackMinor = osd.wServicePackMinor;
      osvi.wSuiteMask = osd.suiteName != null ? (ushort) PlatformDetector.NameMap.MapNameToMask(osd.suiteName, (PlatformDetector.NameMap[]) PlatformDetector.Suites) : (ushort) 0;
      osvi.bProductType = osd.productName != null ? (byte) PlatformDetector.NameMap.MapNameToMask(osd.productName, (PlatformDetector.NameMap[]) PlatformDetector.Products) : (byte) 0;
      osvi.bReserved = (byte) 0;
      ulong ConditionMask = 0;
      uint dwTypeMask = (uint) (2 | ((int) osd.dwMinorVersion != 0 ? 1 : 0) | ((int) osd.dwBuildNumber != 0 ? 4 : 0) | (osd.suiteName != null ? 64 : 0) | (osd.productName != null ? 128 : 0) | ((int) osd.wServicePackMajor != 0 ? 32 : 0) | ((int) osd.wServicePackMinor != 0 ? 16 : 0));
      ulong num = NativeMethods.VerSetConditionMask(ConditionMask, 2U, (byte) 3);
      if ((int) osd.dwMinorVersion != 0)
        num = NativeMethods.VerSetConditionMask(num, 1U, (byte) 3);
      if ((int) osd.dwBuildNumber != 0)
        num = NativeMethods.VerSetConditionMask(num, 4U, (byte) 3);
      if (osd.suiteName != null)
        num = NativeMethods.VerSetConditionMask(num, 64U, (byte) 6);
      if (osd.productName != null)
        num = NativeMethods.VerSetConditionMask(num, 128U, (byte) 1);
      if ((int) osd.wServicePackMajor != 0)
        num = NativeMethods.VerSetConditionMask(num, 32U, (byte) 3);
      if ((int) osd.wServicePackMinor != 0)
        num = NativeMethods.VerSetConditionMask(num, 16U, (byte) 3);
      bool flag = NativeMethods.VerifyVersionInfo(osvi, dwTypeMask, num);
      if (!flag)
      {
        int lastWin32Error = Marshal.GetLastWin32Error();
        if (lastWin32Error != 1150)
          throw new Win32Exception(lastWin32Error);
      }
      return flag;
    }

    public static bool VerifyGACDependency(NativeMethods.IAssemblyCache AssemblyCache, bool targetOtherClr, NativeMethods.CCorRuntimeHost RuntimeHost, ReferenceIdentity refId, string tempDir)
    {
      if (string.Compare(refId.ProcessorArchitecture, "msil", StringComparison.OrdinalIgnoreCase) == 0 || !PlatformDetector.VerifyGACDependencyXP(refId, tempDir))
        return PlatformDetector.VerifyGACDependencyWhidbey(AssemblyCache, targetOtherClr, RuntimeHost, refId);
      return true;
    }

    public static bool VerifyGACDependencyWhidbey(NativeMethods.IAssemblyCache AssemblyCache, bool targetOtherClr, NativeMethods.CCorRuntimeHost RuntimeHost, ReferenceIdentity refId)
    {
      string str = refId.ToString();
      string text;
      if (targetOtherClr)
      {
        try
        {
          text = RuntimeHost.ApplyPolicyInOtherRuntime(str);
        }
        catch (ArgumentException ex)
        {
          return false;
        }
        catch (COMException ex)
        {
          return false;
        }
      }
      else
      {
        try
        {
          text = AppDomain.CurrentDomain.ApplyPolicy(str);
        }
        catch (ArgumentException ex)
        {
          return false;
        }
        catch (COMException ex)
        {
          return false;
        }
      }
      ReferenceIdentity referenceIdentity = new ReferenceIdentity(text);
      referenceIdentity.ProcessorArchitecture = refId.ProcessorArchitecture;
      string assemblyName = referenceIdentity.ToString();
      Logger.AddPhaseInformation(Resources.GetString("DetectingDependentAssembly"), (object) str, (object) assemblyName);
      SystemUtils.AssemblyInfo assemblyInfo = SystemUtils.QueryAssemblyInfo(AssemblyCache, SystemUtils.QueryAssemblyInfoFlags.All, assemblyName);
      if (assemblyInfo != null || referenceIdentity.ProcessorArchitecture != null)
        return assemblyInfo != null;
      NativeMethods.IAssemblyName pName;
      NativeMethods.CreateAssemblyNameObject(out pName, referenceIdentity.ToString(), 1U, IntPtr.Zero);
      NativeMethods.IAssemblyEnum ppEnum;
      NativeMethods.CreateAssemblyEnum(out ppEnum, (NativeMethods.IApplicationContext) null, pName, 2U, IntPtr.Zero);
      return ppEnum.GetNextAssembly((NativeMethods.IApplicationContext) null, out pName, 0U) == 0;
    }

    public static bool VerifyGACDependencyXP(ReferenceIdentity refId, string tempDir)
    {
      if (!PlatformSpecific.OnXPOrAbove)
        return false;
      using (TempFile tempFile = new TempFile(tempDir, ".manifest"))
      {
        ManifestGenerator.GenerateGACDetectionManifest(refId, tempFile.Path);
        IntPtr actCtxW = NativeMethods.CreateActCtxW(new NativeMethods.ACTCTXW(tempFile.Path));
        if (!(actCtxW != NativeMethods.INVALID_HANDLE_VALUE))
          return false;
        NativeMethods.ReleaseActCtx(actCtxW);
        return true;
      }
    }

    public static bool IsWin8orLater()
    {
      PlatformDetector.OSDependency osd = new PlatformDetector.OSDependency(6U, 2U, 0U, (ushort) 0, (ushort) 0, (string) null, (string) null);
      return PlatformDetector.VerifyOSDependency(ref osd);
    }

    public static void VerifyPlatformDependencies(AssemblyManifest appManifest, AssemblyManifest deployManifest, string tempDir)
    {
      Logger.AddMethodCall("VerifyPlatformDependencies called.");
      string str1 = (string) null;
      Uri supportUrl1 = deployManifest.Description.SupportUri;
      bool flag1 = false;
      DependentOS dependentOs = appManifest.DependentOS;
      if (dependentOs != null)
      {
        PlatformDetector.OSDependency osd = new PlatformDetector.OSDependency((uint) dependentOs.MajorVersion, (uint) dependentOs.MinorVersion, (uint) dependentOs.BuildNumber, (ushort) dependentOs.ServicePackMajor, (ushort) dependentOs.ServicePackMinor, (string) null, (string) null);
        if (!PlatformDetector.VerifyOSDependency(ref osd))
        {
          StringBuilder stringBuilder = new StringBuilder();
          string str2 = ((int) dependentOs.MajorVersion).ToString() + "." + (object) dependentOs.MinorVersion + "." + (object) dependentOs.BuildNumber + "." + (object) dependentOs.ServicePackMajor + (object) dependentOs.ServicePackMinor;
          stringBuilder.AppendFormat(Resources.GetString("PlatformMicrosoftWindowsOperatingSystem"), (object) str2);
          string str3 = stringBuilder.ToString();
          if (dependentOs.SupportUrl != (Uri) null)
            supportUrl1 = dependentOs.SupportUrl;
          throw new DependentPlatformMissingException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("ErrorMessage_PlatformDetectionFailed"), new object[1]{ (object) str3 }), supportUrl1);
        }
      }
      if (PlatformDetector.IsWin8orLater() && !appManifest.EntryPoints[0].HostInBrowser)
        flag1 = true;
      Version clrVersion = Constants.V2CLRVersion;
      string clrVersionString = clrVersion.ToString(3);
      string processorArchitecture = appManifest.Identity.ProcessorArchitecture;
      Uri supportUrl2 = supportUrl1;
      if (appManifest.CLRDependentAssembly != null)
      {
        clrVersion = appManifest.CLRDependentAssembly.Identity.Version;
        clrVersionString = clrVersion.ToString(3);
        processorArchitecture = appManifest.CLRDependentAssembly.Identity.ProcessorArchitecture;
        if (appManifest.CLRDependentAssembly.SupportUrl != (Uri) null)
          supportUrl2 = appManifest.CLRDependentAssembly.SupportUrl;
        if (appManifest.CLRDependentAssembly.Description != null)
          str1 = appManifest.CLRDependentAssembly.Description;
      }
      if (deployManifest.CompatibleFrameworks != null)
      {
        bool flag2 = false;
        for (int index = 0; index < deployManifest.CompatibleFrameworks.Frameworks.Count; ++index)
        {
          if (PlatformDetector.CheckCompatibleFramework(deployManifest.CompatibleFrameworks.Frameworks[index], ref clrVersion, ref clrVersionString, processorArchitecture))
          {
            flag2 = true;
            break;
          }
        }
        if (!flag2)
        {
          Uri supportUrl3 = !(deployManifest.CompatibleFrameworks.SupportUrl != (Uri) null) ? supportUrl2 : deployManifest.CompatibleFrameworks.SupportUrl;
          if (flag1)
            return;
          throw new CompatibleFrameworkMissingException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("ErrorMessage_CompatiblePlatformDetectionFailed"), new object[1]
          {
            (object) PlatformDetector.FormatFrameworkString(deployManifest.CompatibleFrameworks.Frameworks[0])
          }), supportUrl3, deployManifest.CompatibleFrameworks);
        }
      }
      else
      {
        if (clrVersion >= Constants.V4CLRVersion)
          throw new InvalidDeploymentException(ExceptionTypes.ManifestSemanticValidation, Resources.GetString("Ex_SemanticallyInvalidDeploymentManifest"), (Exception) new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_DepMissingCompatibleFrameworks")));
        if (!NativeMethods.VerifyCLRVersionInfo(clrVersion, processorArchitecture))
        {
          StringBuilder stringBuilder = new StringBuilder();
          if (str1 == null)
          {
            stringBuilder.AppendFormat(Resources.GetString("PlatformMicrosoftCommonLanguageRuntime"), (object) clrVersionString);
            str1 = stringBuilder.ToString();
          }
          Uri supportUrl3 = supportUrl2;
          if (flag1)
            return;
          throw new SupportedRuntimeMissingException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("ErrorMessage_PlatformDetectionFailed"), new object[1]{ (object) str1 }), supportUrl3, clrVersionString);
        }
      }
      Logger.AddPhaseInformation(Resources.GetString("CompatibleRuntimeFound"), new object[1]
      {
        (object) clrVersionString
      });
      bool flag3 = false;
      if (clrVersion < Constants.V4CLRVersion)
        flag3 = true;
      NativeMethods.CCorRuntimeHost RuntimeHost = (NativeMethods.CCorRuntimeHost) null;
      try
      {
        NativeMethods.IAssemblyCache assemblyCacheInterface = NativeMethods.GetAssemblyCacheInterface(clrVersionString, flag3, out RuntimeHost);
        if (assemblyCacheInterface == null || flag3 && RuntimeHost == null)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.AppendFormat(Resources.GetString("PlatformMicrosoftCommonLanguageRuntime"), (object) clrVersionString);
          throw new DependentPlatformMissingException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("ErrorMessage_PlatformDetectionFailed"), new object[1]
          {
            (object) stringBuilder.ToString()
          }), supportUrl2);
        }
        bool flag2 = false;
        bool flag4 = false;
        if (flag3 && !PolicyKeys.SkipSKUDetection())
        {
          foreach (DependentAssembly dependentAssembly in appManifest.DependentAssemblies)
          {
            if (dependentAssembly.IsPreRequisite && PlatformDetector.IsNetFX35SP1ClientSignatureAsm(dependentAssembly.Identity))
              flag2 = true;
            if (dependentAssembly.IsPreRequisite && PlatformDetector.IsNetFX35SP1FullSignatureAsm(dependentAssembly.Identity))
              flag4 = true;
          }
          if (PlatformDetector.GetPlatformNetFx35SKU(assemblyCacheInterface, flag3, RuntimeHost, tempDir) == PlatformDetector.NetFX35SP1SKU.Client35SP1 && !flag2 && !flag4)
            throw new DependentPlatformMissingException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("ErrorMessage_PlatformDetectionFailed"), new object[1]
            {
              (object) ".NET Framework 3.5 SP1"
            }));
        }
        foreach (DependentAssembly dependentAssembly in appManifest.DependentAssemblies)
        {
          if (dependentAssembly.IsPreRequisite && !PlatformDetector.IsCLRDependencyText(dependentAssembly.Identity.Name))
          {
            if (!flag3 && (PlatformDetector.IsNetFX35SP1ClientSignatureAsm(dependentAssembly.Identity) || PlatformDetector.IsNetFX35SP1FullSignatureAsm(dependentAssembly.Identity) || "framework".Equals(dependentAssembly.Group, StringComparison.OrdinalIgnoreCase)))
              Logger.AddPhaseInformation(Resources.GetString("SkippingSentinalDependentAssembly"), new object[1]
              {
                (object) dependentAssembly.Identity.ToString()
              });
            else if (!PlatformDetector.VerifyGACDependency(assemblyCacheInterface, flag3, RuntimeHost, dependentAssembly.Identity, tempDir))
            {
              string description;
              if (dependentAssembly.Description != null)
              {
                description = dependentAssembly.Description;
              }
              else
              {
                ReferenceIdentity identity = dependentAssembly.Identity;
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat(Resources.GetString("PlatformDependentAssemblyVersion"), (object) identity.Name, (object) identity.Version);
                description = stringBuilder.ToString();
              }
              if (dependentAssembly.SupportUrl != (Uri) null)
                supportUrl1 = dependentAssembly.SupportUrl;
              throw new DependentPlatformMissingException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("ErrorMessage_PlatformGACDetectionFailed"), new object[1]
              {
                (object) description
              }), supportUrl1);
            }
          }
        }
      }
      finally
      {
        if (RuntimeHost != null)
          RuntimeHost.Dispose();
      }
    }

    private static bool IsNetFX35SP1ClientSignatureAsm(ReferenceIdentity ra)
    {
      return new DefinitionIdentity("Sentinel.v3.5Client, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a,processorArchitecture=msil").Matches(ra, true);
    }

    private static bool IsNetFX35SP1FullSignatureAsm(ReferenceIdentity ra)
    {
      return new DefinitionIdentity("System.Data.Entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089,processorArchitecture=msil").Matches(ra, true);
    }

    private static PlatformDetector.NetFX35SP1SKU GetPlatformNetFx35SKU(NativeMethods.IAssemblyCache AssemblyCache, bool targetOtherCLR, NativeMethods.CCorRuntimeHost RuntimeHost, string tempDir)
    {
      ReferenceIdentity refId1 = new ReferenceIdentity("Sentinel.v3.5Client, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a,processorArchitecture=msil");
      ReferenceIdentity refId2 = new ReferenceIdentity("System.Data.Entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089,processorArchitecture=msil");
      bool flag1 = false;
      bool flag2 = false;
      if (PlatformDetector.VerifyGACDependency(AssemblyCache, targetOtherCLR, RuntimeHost, refId1, tempDir))
        flag1 = true;
      if (PlatformDetector.VerifyGACDependency(AssemblyCache, targetOtherCLR, RuntimeHost, refId2, tempDir))
        flag2 = true;
      if (flag1 && !flag2)
        return PlatformDetector.NetFX35SP1SKU.Client35SP1;
      return flag1 & flag2 ? PlatformDetector.NetFX35SP1SKU.Full35SP1 : PlatformDetector.NetFX35SP1SKU.No35SP1;
    }

    private enum NetFX35SP1SKU
    {
      No35SP1,
      Client35SP1,
      Full35SP1,
    }

    public class OSDependency
    {
      public uint dwMajorVersion;
      public uint dwMinorVersion;
      public uint dwBuildNumber;
      public ushort wServicePackMajor;
      public ushort wServicePackMinor;
      public string suiteName;
      public string productName;

      public OSDependency()
      {
      }

      public OSDependency(uint dwMajorVersion, uint dwMinorVersion, uint dwBuildNumber, ushort wServicePackMajor, ushort wServicePackMinor, string suiteName, string productName)
      {
        this.dwMajorVersion = dwMajorVersion;
        this.dwMinorVersion = dwMinorVersion;
        this.dwBuildNumber = dwBuildNumber;
        this.wServicePackMajor = wServicePackMajor;
        this.wServicePackMinor = wServicePackMinor;
        this.suiteName = suiteName;
        this.productName = productName;
      }

      public OSDependency(NativeMethods.OSVersionInfoEx osvi)
      {
        this.dwMajorVersion = osvi.dwMajorVersion;
        this.dwMinorVersion = osvi.dwMinorVersion;
        this.dwMajorVersion = osvi.dwBuildNumber;
        this.dwMajorVersion = (uint) osvi.wServicePackMajor;
        this.dwMajorVersion = (uint) osvi.wServicePackMinor;
        this.suiteName = PlatformDetector.NameMap.MapMaskToName((uint) osvi.wSuiteMask, (PlatformDetector.NameMap[]) PlatformDetector.Suites);
        this.productName = PlatformDetector.NameMap.MapMaskToName((uint) osvi.bProductType, (PlatformDetector.NameMap[]) PlatformDetector.Products);
      }
    }

    public class NameMap
    {
      public string name;
      public uint mask;

      public NameMap(string Name, uint Mask)
      {
        this.name = Name;
        this.mask = Mask;
      }

      public static uint MapNameToMask(string name, PlatformDetector.NameMap[] nmArray)
      {
        foreach (PlatformDetector.NameMap nm in nmArray)
        {
          if (nm.name == name)
            return nm.mask;
        }
        return 0;
      }

      public static string MapMaskToName(uint mask, PlatformDetector.NameMap[] nmArray)
      {
        foreach (PlatformDetector.NameMap nm in nmArray)
        {
          if ((int) nm.mask == (int) mask)
            return nm.name;
        }
        return (string) null;
      }
    }

    public class Suite : PlatformDetector.NameMap
    {
      public Suite(string Name, uint Mask)
        : base(Name, Mask)
      {
      }
    }

    public class Product : PlatformDetector.NameMap
    {
      public Product(string Name, uint Mask)
        : base(Name, Mask)
      {
      }
    }
  }
}
