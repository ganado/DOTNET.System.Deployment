// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.NativeMethods
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using Microsoft.Win32.SafeHandles;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Text;

namespace System.Deployment.Application
{
  internal static class NativeMethods
  {
    internal static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
    private static Guid _metaHostPolicyGuid = new Guid(3793290901U, (ushort) 30642, (ushort) 18734, (byte) 142, (byte) 20, (byte) 196, (byte) 179, (byte) 167, (byte) 253, (byte) 213, (byte) 147);
    private static Guid _clrRuntimeInfoGuid = new Guid(3174683090U, (ushort) 47663, (ushort) 18538, (byte) 137, (byte) 176, (byte) 180, (byte) 176, (byte) 203, (byte) 70, (byte) 104, (byte) 145);
    private static Guid _metaHostPolicyClsIdGuid = new Guid(784127130, (short) 6983, (short) 19041, (byte) 177, (byte) 58, (byte) 74, (byte) 3, (byte) 112, (byte) 30, (byte) 89, (byte) 75);
    private static Guid _corRuntimeHostClsIdGuid = new Guid(3408881443U, (ushort) 43834, (ushort) 4562, (byte) 156, (byte) 64, (byte) 0, (byte) 192, (byte) 79, (byte) 163, (byte) 10, (byte) 62);
    private static Guid _corRuntimeHostInterfaceIdGuid = new Guid(3408881442U, (ushort) 43834, (ushort) 4562, (byte) 156, (byte) 64, (byte) 0, (byte) 192, (byte) 79, (byte) 163, (byte) 10, (byte) 62);
    private static Guid _metaHostGuid = new Guid(3543325598U, (ushort) 47539, (ushort) 16677, (byte) 130, (byte) 7, (byte) 161, (byte) 72, (byte) 132, (byte) 245, (byte) 50, (byte) 22);
    private static Guid _metaHostClsId = new Guid(2457868429U, (ushort) 3726, (ushort) 18535, (byte) 179, (byte) 12, (byte) 127, (byte) 168, (byte) 56, (byte) 132, (byte) 232, (byte) 222);
    public const ushort PROCESSOR_ARCHITECTURE_INTEL = 0;
    public const ushort PROCESSOR_ARCHITECTURE_IA64 = 6;
    public const ushort PROCESSOR_ARCHITECTURE_AMD64 = 9;

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern void GetSystemInfo([MarshalAs(UnmanagedType.Struct)] ref NativeMethods.SYSTEM_INFO sysInfo);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern void GetNativeSystemInfo([MarshalAs(UnmanagedType.Struct)] ref NativeMethods.SYSTEM_INFO sysInfo);

    [DllImport("kernel32.dll", SetLastError = true, BestFitMapping = false)]
    public static extern bool VerifyVersionInfo([In, Out] NativeMethods.OSVersionInfoEx osvi, [In] uint dwTypeMask, [In] ulong dwConditionMask);

