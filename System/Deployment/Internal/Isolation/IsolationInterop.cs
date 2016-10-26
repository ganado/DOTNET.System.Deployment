// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.IsolationInterop
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Deployment.Internal.Isolation.Manifest;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation
{
  internal static class IsolationInterop
  {
    private static object _synchObject = new object();
    private static Store _userStore = (Store) null;
    private static Store _systemStore = (Store) null;
    private static volatile IIdentityAuthority _idAuth = (IIdentityAuthority) null;
    private static volatile IAppIdAuthority _appIdAuth = (IAppIdAuthority) null;
    public static Guid IID_ICMS = IsolationInterop.GetGuidOfType(typeof (ICMS));
    public static Guid IID_IDefinitionIdentity = IsolationInterop.GetGuidOfType(typeof (IDefinitionIdentity));
    public static Guid IID_IManifestInformation = IsolationInterop.GetGuidOfType(typeof (IManifestInformation));
    public static Guid IID_IEnumSTORE_ASSEMBLY = IsolationInterop.GetGuidOfType(typeof (IEnumSTORE_ASSEMBLY));
    public static Guid IID_IEnumSTORE_ASSEMBLY_FILE = IsolationInterop.GetGuidOfType(typeof (IEnumSTORE_ASSEMBLY_FILE));
    public static Guid IID_IEnumSTORE_CATEGORY = IsolationInterop.GetGuidOfType(typeof (IEnumSTORE_CATEGORY));
    public static Guid IID_IEnumSTORE_CATEGORY_INSTANCE = IsolationInterop.GetGuidOfType(typeof (IEnumSTORE_CATEGORY_INSTANCE));
    public static Guid IID_IEnumSTORE_DEPLOYMENT_METADATA = IsolationInterop.GetGuidOfType(typeof (IEnumSTORE_DEPLOYMENT_METADATA));
    public static Guid IID_IEnumSTORE_DEPLOYMENT_METADATA_PROPERTY = IsolationInterop.GetGuidOfType(typeof (IEnumSTORE_DEPLOYMENT_METADATA_PROPERTY));
    public static Guid IID_IStore = IsolationInterop.GetGuidOfType(typeof (IStore));
    public static Guid GUID_SXS_INSTALL_REFERENCE_SCHEME_OPAQUESTRING = new Guid("2ec93463-b0c3-45e1-8364-327e96aea856");
    public static Guid SXS_INSTALL_REFERENCE_SCHEME_SXS_STRONGNAME_SIGNED_PRIVATE_ASSEMBLY = new Guid("3ab20ac0-67e8-4512-8385-a487e35df3da");
    public const string IsolationDllName = "clr.dll";

    public static Store UserStore
    {
      get
      {
        if (IsolationInterop._userStore == null)
        {
          lock (IsolationInterop._synchObject)
          {
            if (IsolationInterop._userStore == null)
              IsolationInterop._userStore = new Store(IsolationInterop.GetUserStore(0U, IntPtr.Zero, ref IsolationInterop.IID_IStore) as IStore);
          }
        }
        return IsolationInterop._userStore;
      }
    }

    public static Store SystemStore
    {
      get
      {
        if (IsolationInterop._systemStore == null)
        {
          lock (IsolationInterop._synchObject)
          {
            if (IsolationInterop._systemStore == null)
              IsolationInterop._systemStore = new Store(IsolationInterop.GetSystemStore(0U, ref IsolationInterop.IID_IStore) as IStore);
          }
        }
        return IsolationInterop._systemStore;
      }
    }

    public static IIdentityAuthority IdentityAuthority
    {
      [SecuritySafeCritical] get
      {
        if (IsolationInterop._idAuth == null)
        {
          lock (IsolationInterop._synchObject)
          {
            if (IsolationInterop._idAuth == null)
              IsolationInterop._idAuth = IsolationInterop.GetIdentityAuthority();
          }
        }
        return IsolationInterop._idAuth;
      }
    }

    public static IAppIdAuthority AppIdAuthority
    {
      [SecuritySafeCritical] get
      {
        if (IsolationInterop._appIdAuth == null)
        {
          lock (IsolationInterop._synchObject)
          {
            if (IsolationInterop._appIdAuth == null)
              IsolationInterop._appIdAuth = IsolationInterop.GetAppIdAuthority();
          }
        }
        return IsolationInterop._appIdAuth;
      }
    }

    [SecuritySafeCritical]
    public static Store GetUserStore()
    {
      return new Store(IsolationInterop.GetUserStore(0U, IntPtr.Zero, ref IsolationInterop.IID_IStore) as IStore);
    }

    [SecuritySafeCritical]
    internal static IActContext CreateActContext(IDefinitionAppId AppId)
    {
      IsolationInterop.CreateActContextParameters Params;
      Params.Size = (uint) Marshal.SizeOf(typeof (IsolationInterop.CreateActContextParameters));
      Params.Flags = 16U;
      Params.CustomStoreList = IntPtr.Zero;
      Params.CultureFallbackList = IntPtr.Zero;
      Params.ProcessorArchitectureList = IntPtr.Zero;
      Params.Source = IntPtr.Zero;
      Params.ProcArch = (ushort) 0;
      IsolationInterop.CreateActContextParametersSource parametersSource;
      parametersSource.Size = (uint) Marshal.SizeOf(typeof (IsolationInterop.CreateActContextParametersSource));
      parametersSource.Flags = 0U;
      parametersSource.SourceType = 1U;
      parametersSource.Data = IntPtr.Zero;
      IsolationInterop.CreateActContextParametersSourceDefinitionAppid sourceDefinitionAppid;
      sourceDefinitionAppid.Size = (uint) Marshal.SizeOf(typeof (IsolationInterop.CreateActContextParametersSourceDefinitionAppid));
      sourceDefinitionAppid.Flags = 0U;
      sourceDefinitionAppid.AppId = AppId;
      try
      {
        parametersSource.Data = sourceDefinitionAppid.ToIntPtr();
        Params.Source = parametersSource.ToIntPtr();
        return IsolationInterop.CreateActContext(ref Params) as IActContext;
      }
      finally
      {
        if (parametersSource.Data != IntPtr.Zero)
        {
          IsolationInterop.CreateActContextParametersSourceDefinitionAppid.Destroy(parametersSource.Data);
          parametersSource.Data = IntPtr.Zero;
        }
        if (Params.Source != IntPtr.Zero)
        {
          IsolationInterop.CreateActContextParametersSource.Destroy(Params.Source);
          Params.Source = IntPtr.Zero;
        }
      }
    }

    internal static IActContext CreateActContext(IReferenceAppId AppId)
    {
      IsolationInterop.CreateActContextParameters Params;
      Params.Size = (uint) Marshal.SizeOf(typeof (IsolationInterop.CreateActContextParameters));
      Params.Flags = 16U;
      Params.CustomStoreList = IntPtr.Zero;
      Params.CultureFallbackList = IntPtr.Zero;
      Params.ProcessorArchitectureList = IntPtr.Zero;
      Params.Source = IntPtr.Zero;
      Params.ProcArch = (ushort) 0;
      IsolationInterop.CreateActContextParametersSource parametersSource;
      parametersSource.Size = (uint) Marshal.SizeOf(typeof (IsolationInterop.CreateActContextParametersSource));
      parametersSource.Flags = 0U;
      parametersSource.SourceType = 2U;
      parametersSource.Data = IntPtr.Zero;
      IsolationInterop.CreateActContextParametersSourceReferenceAppid sourceReferenceAppid;
      sourceReferenceAppid.Size = (uint) Marshal.SizeOf(typeof (IsolationInterop.CreateActContextParametersSourceReferenceAppid));
      sourceReferenceAppid.Flags = 0U;
      sourceReferenceAppid.AppId = AppId;
      try
      {
        parametersSource.Data = sourceReferenceAppid.ToIntPtr();
        Params.Source = parametersSource.ToIntPtr();
        return IsolationInterop.CreateActContext(ref Params) as IActContext;
      }
      finally
      {
        if (parametersSource.Data != IntPtr.Zero)
        {
          IsolationInterop.CreateActContextParametersSourceDefinitionAppid.Destroy(parametersSource.Data);
          parametersSource.Data = IntPtr.Zero;
        }
        if (Params.Source != IntPtr.Zero)
        {
          IsolationInterop.CreateActContextParametersSource.Destroy(Params.Source);
          Params.Source = IntPtr.Zero;
        }
      }
    }

    [DllImport("clr.dll", PreserveSig = false)]
    [return: MarshalAs(UnmanagedType.IUnknown)]
    internal static extern object CreateActContext(ref IsolationInterop.CreateActContextParameters Params);

    [SecurityCritical]
    [DllImport("clr.dll", PreserveSig = false)]
    [return: MarshalAs(UnmanagedType.IUnknown)]
    internal static extern object CreateCMSFromXml([In] byte[] buffer, [In] uint bufferSize, [In] IManifestParseErrorCallback Callback, [In] ref Guid riid);

    [SecurityCritical]
    [DllImport("clr.dll", PreserveSig = false)]
    [return: MarshalAs(UnmanagedType.IUnknown)]
    internal static extern object ParseManifest([MarshalAs(UnmanagedType.LPWStr), In] string pszManifestPath, [In] IManifestParseErrorCallback pIManifestParseErrorCallback, [In] ref Guid riid);

    [SecurityCritical]
    [DllImport("clr.dll", PreserveSig = false)]
    [return: MarshalAs(UnmanagedType.IUnknown)]
    private static extern object GetUserStore([In] uint Flags, [In] IntPtr hToken, [In] ref Guid riid);

    [SecurityCritical]
    [DllImport("clr.dll", PreserveSig = false)]
    [return: MarshalAs(UnmanagedType.IUnknown)]
    private static extern object GetSystemStore([In] uint Flags, [In] ref Guid riid);

    [SecurityCritical]
    [DllImport("clr.dll", PreserveSig = false)]
    [return: MarshalAs(UnmanagedType.Interface)]
    private static extern IIdentityAuthority GetIdentityAuthority();

    [SecurityCritical]
    [DllImport("clr.dll", PreserveSig = false)]
    [return: MarshalAs(UnmanagedType.Interface)]
    private static extern IAppIdAuthority GetAppIdAuthority();

    [DllImport("clr.dll", PreserveSig = false)]
    [return: MarshalAs(UnmanagedType.IUnknown)]
    internal static extern object GetUserStateManager([In] uint Flags, [In] IntPtr hToken, [In] ref Guid riid);

    internal static Guid GetGuidOfType(Type type)
    {
      return new Guid(((GuidAttribute) Attribute.GetCustomAttribute((MemberInfo) type, typeof (GuidAttribute), false)).Value);
    }

    internal struct CreateActContextParameters
    {
      [MarshalAs(UnmanagedType.U4)]
      public uint Size;
      [MarshalAs(UnmanagedType.U4)]
      public uint Flags;
      [MarshalAs(UnmanagedType.SysInt)]
      public IntPtr CustomStoreList;
      [MarshalAs(UnmanagedType.SysInt)]
      public IntPtr CultureFallbackList;
      [MarshalAs(UnmanagedType.SysInt)]
      public IntPtr ProcessorArchitectureList;
      [MarshalAs(UnmanagedType.SysInt)]
      public IntPtr Source;
      [MarshalAs(UnmanagedType.U2)]
      public ushort ProcArch;

      [Flags]
      public enum CreateFlags
      {
        Nothing = 0,
        StoreListValid = 1,
        CultureListValid = 2,
        ProcessorFallbackListValid = 4,
        ProcessorValid = 8,
        SourceValid = 16,
        IgnoreVisibility = 32,
      }
    }

    internal struct CreateActContextParametersSource
    {
      [MarshalAs(UnmanagedType.U4)]
      public uint Size;
      [MarshalAs(UnmanagedType.U4)]
      public uint Flags;
      [MarshalAs(UnmanagedType.U4)]
      public uint SourceType;
      [MarshalAs(UnmanagedType.SysInt)]
      public IntPtr Data;

      [SecurityCritical]
      public IntPtr ToIntPtr()
      {
        IntPtr ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf((object) this));
        Marshal.StructureToPtr((object) this, ptr, false);
        return ptr;
      }

      [SecurityCritical]
      public static void Destroy(IntPtr p)
      {
        Marshal.DestroyStructure(p, typeof (IsolationInterop.CreateActContextParametersSource));
        Marshal.FreeCoTaskMem(p);
      }

      [Flags]
      public enum SourceFlags
      {
        Definition = 1,
        Reference = 2,
      }
    }

    internal struct CreateActContextParametersSourceReferenceAppid
    {
      [MarshalAs(UnmanagedType.U4)]
      public uint Size;
      [MarshalAs(UnmanagedType.U4)]
      public uint Flags;
      public IReferenceAppId AppId;

      public IntPtr ToIntPtr()
      {
        IntPtr ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf((object) this));
        Marshal.StructureToPtr((object) this, ptr, false);
        return ptr;
      }

      public static void Destroy(IntPtr p)
      {
        Marshal.DestroyStructure(p, typeof (IsolationInterop.CreateActContextParametersSourceReferenceAppid));
        Marshal.FreeCoTaskMem(p);
      }
    }

    internal struct CreateActContextParametersSourceDefinitionAppid
    {
      [MarshalAs(UnmanagedType.U4)]
      public uint Size;
      [MarshalAs(UnmanagedType.U4)]
      public uint Flags;
      public IDefinitionAppId AppId;

      [SecurityCritical]
      public IntPtr ToIntPtr()
      {
        IntPtr ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf((object) this));
        Marshal.StructureToPtr((object) this, ptr, false);
        return ptr;
      }

      [SecurityCritical]
      public static void Destroy(IntPtr p)
      {
        Marshal.DestroyStructure(p, typeof (IsolationInterop.CreateActContextParametersSourceDefinitionAppid));
        Marshal.FreeCoTaskMem(p);
      }
    }
  }
}
