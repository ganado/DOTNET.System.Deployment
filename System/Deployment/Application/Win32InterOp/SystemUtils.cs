// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.Win32InterOp.SystemUtils
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.ComponentModel;
using System.Deployment.Internal.Isolation;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Deployment.Application.Win32InterOp
{
  internal class SystemUtils
  {
    private const int MAX_CLR_VERSION_LENGTH = 24;

    public static byte[] GetManifestFromPEResources(string filePath)
    {
      IntPtr zero1 = IntPtr.Zero;
      IntPtr hModule = IntPtr.Zero;
      IntPtr zero2 = IntPtr.Zero;
      IntPtr zero3 = IntPtr.Zero;
      IntPtr hFile = new IntPtr(0);
      uint dwFlags = 2;
      byte[] destination = (byte[]) null;
      try
      {
        hModule = System.Deployment.Application.NativeMethods.LoadLibraryEx(filePath, hFile, dwFlags);
        int lastWin32Error1 = Marshal.GetLastWin32Error();
        if (hModule == IntPtr.Zero)
          SystemUtils.Win32LoadExceptionHelper(lastWin32Error1, "Ex_Win32LoadException", filePath);
        IntPtr resource = System.Deployment.Application.NativeMethods.FindResource(hModule, "#1", "#24");
        if (resource != IntPtr.Zero)
        {
          uint num = System.Deployment.Application.NativeMethods.SizeofResource(hModule, resource);
          int lastWin32Error2 = Marshal.GetLastWin32Error();
          if ((int) num == 0)
            SystemUtils.Win32LoadExceptionHelper(lastWin32Error2, "Ex_Win32ResourceLoadException", filePath);
          IntPtr hglobal = System.Deployment.Application.NativeMethods.LoadResource(hModule, resource);
          int lastWin32Error3 = Marshal.GetLastWin32Error();
          if (hglobal == IntPtr.Zero)
            SystemUtils.Win32LoadExceptionHelper(lastWin32Error3, "Ex_Win32ResourceLoadException", filePath);
          IntPtr source = System.Deployment.Application.NativeMethods.LockResource(hglobal);
          if (source == IntPtr.Zero)
            throw new Win32Exception(33);
          destination = new byte[(int) num];
          Marshal.Copy(source, destination, 0, (int) num);
        }
      }
      finally
      {
        if (hModule != IntPtr.Zero)
        {
          bool flag = System.Deployment.Application.NativeMethods.FreeLibrary(hModule);
          int lastWin32Error = Marshal.GetLastWin32Error();
          if (!flag)
            throw new Win32Exception(lastWin32Error);
        }
      }
      return destination;
    }

    private static void Win32LoadExceptionHelper(int win32ErrorCode, string resourceId, string filePath)
    {
      string fileName = Path.GetFileName(filePath);
      string message = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString(resourceId), new object[2]
      {
        (object) fileName,
        (object) Convert.ToString(win32ErrorCode, 16)
      });
      throw new Win32Exception(win32ErrorCode, message);
    }

    internal static SystemUtils.AssemblyInfo QueryAssemblyInfo(System.Deployment.Application.NativeMethods.IAssemblyCache AssemblyCache, SystemUtils.QueryAssemblyInfoFlags flags, string assemblyName)
    {
      string assemblyName1 = assemblyName;
      SystemUtils.AssemblyInfo assemblyInfo1 = new SystemUtils.AssemblyInfo();
      System.Deployment.Application.NativeMethods.AssemblyInfoInternal assemblyInfo2 = new System.Deployment.Application.NativeMethods.AssemblyInfoInternal();
      if ((flags & SystemUtils.QueryAssemblyInfoFlags.GetCurrentPath) != (SystemUtils.QueryAssemblyInfoFlags) 0)
      {
        assemblyInfo2.cchBuf = 1024;
        assemblyInfo2.currentAssemblyPathBuf = Marshal.AllocHGlobal(assemblyInfo2.cchBuf * 2);
      }
      else
      {
        assemblyInfo2.cchBuf = 0;
        assemblyInfo2.currentAssemblyPathBuf = (IntPtr) 0;
      }
      try
      {
        AssemblyCache.QueryAssemblyInfo((int) flags, assemblyName1, ref assemblyInfo2);
      }
      catch (Exception ex)
      {
        if (ExceptionUtility.IsHardException(ex))
        {
          throw;
        }
        else
        {
          Logger.AddInternalState("Exception thrown : " + ex.GetType().ToString() + ":" + ex.Message);
          assemblyInfo1 = (SystemUtils.AssemblyInfo) null;
        }
      }
      if (assemblyInfo1 != null)
      {
        assemblyInfo1.AssemblyInfoSizeInByte = assemblyInfo2.cbAssemblyInfo;
        assemblyInfo1.AssemblyFlags = (SystemUtils.AssemblyInfoFlags) assemblyInfo2.assemblyFlags;
        assemblyInfo1.AssemblySizeInKB = assemblyInfo2.assemblySizeInKB;
        if ((flags & SystemUtils.QueryAssemblyInfoFlags.GetCurrentPath) != (SystemUtils.QueryAssemblyInfoFlags) 0)
        {
          assemblyInfo1.CurrentAssemblyPath = Marshal.PtrToStringUni(assemblyInfo2.currentAssemblyPathBuf);
          Marshal.FreeHGlobal(assemblyInfo2.currentAssemblyPathBuf);
        }
      }
      return assemblyInfo1;
    }

    internal static System.Deployment.Application.DefinitionIdentity GetDefinitionIdentityFromManagedAssembly(string filePath)
    {
      Guid guidOfType = IsolationInterop.GetGuidOfType(typeof (IReferenceIdentity));
      System.Deployment.Application.ReferenceIdentity refId = new System.Deployment.Application.ReferenceIdentity((IReferenceIdentity) System.Deployment.Application.NativeMethods.GetAssemblyIdentityFromFile(filePath, ref guidOfType));
      string processorArchitecture = refId.ProcessorArchitecture;
      if (processorArchitecture != null)
        refId.ProcessorArchitecture = processorArchitecture.ToLower(CultureInfo.InvariantCulture);
      System.Deployment.Application.DefinitionIdentity definitionIdentity = new System.Deployment.Application.DefinitionIdentity(refId);
      Logger.AddInternalState("Managed Assembly Identity = " + definitionIdentity.ToString());
      return definitionIdentity;
    }

    internal static void CheckSupportedImageAndCLRVersions(string path)
    {
      Logger.AddMethodCall("CheckSupportedImageAndCLRVersions(" + path + ") called.");
      StringBuilder stringBuilder = new StringBuilder(24);
      uint dwLength;
      try
      {
        System.Deployment.Application.NativeMethods.GetFileVersion(path, stringBuilder, (uint) stringBuilder.Capacity, out dwLength);
      }
      catch (BadImageFormatException ex)
      {
        throw;
      }
      if ((int) stringBuilder[0] != 118)
        throw new InvalidDeploymentException(ExceptionTypes.ClrValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_InvalidCLRVersionInFile"), new object[2]
        {
          (object) stringBuilder,
          (object) Path.GetFileName(path)
        }));
      Version version = new Version(stringBuilder.ToString(1, stringBuilder.Length - 1));
      if ((long) version.Major < 2L)
        throw new InvalidDeploymentException(ExceptionTypes.ClrValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_ImageVersionCLRNotSupported"), new object[2]
        {
          (object) version,
          (object) Path.GetFileName(path)
        }));
      uint runtimeInfoFlags = 465;
      uint dwDirectoryLength;
      System.Deployment.Application.NativeMethods.GetRequestedRuntimeInfo(path, (string) null, (string) null, 0U, runtimeInfoFlags, (StringBuilder) null, 0U, out dwDirectoryLength, stringBuilder, (uint) stringBuilder.Capacity, out dwLength);
      if ((int) stringBuilder[0] != 118)
        throw new FormatException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_InvalidCLRVersionInFile"), new object[2]
        {
          (object) stringBuilder,
          (object) Path.GetFileName(path)
        }));
      string str = stringBuilder.ToString(1, stringBuilder.Length - 1);
      int length = str.IndexOf(".", StringComparison.Ordinal);
      if (uint.Parse(length >= 0 ? str.Substring(0, length) : str, (IFormatProvider) CultureInfo.InvariantCulture) < 2U)
        throw new InvalidDeploymentException(ExceptionTypes.ClrValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_RuntimeVersionCLRNotSupported"), new object[2]
        {
          (object) str,
          (object) Path.GetFileName(path)
        }));
    }

    private enum RUNTIME_INFO_FLAGS : uint
    {
      RUNTIME_INFO_UPGRADE_VERSION = 1,
      RUNTIME_INFO_REQUEST_IA64 = 2,
      RUNTIME_INFO_REQUEST_AMD64 = 4,
      RUNTIME_INFO_REQUEST_X86 = 8,
      RUNTIME_INFO_DONT_RETURN_DIRECTORY = 16,
      RUNTIME_INFO_DONT_RETURN_VERSION = 32,
      RUNTIME_INFO_DONT_SHOW_ERROR_DIALOG = 64,
      RUNTIME_INFO_CONSIDER_POST_2_0 = 128,
      RUNTIME_INFO_EMULATE_EXE_LAUNCH = 256,
    }

    internal enum AssemblyInfoFlags
    {
      Installed = 1,
      PayLoadResident = 2,
    }

    [Flags]
    internal enum QueryAssemblyInfoFlags
    {
      Validate = 1,
      GetSize = 2,
      GetCurrentPath = 4,
      All = GetCurrentPath | GetSize | Validate,
    }

    internal class AssemblyInfo
    {
      private int assemblyInfoSizeInByte;
      private SystemUtils.AssemblyInfoFlags assemblyFlags;
      private long assemblySizeInKB;
      private string currentAssemblyPath;

      internal int AssemblyInfoSizeInByte
      {
        set
        {
          this.assemblyInfoSizeInByte = value;
        }
      }

      internal SystemUtils.AssemblyInfoFlags AssemblyFlags
      {
        set
        {
          this.assemblyFlags = value;
        }
      }

      internal long AssemblySizeInKB
      {
        set
        {
          this.assemblySizeInKB = value;
        }
      }

      internal string CurrentAssemblyPath
      {
        set
        {
          this.currentAssemblyPath = value;
        }
      }
    }
  }
}
