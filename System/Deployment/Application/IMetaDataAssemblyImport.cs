// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.IMetaDataAssemblyImport
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Deployment.Application
{
  [Guid("EE62470B-E94B-424e-9B7C-2F00C9249F93")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  internal interface IMetaDataAssemblyImport
  {
    void GetAssemblyProps(uint mdAsm, out IntPtr pPublicKeyPtr, out uint ucbPublicKeyPtr, out uint uHashAlg, [MarshalAs(UnmanagedType.LPArray)] char[] strName, uint cchNameIn, out uint cchNameRequired, IntPtr amdInfo, out uint dwFlags);

    void GetAssemblyRefProps(uint mdAsmRef, out IntPtr ppbPublicKeyOrToken, out uint pcbPublicKeyOrToken, [MarshalAs(UnmanagedType.LPArray)] char[] strName, uint cchNameIn, out uint pchNameOut, IntPtr amdInfo, out IntPtr ppbHashValue, out uint pcbHashValue, out uint pdwAssemblyRefFlags);

    void GetFileProps([In] uint mdFile, [MarshalAs(UnmanagedType.LPArray)] char[] strName, uint cchName, out uint cchNameRequired, out IntPtr bHashData, out uint cchHashBytes, out uint dwFileFlags);

    void GetExportedTypeProps();

    void GetManifestResourceProps();

    void EnumAssemblyRefs([In, Out] ref IntPtr phEnum, [MarshalAs(UnmanagedType.LPArray), Out] uint[] asmRefs, uint asmRefCount, out uint iFetched);

    void EnumFiles([In, Out] ref IntPtr phEnum, [MarshalAs(UnmanagedType.LPArray), Out] uint[] fileRefs, uint fileRefCount, out uint iFetched);

    void EnumExportedTypes();

    void EnumManifestResources();

    void GetAssemblyFromScope(out uint mdAsm);

    void FindExportedTypeByName();

    void FindManifestResourceByName();

    [MethodImpl(MethodImplOptions.PreserveSig)]
    void CloseEnum([In] IntPtr phEnum);

    void FindAssembliesByName();
  }
}