    [DllImport("kernel32.dll")]
    public static extern ulong VerSetConditionMask([In] ulong ConditionMask, [In] uint TypeMask, [In] byte Condition);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
    public static extern IntPtr LoadLibraryEx(string lpModuleName, IntPtr hFile, uint dwFlags);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
    public static extern IntPtr LoadLibrary(string lpModuleName);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true, BestFitMapping = false)]
    public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool FreeLibrary(IntPtr hModule);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
    public static extern IntPtr FindResource(IntPtr hModule, string lpName, string lpType);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr LoadResource(IntPtr hModule, IntPtr handle);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr LockResource(IntPtr hglobal);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern uint SizeofResource(IntPtr hModule, IntPtr handle);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern bool CloseHandle(HandleRef handle);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
    internal static extern int GetShortPathName(string LongPath, [Out] StringBuilder ShortPath, int BufferSize);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
    internal static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

    [DllImport("clr.dll", CharSet = CharSet.Unicode, BestFitMapping = false, PreserveSig = false)]
    internal static extern void CorLaunchApplication(uint hostType, string applicationFullName, int manifestPathsCount, string[] manifestPaths, int activationDataCount, string[] activationData, NativeMethods.PROCESS_INFORMATION processInformation);

    [DllImport("clr.dll", PreserveSig = false)]
    internal static extern void CreateAssemblyCache(out NativeMethods.IAssemblyCache ppAsmCache, int reserved);

    [DllImport("clr.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
    [return: MarshalAs(UnmanagedType.IUnknown)]
    internal static extern object GetAssemblyIdentityFromFile([MarshalAs(UnmanagedType.LPWStr), In] string filePath, [In] ref Guid riid);

    [DllImport("clr.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
    internal static extern void CreateAssemblyNameObject(out NativeMethods.IAssemblyName ppEnum, string szAssemblyName, uint dwFlags, IntPtr pvReserved);

    [DllImport("clr.dll", CharSet = CharSet.Auto, PreserveSig = false)]
    internal static extern void CreateAssemblyEnum(out NativeMethods.IAssemblyEnum ppEnum, NativeMethods.IApplicationContext pAppCtx, NativeMethods.IAssemblyName pName, uint dwFlags, IntPtr pvReserved);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    internal static extern IntPtr CreateActCtxW([In] NativeMethods.ACTCTXW actCtx);

    [DllImport("kernel32.dll")]
    internal static extern void ReleaseActCtx([In] IntPtr hActCtx);

    internal static string GetLoadedModulePath(string moduleName)
    {
      string str = (string) null;
      IntPtr moduleHandle = NativeMethods.GetModuleHandle(moduleName);
      if (moduleHandle != IntPtr.Zero)
      {
        StringBuilder fileName = new StringBuilder(260);
        if (NativeMethods.GetModuleFileName(moduleHandle, fileName, fileName.Capacity) > 0)
          str = fileName.ToString();
      }
      return str;
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
    public static extern IntPtr GetModuleHandle(string moduleName);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
    public static extern int GetModuleFileName(IntPtr module, [Out] StringBuilder fileName, int size);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern uint GetCurrentThreadId();

    [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool CreateUrlCacheEntry([In] string urlName, [In] int expectedFileSize, [In] string fileExtension, [Out] StringBuilder fileName, [In] int dwReserved);

    [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool CommitUrlCacheEntry([In] string lpszUrlName, [In] string lpszLocalFileName, [In] long ExpireTime, [In] long LastModifiedTime, [In] uint CacheEntryType, [In] string lpHeaderInfo, [In] int dwHeaderSize, [In] string lpszFileExtension, [In] string lpszOriginalUrl);

    [SecurityCritical]
    [DllImport("mscoree.dll", EntryPoint = "CLRCreateInstance", PreserveSig = false)]
    [return: MarshalAs(UnmanagedType.Interface)]
    private static extern void GetClrMetaHostPolicy(ref Guid clsid, ref Guid iid, [MarshalAs(UnmanagedType.Interface)] out NativeMethods.IClrMetaHostPolicy ClrMetaHostPolicy);

    public static NativeMethods.IAssemblyCache GetAssemblyCacheInterface(string CLRVersionString, bool FetchRuntimeHost, out NativeMethods.CCorRuntimeHost RuntimeHost)
    {
      NativeMethods.IClrMetaHostPolicy ClrMetaHostPolicy = (NativeMethods.IClrMetaHostPolicy) null;
      RuntimeHost = (NativeMethods.CCorRuntimeHost) null;
      NativeMethods.GetClrMetaHostPolicy(ref NativeMethods._metaHostPolicyClsIdGuid, ref NativeMethods._metaHostPolicyGuid, out ClrMetaHostPolicy);
      if (ClrMetaHostPolicy == null)
        return (NativeMethods.IAssemblyCache) null;
      StringBuilder version = new StringBuilder("v", "v65535.65535.65535".Length);
      version.Append(CLRVersionString);
      int versionLength = version.Capacity;
      int imageVersionLength = 0;
      int pdwConfigFlags = 0;
      NativeMethods.IClrRuntimeInfo requestedRuntime = (NativeMethods.IClrRuntimeInfo) ClrMetaHostPolicy.GetRequestedRuntime(NativeMethods.MetaHostPolicyFlags.MetaHostPolicyApplyUpgradePolicy, (string) null, (IStream) null, version, out versionLength, (StringBuilder) null, out imageVersionLength, out pdwConfigFlags, NativeMethods._clrRuntimeInfoGuid);
      if (requestedRuntime == null)
        return (NativeMethods.IAssemblyCache) null;
      Marshal.ThrowExceptionForHR(((NativeMethods.CoInitializeEEDelegate) Marshal.GetDelegateForFunctionPointer(requestedRuntime.GetProcAddress("CoInitializeEE"), typeof (NativeMethods.CoInitializeEEDelegate)))(0U));
      if (FetchRuntimeHost)
        RuntimeHost = new NativeMethods.CCorRuntimeHost(requestedRuntime);
      NativeMethods.CreateAssemblyCacheDelegate forFunctionPointer = (NativeMethods.CreateAssemblyCacheDelegate) Marshal.GetDelegateForFunctionPointer(requestedRuntime.GetProcAddress("CreateAssemblyCache"), typeof (NativeMethods.CreateAssemblyCacheDelegate));
      NativeMethods.IAssemblyCache ppAsmCache = (NativeMethods.IAssemblyCache) null;
      Marshal.ThrowExceptionForHR(forFunctionPointer(out ppAsmCache, 0U));
      return ppAsmCache;
    }

    public static bool VerifyCLRVersionInfo(Version v, string procArch)
    {
      bool flag = true;
      NativeMethods.IClrMetaHostPolicy ClrMetaHostPolicy = (NativeMethods.IClrMetaHostPolicy) null;
      NativeMethods.GetClrMetaHostPolicy(ref NativeMethods._metaHostPolicyClsIdGuid, ref NativeMethods._metaHostPolicyGuid, out ClrMetaHostPolicy);
      if (ClrMetaHostPolicy == null)
        return false;
      string str = v.ToString(3);
      StringBuilder version = new StringBuilder("v", "v65535.65535.65535".Length);
      version.Append(str);
      int versionLength = version.Capacity;
      int imageVersionLength = 0;
      int pdwConfigFlags = 0;
      try
      {
        if ((NativeMethods.IClrRuntimeInfo) ClrMetaHostPolicy.GetRequestedRuntime(NativeMethods.MetaHostPolicyFlags.MetaHostPolicyApplyUpgradePolicy, (string) null, (IStream) null, version, out versionLength, (StringBuilder) null, out imageVersionLength, out pdwConfigFlags, NativeMethods._clrRuntimeInfoGuid) == null)
          flag = false;
      }
      catch (COMException ex)
      {
        flag = false;
        if (ex.ErrorCode != -2146232576)
          throw;
      }
      return flag;
    }

    [SecurityCritical]
    [DllImport("mscoree.dll", EntryPoint = "CLRCreateInstance", PreserveSig = false)]
    [return: MarshalAs(UnmanagedType.Interface)]
    private static extern object nCLRCreateInstance([MarshalAs(UnmanagedType.LPStruct)] Guid clsid, [MarshalAs(UnmanagedType.LPStruct)] Guid iid);

    [SecurityCritical]
    public static void GetFileVersion(string szFileName, StringBuilder szBuffer, uint cchBuffer, out uint dwLength)
    {
      ((NativeMethods.IClrMetaHost) NativeMethods.nCLRCreateInstance(NativeMethods._metaHostClsId, NativeMethods._metaHostGuid)).GetVersionFromFile(szFileName, szBuffer, out cchBuffer);
      dwLength = cchBuffer;
    }

    [SecurityCritical]
    private static T GetClrMetaHost<T>()
    {
      return (T) NativeMethods.nCLRCreateInstance(NativeMethods._metaHostClsId, NativeMethods._metaHostGuid);
    }

    [DllImport("mscoree.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
    public static extern void GetRequestedRuntimeInfo(string pExe, string pwszVersion, string pConfigurationFile, uint startupFlags, uint runtimeInfoFlags, StringBuilder pDirectory, uint dwDirectory, out uint dwDirectoryLength, StringBuilder pVersion, uint cchBuffer, out uint dwLength);

    [DllImport("wininet.dll", CharSet = CharSet.Unicode)]
    public static extern bool InternetGetCookieW([In] string url, [In] string cookieName, [Out] StringBuilder cookieData, [In, Out] ref uint bytes);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    public static extern void SHChangeNotify(int eventID, uint flags, IntPtr item1, IntPtr item2);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    public static extern uint SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr), In] string pszPath, [In] IntPtr pbc, [MarshalAs(UnmanagedType.LPStruct), In] Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppv);

    [DllImport("Ole32.dll")]
    public static extern uint CoCreateInstance([In] ref Guid clsid, [MarshalAs(UnmanagedType.IUnknown)] object punkOuter, int context, [In] ref Guid iid, [MarshalAs(UnmanagedType.IUnknown)] out object o);

    public struct SYSTEM_INFO
    {
      internal NativeMethods._PROCESSOR_INFO_UNION uProcessorInfo;
      public uint dwPageSize;
      public IntPtr lpMinimumApplicationAddress;
      public IntPtr lpMaximumApplicationAddress;
      public IntPtr dwActiveProcessorMask;
      public uint dwNumberOfProcessors;
      public uint dwProcessorType;
      public uint dwAllocationGranularity;
      public uint dwProcessorLevel;
      public uint dwProcessorRevision;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct _PROCESSOR_INFO_UNION
    {
      [FieldOffset(0)]
      internal uint dwOemId;
      [FieldOffset(0)]
      internal ushort wProcessorArchitecture;
      [FieldOffset(2)]
      internal ushort wReserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class OSVersionInfoEx
    {
      public uint dwOSVersionInfoSize;
      public uint dwMajorVersion;
      public uint dwMinorVersion;
      public uint dwBuildNumber;
      public uint dwPlatformId;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
      public string szCSDVersion;
      public ushort wServicePackMajor;
      public ushort wServicePackMinor;
      public ushort wSuiteMask;
      public byte bProductType;
      public byte bReserved;
    }

    [Flags]
    internal enum GenericAccess : uint
    {
      GENERIC_READ = 2147483648,
      GENERIC_WRITE = 1073741824,
      GENERIC_EXECUTE = 536870912,
      GENERIC_ALL = 268435456,
    }

    internal enum CreationDisposition : uint
    {
      CREATE_NEW = 1,
      CREATE_ALWAYS = 2,
      OPEN_EXISTING = 3,
      OPEN_ALWAYS = 4,
      TRUNCATE_EXISTING = 5,
    }

    [Flags]
    internal enum ShareMode : uint
    {
      FILE_SHARE_NONE = 0,
      FILE_SHARE_READ = 1,
      FILE_SHARE_WRITE = 2,
      FILE_SHARE_DELETE = 4,
    }

    [Flags]
    internal enum FlagsAndAttributes : uint
    {
      FILE_FLAG_WRITE_THROUGH = 2147483648,
      FILE_FLAG_OVERLAPPED = 1073741824,
      FILE_FLAG_NO_BUFFERING = 536870912,
      FILE_FLAG_RANDOM_ACCESS = 268435456,
      FILE_FLAG_SEQUENTIAL_SCAN = 134217728,
      FILE_FLAG_DELETE_ON_CLOSE = 67108864,
      FILE_FLAG_BACKUP_SEMANTICS = 33554432,
      FILE_FLAG_POSIX_SEMANTICS = 16777216,
      FILE_FLAG_OPEN_REPARSE_POINT = 2097152,
      FILE_FLAG_OPEN_NO_RECALL = 1048576,
      FILE_FLAG_FIRST_PIPE_INSTANCE = 524288,
      FILE_ATTRIBUTE_READONLY = 1,
      FILE_ATTRIBUTE_HIDDEN = 2,
      FILE_ATTRIBUTE_SYSTEM = 4,
      FILE_ATTRIBUTE_DIRECTORY = 16,
      FILE_ATTRIBUTE_ARCHIVE = 32,
      FILE_ATTRIBUTE_DEVICE = 64,
      FILE_ATTRIBUTE_NORMAL = 128,
      FILE_ATTRIBUTE_TEMPORARY = 256,
      FILE_ATTRIBUTE_SPARSE_FILE = 512,
      FILE_ATTRIBUTE_REPARSE_POINT = 1024,
      FILE_ATTRIBUTE_COMPRESSED = 2048,
      FILE_ATTRIBUTE_OFFLINE = 4096,
      FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 8192,
      FILE_ATTRIBUTE_ENCRYPTED = 16384,
    }

    internal enum Win32Error
    {
      ERROR_SUCCESS = 0,
      ERROR_INVALID_FUNCTION = 1,
      ERROR_FILE_NOT_FOUND = 2,
      ERROR_PATH_NOT_FOUND = 3,
      ERROR_TOO_MANY_OPEN_FILES = 4,
      ERROR_ACCESS_DENIED = 5,
      ERROR_INVALID_HANDLE = 6,
      ERROR_NO_MORE_FILES = 18,
      ERROR_NOT_READY = 21,
      ERROR_SHARING_VIOLATION = 32,
      ERROR_FILE_EXISTS = 80,
      ERROR_INVALID_PARAMETER = 87,
      ERROR_CALL_NOT_IMPLEMENTED = 120,
      ERROR_ALREADY_EXISTS = 183,
      ERROR_FILENAME_EXCED_RANGE = 206,
    }

    internal enum HResults
    {
      HRESULT_ERROR_REVISION_MISMATCH = -2147023590,
    }

    [SuppressUnmanagedCodeSecurity]
    [StructLayout(LayoutKind.Sequential)]
    internal class PROCESS_INFORMATION
    {
      public IntPtr hProcess = IntPtr.Zero;
      public IntPtr hThread = IntPtr.Zero;
      public int dwProcessId;
      public int dwThreadId;

      ~PROCESS_INFORMATION()
      {
        this.Close();
      }

      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      internal void Close()
      {
        if (this.hProcess != IntPtr.Zero && this.hProcess != NativeMethods.INVALID_HANDLE_VALUE)
        {
          NativeMethods.CloseHandle(new HandleRef((object) this, this.hProcess));
          this.hProcess = NativeMethods.INVALID_HANDLE_VALUE;
        }
        if (!(this.hThread != IntPtr.Zero) || !(this.hThread != NativeMethods.INVALID_HANDLE_VALUE))
          return;
        NativeMethods.CloseHandle(new HandleRef((object) this, this.hThread));
        this.hThread = NativeMethods.INVALID_HANDLE_VALUE;
      }
    }

    internal struct AssemblyInfoInternal
    {
      internal int cbAssemblyInfo;
      internal int assemblyFlags;
      internal long assemblySizeInKB;
      internal IntPtr currentAssemblyPathBuf;
      internal int cchBuf;
      internal const int MaxPath = 1024;
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("e707dcde-d1cd-11d2-bab9-00c04f8eceae")]
    [ComImport]
    internal interface IAssemblyCache
    {
      void UninstallAssembly();

      void QueryAssemblyInfo(int flags, [MarshalAs(UnmanagedType.LPWStr)] string assemblyName, ref NativeMethods.AssemblyInfoInternal assemblyInfo);

      void CreateAssemblyCacheItem();

      void CreateAssemblyScavenger();

      void InstallAssembly();
    }

    internal delegate int CreateAssemblyCacheDelegate([MarshalAs(UnmanagedType.Interface)] out NativeMethods.IAssemblyCache ppAsmCache, uint reserved);

    public enum tagCOINITEE : uint
    {
      COINITEE_DEFAULT,
      COINITEE_DLL,
      COINITEE_MAIN,
    }

    internal delegate int CoInitializeEEDelegate(uint fFlags);

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("21b8916c-f28e-11d2-a473-00c04f8ef448")]
    [ComImport]
    internal interface IAssemblyEnum
    {
      [MethodImpl(MethodImplOptions.PreserveSig)]
      int GetNextAssembly(NativeMethods.IApplicationContext ppAppCtx, out NativeMethods.IAssemblyName ppName, uint dwFlags);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int Reset();

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int Clone(out NativeMethods.IAssemblyEnum ppEnum);
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("7c23ff90-33af-11d3-95da-00a024a85b51")]
    [ComImport]
    internal interface IApplicationContext
    {
      void SetContextNameObject(NativeMethods.IAssemblyName pName);

      void GetContextNameObject(out NativeMethods.IAssemblyName ppName);

      void Set([MarshalAs(UnmanagedType.LPWStr)] string szName, int pvValue, uint cbValue, uint dwFlags);

      void Get([MarshalAs(UnmanagedType.LPWStr)] string szName, out int pvValue, ref uint pcbValue, uint dwFlags);

      void GetDynamicDirectory(out int wzDynamicDir, ref uint pdwSize);
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("CD193BC0-B4BC-11d2-9833-00C04FC31D2E")]
    [ComImport]
    internal interface IAssemblyName
    {
      [MethodImpl(MethodImplOptions.PreserveSig)]
      int SetProperty(uint PropertyId, IntPtr pvProperty, uint cbProperty);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int GetProperty(uint PropertyId, IntPtr pvProperty, ref uint pcbProperty);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int Finalize();

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int GetDisplayName(IntPtr szDisplayName, ref uint pccDisplayName, uint dwDisplayFlags);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int BindToObject(object refIID, object pAsmBindSink, NativeMethods.IApplicationContext pApplicationContext, [MarshalAs(UnmanagedType.LPWStr)] string szCodeBase, long llFlags, int pvReserved, uint cbReserved, out int ppv);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int GetName(out uint lpcwBuffer, out int pwzName);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int GetVersion(out uint pdwVersionHi, out uint pdwVersionLow);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int IsEqual(NativeMethods.IAssemblyName pName, uint dwCmpFlags);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int Clone(out NativeMethods.IAssemblyName pName);
    }

    internal enum ASM_CACHE : uint
    {
      ZAP = 1,
      GAC = 2,
      DOWNLOAD = 4,
    }

    internal enum CreateAssemblyNameObjectFlags : uint
    {
      CANOF_DEFAULT,
      CANOF_PARSE_DISPLAY_NAME,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal class ACTCTXW
    {
      public uint cbSize;
      public uint dwFlags;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string lpSource;
      public ushort wProcessorArchitecture;
      public ushort wLangId;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string lpAssemblyDirectory;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string lpResourceName;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string lpApplicationName;
      public IntPtr hModule;

      public ACTCTXW(string manifestPath)
      {
        this.cbSize = (uint) Marshal.SizeOf(typeof (NativeMethods.ACTCTXW));
        this.dwFlags = 0U;
        this.lpSource = manifestPath;
      }
    }

    public enum CacheEntryFlags : uint
    {
      Normal = 1,
      Sticky = 4,
      Edited = 8,
      TrackOffline = 16,
      TrackOnline = 32,
      Sparse = 65536,
      Cookie = 1048576,
      UrlHistory = 2097152,
    }

    [SecurityCritical]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("00000100-0000-0000-C000-000000000046")]
    [ComImport]
    public interface IEnumUnknown
    {
      [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      int Next([MarshalAs(UnmanagedType.U4), In] int elementArrayLength, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.IUnknown), Out] object[] elementArray, [MarshalAs(UnmanagedType.U4)] out int fetchedElementCount);

      [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      int Skip([MarshalAs(UnmanagedType.U4), In] int count);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void Reset();

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void Clone([MarshalAs(UnmanagedType.Interface)] out NativeMethods.IEnumUnknown enumerator);
    }

    [SecurityCritical]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("BD39D1D2-BA2F-486A-89B0-B4B0CB466891")]
    [ComImport]
    public interface IClrRuntimeInfo
    {
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetVersionString([MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder buffer, [MarshalAs(UnmanagedType.U4), In, Out] ref int bufferLength);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetRuntimeDirectory([MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder buffer, [MarshalAs(UnmanagedType.U4), In, Out] ref int bufferLength);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      [return: MarshalAs(UnmanagedType.Bool)]
      bool IsLoaded([In] IntPtr processHandle);

      [LCIDConversion(3)]
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void LoadErrorString([MarshalAs(UnmanagedType.U4), In] int resourceId, [MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder buffer, [MarshalAs(UnmanagedType.U4), In, Out] ref int bufferLength);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPWStr), In] string dllName);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      IntPtr GetProcAddress([MarshalAs(UnmanagedType.LPStr), In] string procName);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      [return: MarshalAs(UnmanagedType.Interface)]
      object GetInterface([MarshalAs(UnmanagedType.LPStruct), In] Guid coClassId, [MarshalAs(UnmanagedType.LPStruct), In] Guid interfaceId);
    }

    [SecurityCritical]
    [Flags]
    public enum MetaHostPolicyFlags
    {
      MetaHostPolicyHighCompatibility = 0,
      MetaHostPolicyApplyUpgradePolicy = 8,
      MetaHostPolicyEmulateExeLaunch = 15,
    }

    [SecurityCritical]
    [Guid("E2190695-77B2-492E-8E14-C4B3A7FDD593")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface IClrMetaHostPolicy
    {
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      [return: MarshalAs(UnmanagedType.Interface)]
      object GetRequestedRuntime([ComAliasName("Microsoft.Runtime.Hosting.Interop.MetaHostPolicyFlags"), In] NativeMethods.MetaHostPolicyFlags policyFlags, [MarshalAs(UnmanagedType.LPWStr), In] string binaryPath, [MarshalAs(UnmanagedType.Interface), In] IStream configStream, [MarshalAs(UnmanagedType.LPWStr), In, Out] StringBuilder version, [MarshalAs(UnmanagedType.U4), In, Out] ref int versionLength, [MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder imageVersion, [MarshalAs(UnmanagedType.U4), In, Out] ref int imageVersionLength, [MarshalAs(UnmanagedType.U4)] out int pdwConfigFlags, [MarshalAs(UnmanagedType.LPStruct), In] Guid interfaceId);
    }

    [Guid("CB2F6722-AB3A-11d2-9C40-00C04FA30A3E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    internal interface ICorRuntimeHost
    {
      [MethodImpl(MethodImplOptions.PreserveSig)]
      int CreateLogicalThreadState();

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int DeleteLogicalThreadState();

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int SwitchInLogicalThreadState([In] ref uint pFiberCookie);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int SwitchOutLogicalThreadState(out uint FiberCookie);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int LocksHeldByLogicalThread(out uint pCount);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int MapFile(IntPtr hFile, out IntPtr hMapAddress);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int GetConfiguration([MarshalAs(UnmanagedType.IUnknown)] out object pConfiguration);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int Start();

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int Stop();

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int CreateDomain(string pwzFriendlyName, [MarshalAs(UnmanagedType.IUnknown)] object pIdentityArray, [MarshalAs(UnmanagedType.IUnknown)] out object pAppDomain);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int GetDefaultDomain([MarshalAs(UnmanagedType.IUnknown)] out object pAppDomain);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int EnumDomains(out IntPtr hEnum);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int NextDomain(IntPtr hEnum, [MarshalAs(UnmanagedType.IUnknown)] out object pAppDomain);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int CloseEnum(IntPtr hEnum);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int CreateDomainEx(string pwzFriendlyName, [MarshalAs(UnmanagedType.IUnknown)] object pSetup, [MarshalAs(UnmanagedType.IUnknown)] object pEvidence, [MarshalAs(UnmanagedType.IUnknown)] out object pAppDomain);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int CreateDomainSetup([MarshalAs(UnmanagedType.IUnknown)] out object pAppDomainSetup);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int CreateEvidence([MarshalAs(UnmanagedType.IUnknown)] out object pEvidence);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int UnloadDomain([MarshalAs(UnmanagedType.IUnknown)] object pAppDomain);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int CurrentDomain([MarshalAs(UnmanagedType.IUnknown)] out object pAppDomain);
    }

    public sealed class CCorRuntimeHost : IDisposable
    {
      private IntPtr RuntimeHostPtr = IntPtr.Zero;
      private IntPtr DomainObjectPtr = IntPtr.Zero;
      private IntPtr DomainTypePtr = IntPtr.Zero;
      private string ClrRuntimeInfoVersion = string.Empty;
      private NativeMethods.ICorRuntimeHost RuntimeHostInstance;
      private NativeMethods.CCorRuntimeHost.Host_CurrentDomain CurrentDomainFnPtr;
      private NativeMethods.CCorRuntimeHost.AppDomain_GetType GetTypeFnPtr;
      private NativeMethods.CCorRuntimeHost.Type_InvokeMember InvokeMemberFnPtr;
      private bool fDelegatesBound;

      public CCorRuntimeHost(NativeMethods.IClrRuntimeInfo RuntimeInfo)
      {
        StringBuilder buffer = new StringBuilder(260);
        int bufferLength = buffer.Capacity;
        RuntimeInfo.GetVersionString(buffer, out bufferLength);
        this.ClrRuntimeInfoVersion = buffer.ToString();
        Logger.AddMethodCall("CCorRuntimeHost.ctor called with IClrRuntimeInfo version " + this.ClrRuntimeInfoVersion, DateTime.Now);
        this.RuntimeHostInstance = (NativeMethods.ICorRuntimeHost) RuntimeInfo.GetInterface(NativeMethods._corRuntimeHostClsIdGuid, NativeMethods._corRuntimeHostInterfaceIdGuid);
      }

      public void Dispose()
      {
        this.fDelegatesBound = false;
        this.InvokeMemberFnPtr = (NativeMethods.CCorRuntimeHost.Type_InvokeMember) null;
        if (IntPtr.Zero != this.DomainTypePtr)
        {
          Marshal.Release(this.DomainTypePtr);
          this.DomainTypePtr = IntPtr.Zero;
        }
        this.GetTypeFnPtr = (NativeMethods.CCorRuntimeHost.AppDomain_GetType) null;
        if (IntPtr.Zero != this.DomainObjectPtr)
        {
          Marshal.Release(this.DomainObjectPtr);
          this.DomainObjectPtr = IntPtr.Zero;
        }
        this.CurrentDomainFnPtr = (NativeMethods.CCorRuntimeHost.Host_CurrentDomain) null;
        if (IntPtr.Zero != this.RuntimeHostPtr)
        {
          Marshal.Release(this.RuntimeHostPtr);
          this.RuntimeHostPtr = IntPtr.Zero;
        }
        this.RuntimeHostInstance = (NativeMethods.ICorRuntimeHost) null;
      }

      private void BindDelegatesToManualCOMPInvokeFunctionPointers()
      {
        if (this.fDelegatesBound)
          return;
        this.RuntimeHostInstance.Start();
        int ofs1 = 21 * IntPtr.Size;
        int ofs2 = 10 * IntPtr.Size;
        int ofs3 = 57 * IntPtr.Size;
        this.RuntimeHostPtr = Marshal.GetIUnknownForObject((object) this.RuntimeHostInstance);
        this.CurrentDomainFnPtr = (NativeMethods.CCorRuntimeHost.Host_CurrentDomain) Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(Marshal.ReadIntPtr(this.RuntimeHostPtr), ofs1), typeof (NativeMethods.CCorRuntimeHost.Host_CurrentDomain));
        IntPtr domain;
        int errorCode1 = this.CurrentDomainFnPtr(this.RuntimeHostPtr, out domain);
        if (errorCode1 < 0)
          Marshal.ThrowExceptionForHR(errorCode1);
        Guid guid = typeof (_AppDomain).GUID;
        int errorCode2 = Marshal.QueryInterface(domain, ref guid, out this.DomainObjectPtr);
        if (errorCode2 < 0)
          Marshal.ThrowExceptionForHR(errorCode2);
        this.GetTypeFnPtr = (NativeMethods.CCorRuntimeHost.AppDomain_GetType) Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(Marshal.ReadIntPtr(this.DomainObjectPtr), ofs2), typeof (NativeMethods.CCorRuntimeHost.AppDomain_GetType));
        int errorCode3 = this.GetTypeFnPtr(this.DomainObjectPtr, out this.DomainTypePtr);
        if (errorCode3 < 0)
          Marshal.ThrowExceptionForHR(errorCode3);
        this.InvokeMemberFnPtr = (NativeMethods.CCorRuntimeHost.Type_InvokeMember) Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(Marshal.ReadIntPtr(this.DomainTypePtr), ofs3), typeof (NativeMethods.CCorRuntimeHost.Type_InvokeMember));
        this.fDelegatesBound = true;
      }

      public string ApplyPolicyInOtherRuntime(string name)
      {
        if (!this.fDelegatesBound)
          this.BindDelegatesToManualCOMPInvokeFunctionPointers();
        object[] args = new object[1]{ (object) name };
        object retval = (object) null;
        int errorCode = this.InvokeMemberFnPtr(this.DomainTypePtr, "ApplyPolicy", BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod, (Binder) null, new NativeMethods.CCorRuntimeHost.VARIANT()
        {
          vt = (ushort) 13,
          data1 = this.DomainObjectPtr
        }, args, out retval);
        if (errorCode < 0)
          Marshal.ThrowExceptionForHR(errorCode);
        return retval.ToString();
      }

      [UnmanagedFunctionPointer(CallingConvention.StdCall)]
      private delegate int Host_CurrentDomain(IntPtr _this, out IntPtr domain);

      [UnmanagedFunctionPointer(CallingConvention.StdCall)]
      private delegate int AppDomain_GetType(IntPtr _this, out IntPtr domainType);

      [UnmanagedFunctionPointer(CallingConvention.StdCall)]
      private delegate int Type_InvokeMember(IntPtr _this, [MarshalAs(UnmanagedType.BStr)] string name, BindingFlags invokeAttr, Binder binder, NativeMethods.CCorRuntimeHost.VARIANT target, [MarshalAs(UnmanagedType.SafeArray)] object[] args, out object retval);

      private struct VARIANT
      {
        public ushort vt;
        public ushort wReserved1;
        public ushort wReserved2;
        public ushort wReserved3;
        public IntPtr data1;
        public IntPtr data2;
        public const ushort VT_UNKNOWN = 13;
      }
    }

    [SecurityCritical]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("D332DB9E-B9B3-4125-8207-A14884F53216")]
    [ComImport]
    public interface IClrMetaHost
    {
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      [return: MarshalAs(UnmanagedType.Interface)]
      object GetRuntime([MarshalAs(UnmanagedType.LPWStr), In] string version, [MarshalAs(UnmanagedType.LPStruct), In] Guid interfaceId);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetVersionFromFile([MarshalAs(UnmanagedType.LPWStr), In] string filePath, [MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder buffer, [MarshalAs(UnmanagedType.U4), In, Out] ref uint bufferLength);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      [return: MarshalAs(UnmanagedType.Interface)]
      NativeMethods.IEnumUnknown EnumerateInstalledRuntimes();

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      [return: MarshalAs(UnmanagedType.Interface)]
      NativeMethods.IEnumUnknown EnumerateLoadedRuntimes([In] IntPtr processHandle);

      [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      int Reserved01([In] IntPtr reserved1);
    }

    public enum SHChangeNotifyEventID
    {
      SHCNE_ASSOCCHANGED = 134217728,
    }

    public enum SHChangeNotifyFlags : uint
    {
      SHCNF_IDLIST,
    }

    internal enum SIGDN : uint
    {
      NORMALDISPLAY = 0,
      PARENTRELATIVEPARSING = 2147581953,
      DESKTOPABSOLUTEPARSING = 2147647488,
      PARENTRELATIVEEDITING = 2147684353,
      DESKTOPABSOLUTEEDITING = 2147794944,
      FILESYSPATH = 2147844096,
      URL = 2147909632,
      PARENTRELATIVEFORADDRESSBAR = 2147991553,
      PARENTRELATIVE = 2148007937,
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe")]
    [ComImport]
    public interface IShellItem
    {
      void BindToHandler(IntPtr pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid bhid, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IntPtr ppv);

      void GetParent(out NativeMethods.IShellItem ppsi);

      void GetDisplayName(NativeMethods.SIGDN sigdnName, out IntPtr ppszName);

      void GetAttributes(uint sfgaoMask, out uint psfgaoAttribs);

      void Compare(NativeMethods.IShellItem psi, uint hint, out int piOrder);
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("4CD19ADA-25A5-4A32-B3B7-347BEE5BE36B")]
    [ComImport]
    public interface IStartMenuPinnedList
    {
      void RemoveFromList(NativeMethods.IShellItem psi);
    }
  }
}
