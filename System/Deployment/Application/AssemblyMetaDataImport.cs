// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.AssemblyMetaDataImport
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Deployment.Application
{
  internal class AssemblyMetaDataImport : DisposableBase
  {
    private static Guid _importerGuid = new Guid(((GuidAttribute) Attribute.GetCustomAttribute((MemberInfo) typeof (IMetaDataImport), typeof (GuidAttribute), false)).Value);
    private AssemblyModule[] _modules;
    private AssemblyName _name;
    private AssemblyReference[] _asmRefs;
    private IMetaDataDispenser _metaDispenser;
    private IMetaDataAssemblyImport _assemblyImport;
    private const int GENMAN_STRING_BUF_SIZE = 1024;
    private const int GENMAN_LOCALE_BUF_SIZE = 64;
    private const int GENMAN_ENUM_TOKEN_BUF_SIZE = 16;

    public AssemblyModule[] Files
    {
      get
      {
        if (this._modules == null)
        {
          lock (this)
          {
            if (this._modules == null)
              this._modules = this.ImportAssemblyFiles();
          }
        }
        return this._modules;
      }
    }

    public AssemblyName Name
    {
      get
      {
        if (this._name == null)
        {
          lock (this)
          {
            if (this._name == null)
              this._name = this.ImportIdentity();
          }
        }
        return this._name;
      }
    }

    public AssemblyReference[] References
    {
      get
      {
        if (this._asmRefs == null)
        {
          lock (this)
          {
            if (this._asmRefs == null)
              this._asmRefs = this.ImportAssemblyReferences();
          }
        }
        return this._asmRefs;
      }
    }

    public AssemblyMetaDataImport(string sourceFile)
    {
      this._metaDispenser = (IMetaDataDispenser) new CorMetaDataDispenser();
      this._assemblyImport = (IMetaDataAssemblyImport) this._metaDispenser.OpenScope(sourceFile, 0U, ref AssemblyMetaDataImport._importerGuid);
    }

    protected override void DisposeUnmanagedResources()
    {
      if (this._assemblyImport != null)
        Marshal.ReleaseComObject((object) this._assemblyImport);
      if (this._metaDispenser == null)
        return;
      Marshal.ReleaseComObject((object) this._metaDispenser);
    }

    private AssemblyModule[] ImportAssemblyFiles()
    {
      ArrayList arrayList = new ArrayList();
      IntPtr phEnum = IntPtr.Zero;
      uint[] fileRefs = new uint[16];
      char[] strName = new char[1024];
      try
      {
        uint iFetched;
        do
        {
          this._assemblyImport.EnumFiles(out phEnum, fileRefs, (uint) fileRefs.Length, out iFetched);
          for (uint index = 0; index < iFetched; ++index)
          {
            uint cchNameRequired;
            IntPtr bHashData;
            uint cchHashBytes;
            uint dwFileFlags;
            this._assemblyImport.GetFileProps(fileRefs[(int) index], strName, (uint) strName.Length, out cchNameRequired, out bHashData, out cchHashBytes, out dwFileFlags);
            byte[] numArray = new byte[(int) cchHashBytes];
            Marshal.Copy(bHashData, numArray, 0, (int) cchHashBytes);
            arrayList.Add((object) new AssemblyModule(new string(strName, 0, (int) cchNameRequired - 1), numArray));
          }
        }
        while (iFetched > 0U);
      }
      finally
      {
        if (phEnum != IntPtr.Zero)
          this._assemblyImport.CloseEnum(phEnum);
      }
      return (AssemblyModule[]) arrayList.ToArray(typeof (AssemblyModule));
    }

    private AssemblyName ImportIdentity()
    {
      uint mdAsm;
      this._assemblyImport.GetAssemblyFromScope(out mdAsm);
      IntPtr pPublicKeyPtr;
      uint ucbPublicKeyPtr;
      uint uHashAlg;
      uint cchNameRequired;
      uint dwFlags;
      this._assemblyImport.GetAssemblyProps(mdAsm, out pPublicKeyPtr, out ucbPublicKeyPtr, out uHashAlg, (char[]) null, 0U, out cchNameRequired, IntPtr.Zero, out dwFlags);
      char[] chArray = new char[(int) cchNameRequired + 1];
      IntPtr num = IntPtr.Zero;
      try
      {
        num = this.AllocAsmMeta();
        this._assemblyImport.GetAssemblyProps(mdAsm, out pPublicKeyPtr, out ucbPublicKeyPtr, out uHashAlg, chArray, (uint) chArray.Length, out cchNameRequired, num, out dwFlags);
        return this.ConstructAssemblyName(num, chArray, cchNameRequired, pPublicKeyPtr, ucbPublicKeyPtr, dwFlags);
      }
      finally
      {
        this.FreeAsmMeta(num);
      }
    }

    private AssemblyReference[] ImportAssemblyReferences()
    {
      ArrayList arrayList = new ArrayList();
      IntPtr phEnum = IntPtr.Zero;
      uint[] asmRefs = new uint[16];
      try
      {
        uint iFetched;
        do
        {
          this._assemblyImport.EnumAssemblyRefs(out phEnum, asmRefs, (uint) asmRefs.Length, out iFetched);
          for (uint index = 0; index < iFetched; ++index)
          {
            IntPtr ppbPublicKeyOrToken;
            uint pcbPublicKeyOrToken;
            uint pchNameOut;
            IntPtr ppbHashValue;
            uint pcbHashValue;
            uint pdwAssemblyRefFlags;
            this._assemblyImport.GetAssemblyRefProps(asmRefs[(int) index], out ppbPublicKeyOrToken, out pcbPublicKeyOrToken, (char[]) null, 0U, out pchNameOut, IntPtr.Zero, out ppbHashValue, out pcbHashValue, out pdwAssemblyRefFlags);
            char[] chArray = new char[(int) pchNameOut + 1];
            IntPtr num = IntPtr.Zero;
            try
            {
              num = this.AllocAsmMeta();
              this._assemblyImport.GetAssemblyRefProps(asmRefs[(int) index], out ppbPublicKeyOrToken, out pcbPublicKeyOrToken, chArray, (uint) chArray.Length, out pchNameOut, num, out ppbHashValue, out pcbHashValue, out pdwAssemblyRefFlags);
              AssemblyName name = this.ConstructAssemblyName(num, chArray, pchNameOut, ppbPublicKeyOrToken, pcbPublicKeyOrToken, pdwAssemblyRefFlags);
              arrayList.Add((object) new AssemblyReference(name));
            }
            finally
            {
              this.FreeAsmMeta(num);
            }
          }
        }
        while (iFetched > 0U);
      }
      finally
      {
        if (phEnum != IntPtr.Zero)
          this._assemblyImport.CloseEnum(phEnum);
      }
      return (AssemblyReference[]) arrayList.ToArray(typeof (AssemblyReference));
    }

    private IntPtr AllocAsmMeta()
    {
      ASSEMBLYMETADATA assemblymetadata;
      assemblymetadata.usMajorVersion = assemblymetadata.usMinorVersion = assemblymetadata.usBuildNumber = assemblymetadata.usRevisionNumber = (ushort) 0;
      assemblymetadata.cOses = assemblymetadata.cProcessors = 0U;
      assemblymetadata.rOses = assemblymetadata.rpProcessors = IntPtr.Zero;
      assemblymetadata.rpLocale = Marshal.AllocCoTaskMem(128);
      assemblymetadata.cchLocale = 64U;
      IntPtr ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof (ASSEMBLYMETADATA)));
      Marshal.StructureToPtr((object) assemblymetadata, ptr, false);
      return ptr;
    }

    private AssemblyName ConstructAssemblyName(IntPtr asmMetaPtr, char[] asmNameBuf, uint asmNameLength, IntPtr pubKeyPtr, uint pubKeyBytes, uint flags)
    {
      ASSEMBLYMETADATA structure = (ASSEMBLYMETADATA) Marshal.PtrToStructure(asmMetaPtr, typeof (ASSEMBLYMETADATA));
      AssemblyName assemblyName = new AssemblyName();
      assemblyName.Name = new string(asmNameBuf, 0, (int) asmNameLength - 1);
      assemblyName.Version = new Version((int) structure.usMajorVersion, (int) structure.usMinorVersion, (int) structure.usBuildNumber, (int) structure.usRevisionNumber);
      string stringUni = Marshal.PtrToStringUni(structure.rpLocale);
      assemblyName.CultureInfo = new CultureInfo(stringUni);
      if (pubKeyBytes > 0U)
      {
        byte[] numArray = new byte[(int) pubKeyBytes];
        Marshal.Copy(pubKeyPtr, numArray, 0, (int) pubKeyBytes);
        if (((int) flags & 1) != 0)
          assemblyName.SetPublicKey(numArray);
        else
          assemblyName.SetPublicKeyToken(numArray);
      }
      return assemblyName;
    }

    private void FreeAsmMeta(IntPtr asmMetaPtr)
    {
      if (!(asmMetaPtr != IntPtr.Zero))
        return;
      Marshal.FreeCoTaskMem(((ASSEMBLYMETADATA) Marshal.PtrToStructure(asmMetaPtr, typeof (ASSEMBLYMETADATA))).rpLocale);
      Marshal.DestroyStructure(asmMetaPtr, typeof (ASSEMBLYMETADATA));
      Marshal.FreeCoTaskMem(asmMetaPtr);
    }
  }
}
