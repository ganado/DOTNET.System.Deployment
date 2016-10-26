// Decompiled with JetBrains decompiler
// Type: Microsoft.Runtime.Hosting.IClrStrongNameUsingIntPtr
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Runtime.Hosting
{
  [SecurityCritical]
  [ComConversionLoss]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("9FD93CCF-3280-4391-B3A9-96E1CDE77C8D")]
  [ComImport]
  internal interface IClrStrongNameUsingIntPtr
  {
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int GetHashFromAssemblyFile([MarshalAs(UnmanagedType.LPStr), In] string pszFilePath, [MarshalAs(UnmanagedType.U4), In, Out] ref int piHashAlg, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3), Out] byte[] pbHash, [MarshalAs(UnmanagedType.U4), In] int cchHash, [MarshalAs(UnmanagedType.U4)] out int pchHash);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int GetHashFromAssemblyFileW([MarshalAs(UnmanagedType.LPWStr), In] string pwzFilePath, [MarshalAs(UnmanagedType.U4), In, Out] ref int piHashAlg, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3), Out] byte[] pbHash, [MarshalAs(UnmanagedType.U4), In] int cchHash, [MarshalAs(UnmanagedType.U4)] out int pchHash);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int GetHashFromBlob([In] IntPtr pbBlob, [MarshalAs(UnmanagedType.U4), In] int cchBlob, [MarshalAs(UnmanagedType.U4), In, Out] ref int piHashAlg, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4), Out] byte[] pbHash, [MarshalAs(UnmanagedType.U4), In] int cchHash, [MarshalAs(UnmanagedType.U4)] out int pchHash);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int GetHashFromFile([MarshalAs(UnmanagedType.LPStr), In] string pszFilePath, [MarshalAs(UnmanagedType.U4), In, Out] ref int piHashAlg, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3), Out] byte[] pbHash, [MarshalAs(UnmanagedType.U4), In] int cchHash, [MarshalAs(UnmanagedType.U4)] out int pchHash);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int GetHashFromFileW([MarshalAs(UnmanagedType.LPWStr), In] string pwzFilePath, [MarshalAs(UnmanagedType.U4), In, Out] ref int piHashAlg, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3), Out] byte[] pbHash, [MarshalAs(UnmanagedType.U4), In] int cchHash, [MarshalAs(UnmanagedType.U4)] out int pchHash);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int GetHashFromHandle([In] IntPtr hFile, [MarshalAs(UnmanagedType.U4), In, Out] ref int piHashAlg, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3), Out] byte[] pbHash, [MarshalAs(UnmanagedType.U4), In] int cchHash, [MarshalAs(UnmanagedType.U4)] out int pchHash);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.U4)]
    int StrongNameCompareAssemblies([MarshalAs(UnmanagedType.LPWStr), In] string pwzAssembly1, [MarshalAs(UnmanagedType.LPWStr), In] string pwzAssembly2, [MarshalAs(UnmanagedType.U4)] out int dwResult);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int StrongNameFreeBuffer([In] IntPtr pbMemory);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int StrongNameGetBlob([MarshalAs(UnmanagedType.LPWStr), In] string pwzFilePath, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2), Out] byte[] pbBlob, [MarshalAs(UnmanagedType.U4), In, Out] ref int pcbBlob);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int StrongNameGetBlobFromImage([In] IntPtr pbBase, [MarshalAs(UnmanagedType.U4), In] int dwLength, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3), Out] byte[] pbBlob, [MarshalAs(UnmanagedType.U4), In, Out] ref int pcbBlob);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int StrongNameGetPublicKey([MarshalAs(UnmanagedType.LPWStr), In] string pwzKeyContainer, [In] IntPtr pbKeyBlob, [MarshalAs(UnmanagedType.U4), In] int cbKeyBlob, out IntPtr ppbPublicKeyBlob, [MarshalAs(UnmanagedType.U4)] out int pcbPublicKeyBlob);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.U4)]
    int StrongNameHashSize([MarshalAs(UnmanagedType.U4), In] int ulHashAlg, [MarshalAs(UnmanagedType.U4)] out int cbSize);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int StrongNameKeyDelete([MarshalAs(UnmanagedType.LPWStr), In] string pwzKeyContainer);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int StrongNameKeyGen([MarshalAs(UnmanagedType.LPWStr), In] string pwzKeyContainer, [MarshalAs(UnmanagedType.U4), In] int dwFlags, out IntPtr ppbKeyBlob, [MarshalAs(UnmanagedType.U4)] out int pcbKeyBlob);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int StrongNameKeyGenEx([MarshalAs(UnmanagedType.LPWStr), In] string pwzKeyContainer, [MarshalAs(UnmanagedType.U4), In] int dwFlags, [MarshalAs(UnmanagedType.U4), In] int dwKeySize, out IntPtr ppbKeyBlob, [MarshalAs(UnmanagedType.U4)] out int pcbKeyBlob);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int StrongNameKeyInstall([MarshalAs(UnmanagedType.LPWStr), In] string pwzKeyContainer, [In] IntPtr pbKeyBlob, [MarshalAs(UnmanagedType.U4), In] int cbKeyBlob);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int StrongNameSignatureGeneration([MarshalAs(UnmanagedType.LPWStr), In] string pwzFilePath, [MarshalAs(UnmanagedType.LPWStr), In] string pwzKeyContainer, [In] IntPtr pbKeyBlob, [MarshalAs(UnmanagedType.U4), In] int cbKeyBlob, [In, Out] IntPtr ppbSignatureBlob, [MarshalAs(UnmanagedType.U4)] out int pcbSignatureBlob);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int StrongNameSignatureGenerationEx([MarshalAs(UnmanagedType.LPWStr), In] string wszFilePath, [MarshalAs(UnmanagedType.LPWStr), In] string wszKeyContainer, [In] IntPtr pbKeyBlob, [MarshalAs(UnmanagedType.U4), In] int cbKeyBlob, [In, Out] IntPtr ppbSignatureBlob, [MarshalAs(UnmanagedType.U4)] out int pcbSignatureBlob, [MarshalAs(UnmanagedType.U4), In] int dwFlags);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int StrongNameSignatureSize([In] IntPtr pbPublicKeyBlob, [MarshalAs(UnmanagedType.U4), In] int cbPublicKeyBlob, [MarshalAs(UnmanagedType.U4)] out int pcbSize);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.U4)]
    int StrongNameSignatureVerification([MarshalAs(UnmanagedType.LPWStr), In] string pwzFilePath, [MarshalAs(UnmanagedType.U4), In] int dwInFlags, [MarshalAs(UnmanagedType.U4)] out int dwOutFlags);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.U4)]
    int StrongNameSignatureVerificationEx([MarshalAs(UnmanagedType.LPWStr), In] string pwzFilePath, [MarshalAs(UnmanagedType.I1), In] bool fForceVerification, [MarshalAs(UnmanagedType.I1)] out bool fWasVerified);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.U4)]
    int StrongNameSignatureVerificationFromImage([In] IntPtr pbBase, [MarshalAs(UnmanagedType.U4), In] int dwLength, [MarshalAs(UnmanagedType.U4), In] int dwInFlags, [MarshalAs(UnmanagedType.U4)] out int dwOutFlags);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int StrongNameTokenFromAssembly([MarshalAs(UnmanagedType.LPWStr), In] string pwzFilePath, out IntPtr ppbStrongNameToken, [MarshalAs(UnmanagedType.U4)] out int pcbStrongNameToken);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int StrongNameTokenFromAssemblyEx([MarshalAs(UnmanagedType.LPWStr), In] string pwzFilePath, out IntPtr ppbStrongNameToken, [MarshalAs(UnmanagedType.U4)] out int pcbStrongNameToken, out IntPtr ppbPublicKeyBlob, [MarshalAs(UnmanagedType.U4)] out int pcbPublicKeyBlob);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int StrongNameTokenFromPublicKey([In] IntPtr pbPublicKeyBlob, [MarshalAs(UnmanagedType.U4), In] int cbPublicKeyBlob, out IntPtr ppbStrongNameToken, [MarshalAs(UnmanagedType.U4)] out int pcbStrongNameToken);
  }
}
