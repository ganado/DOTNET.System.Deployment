// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.Manifest.AssemblyManifest
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Collections;
using System.ComponentModel;
using System.Deployment.Application.Win32InterOp;
using System.Deployment.Internal.CodeSigning;
using System.Deployment.Internal.Isolation;
using System.Deployment.Internal.Isolation.Manifest;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Xml;

namespace System.Deployment.Application.Manifest
{
  internal class AssemblyManifest
  {
    private static char[] SpecificInvalidIdentityChars = new char[2]{ '#', '&' };
    private ManifestSourceFormat _manifestSourceFormat = ManifestSourceFormat.Unknown;
    private string _rawXmlFilePath;
    private byte[] _rawXmlBytes;
    private ICMS _cms;
    private object _identity;
    private object _description;
    private object _entryPoints;
    private object _dependentAssemblies;
    private object _files;
    private object _fileAssociations;
    private object _deployment;
    private object _dependentOS;
    private object _manifestFlags;
    private object _requestedExecutionLevel;
    private object _requestedExecutionLevelUIAccess;
    private object _compatibleFrameworks;
    private System.Deployment.Application.DefinitionIdentity _id1Identity;
    private System.Deployment.Application.DefinitionIdentity _complibIdentity;
    private bool _id1ManifestPresent;
    private string _id1RequestedExecutionLevel;
    private ulong _sizeInBytes;
    private bool _unhashedFilePresent;
    private bool _unhashedDependencyPresent;
    private bool _signed;
    private bool _clrDependentAssemblyChecked;
    private DependentAssembly _clrDependentAssembly;

    public string RawXmlFilePath
    {
      get
      {
        return this._rawXmlFilePath;
      }
    }

    public byte[] RawXmlBytes
    {
      get
      {
        return this._rawXmlBytes;
      }
    }

    public System.Deployment.Application.DefinitionIdentity Identity
    {
      get
      {
        if (this._identity == null && this._cms != null)
          Interlocked.CompareExchange(ref this._identity, this._cms.Identity != null ? (object) new System.Deployment.Application.DefinitionIdentity(this._cms.Identity) : (object) new System.Deployment.Application.DefinitionIdentity(), (object) null);
        return (System.Deployment.Application.DefinitionIdentity) this._identity;
      }
    }

    public ulong SizeInBytes
    {
      get
      {
        return this._sizeInBytes;
      }
    }

    public System.Deployment.Application.DefinitionIdentity Id1Identity
    {
      get
      {
        return this._id1Identity;
      }
    }

    public System.Deployment.Application.DefinitionIdentity ComplibIdentity
    {
      get
      {
        return this._complibIdentity;
      }
    }

    public bool Id1ManifestPresent
    {
      get
      {
        return this._id1ManifestPresent;
      }
    }

    public string Id1RequestedExecutionLevel
    {
      get
      {
        return this._id1RequestedExecutionLevel;
      }
    }

    public uint ManifestFlags
    {
      get
      {
        if (this._manifestFlags == null && this._cms != null)
          Interlocked.CompareExchange(ref this._manifestFlags, (object) ((IMetadataSectionEntry) this._cms.MetadataSectionEntry).ManifestFlags, (object) null);
        return (uint) this._manifestFlags;
      }
    }

    public string RequestedExecutionLevel
    {
      get
      {
        if (this._requestedExecutionLevel == null && this._cms != null)
          Interlocked.CompareExchange(ref this._requestedExecutionLevel, (object) ((IMetadataSectionEntry) this._cms.MetadataSectionEntry).RequestedExecutionLevel, (object) null);
        return (string) this._requestedExecutionLevel;
      }
    }

    public bool RequestedExecutionLevelUIAccess
    {
      get
      {
        if (this._requestedExecutionLevelUIAccess == null && this._cms != null)
          Interlocked.CompareExchange(ref this._requestedExecutionLevelUIAccess, (object) ((IMetadataSectionEntry) this._cms.MetadataSectionEntry).RequestedExecutionLevelUIAccess, (object) null);
        return (bool) this._requestedExecutionLevelUIAccess;
      }
    }

    public bool Application
    {
      get
      {
        return (this.ManifestFlags & 4U) > 0U;
      }
    }

    public bool UseManifestForTrust
    {
      get
      {
        return (this.ManifestFlags & 8U) > 0U;
      }
    }

    public Description Description
    {
      get
      {
        if (this._description == null && this._cms != null)
        {
          IDescriptionMetadataEntry descriptionData = ((IMetadataSectionEntry) this._cms.MetadataSectionEntry).DescriptionData;
          if (descriptionData != null)
            Interlocked.CompareExchange(ref this._description, (object) new Description(descriptionData.AllData), (object) null);
        }
        return (Description) this._description;
      }
    }

    public System.Deployment.Application.Manifest.Deployment Deployment
    {
      get
      {
        if (this._deployment == null && this._cms != null)
        {
          IDeploymentMetadataEntry deploymentData = ((IMetadataSectionEntry) this._cms.MetadataSectionEntry).DeploymentData;
          if (deploymentData != null)
            Interlocked.CompareExchange(ref this._deployment, (object) new System.Deployment.Application.Manifest.Deployment(deploymentData.AllData), (object) null);
        }
        return (System.Deployment.Application.Manifest.Deployment) this._deployment;
      }
    }

    public DependentOS DependentOS
    {
      get
      {
        if (this._dependentOS == null && this._cms != null)
        {
          IDependentOSMetadataEntry dependentOsData = ((IMetadataSectionEntry) this._cms.MetadataSectionEntry).DependentOSData;
          if (dependentOsData != null)
            Interlocked.CompareExchange(ref this._dependentOS, (object) new DependentOS(dependentOsData.AllData), (object) null);
        }
        return (DependentOS) this._dependentOS;
      }
    }

    public DependentAssembly[] DependentAssemblies
    {
      get
      {
        if (this._dependentAssemblies == null)
        {
          ISection section = this._cms != null ? this._cms.AssemblyReferenceSection : (ISection) null;
          uint celt = section != null ? section.Count : 0U;
          DependentAssembly[] dependentAssemblyArray = new DependentAssembly[(int) celt];
          if (celt > 0U)
          {
            uint celtFetched = 0;
            IAssemblyReferenceEntry[] assemblyReferenceEntryArray = new IAssemblyReferenceEntry[(int) celt];
            Marshal.ThrowExceptionForHR(((System.Deployment.Internal.Isolation.IEnumUnknown) section._NewEnum).Next(celt, (object[]) assemblyReferenceEntryArray, ref celtFetched));
            if ((int) celtFetched != (int) celt)
              throw new InvalidDeploymentException(ExceptionTypes.Manifest, Resources.GetString("Ex_IsoEnumFetchNotEqualToCount"));
            for (uint index = 0; index < celt; ++index)
              dependentAssemblyArray[(int) index] = new DependentAssembly(assemblyReferenceEntryArray[(int) index].AllData);
          }
          Interlocked.CompareExchange(ref this._dependentAssemblies, (object) dependentAssemblyArray, (object) null);
        }
        return (DependentAssembly[]) this._dependentAssemblies;
      }
    }

    public FileAssociation[] FileAssociations
    {
      get
      {
        if (this._fileAssociations == null)
        {
          ISection section = this._cms != null ? this._cms.FileAssociationSection : (ISection) null;
          uint celt = section != null ? section.Count : 0U;
          FileAssociation[] fileAssociationArray = new FileAssociation[(int) celt];
          if (celt > 0U)
          {
            uint celtFetched = 0;
            IFileAssociationEntry[] associationEntryArray = new IFileAssociationEntry[(int) celt];
            Marshal.ThrowExceptionForHR(((System.Deployment.Internal.Isolation.IEnumUnknown) section._NewEnum).Next(celt, (object[]) associationEntryArray, ref celtFetched));
            if ((int) celtFetched != (int) celt)
              throw new InvalidDeploymentException(ExceptionTypes.Manifest, Resources.GetString("Ex_IsoEnumFetchNotEqualToCount"));
            for (uint index = 0; index < celt; ++index)
              fileAssociationArray[(int) index] = new FileAssociation(associationEntryArray[(int) index].AllData);
          }
          Interlocked.CompareExchange(ref this._fileAssociations, (object) fileAssociationArray, (object) null);
        }
        return (FileAssociation[]) this._fileAssociations;
      }
    }

    public File[] Files
    {
      get
      {
        if (this._files == null)
        {
          ISection section = this._cms != null ? this._cms.FileSection : (ISection) null;
          uint celt = section != null ? section.Count : 0U;
          File[] fileArray = new File[(int) celt];
          if (celt > 0U)
          {
            uint celtFetched = 0;
            IFileEntry[] fileEntryArray = new IFileEntry[(int) celt];
            Marshal.ThrowExceptionForHR(((System.Deployment.Internal.Isolation.IEnumUnknown) section._NewEnum).Next(celt, (object[]) fileEntryArray, ref celtFetched));
            if ((int) celtFetched != (int) celt)
              throw new InvalidDeploymentException(ExceptionTypes.Manifest, Resources.GetString("Ex_IsoEnumFetchNotEqualToCount"));
            for (uint index = 0; index < celt; ++index)
              fileArray[(int) index] = new File(fileEntryArray[(int) index].AllData);
          }
          Interlocked.CompareExchange(ref this._files, (object) fileArray, (object) null);
        }
        return (File[]) this._files;
      }
    }

    public CompatibleFrameworks CompatibleFrameworks
    {
      get
      {
        if (this._compatibleFrameworks == null && this._cms != null)
        {
          ICompatibleFrameworksMetadataEntry compatibleFrameworksData = ((IMetadataSectionEntry) this._cms.MetadataSectionEntry).CompatibleFrameworksData;
          if (compatibleFrameworksData != null)
          {
            ISection section = this._cms != null ? this._cms.CompatibleFrameworksSection : (ISection) null;
            uint celt = section != null ? section.Count : 0U;
            CompatibleFramework[] frameworks = new CompatibleFramework[(int) celt];
            if (celt > 0U)
            {
              uint celtFetched = 0;
              ICompatibleFrameworkEntry[] compatibleFrameworkEntryArray = new ICompatibleFrameworkEntry[(int) celt];
              Marshal.ThrowExceptionForHR(((System.Deployment.Internal.Isolation.IEnumUnknown) section._NewEnum).Next(celt, (object[]) compatibleFrameworkEntryArray, ref celtFetched));
              if ((int) celtFetched != (int) celt)
                throw new InvalidDeploymentException(ExceptionTypes.Manifest, Resources.GetString("Ex_IsoEnumFetchNotEqualToCount"));
              for (uint index = 0; index < celt; ++index)
                frameworks[(int) index] = new CompatibleFramework(compatibleFrameworkEntryArray[(int) index].AllData);
            }
            Interlocked.CompareExchange(ref this._compatibleFrameworks, (object) new CompatibleFrameworks(compatibleFrameworksData.AllData, frameworks), (object) null);
          }
        }
        return (CompatibleFrameworks) this._compatibleFrameworks;
      }
    }

    public EntryPoint[] EntryPoints
    {
      get
      {
        if (this._entryPoints == null)
        {
          ISection section = this._cms != null ? this._cms.EntryPointSection : (ISection) null;
          uint celt = section != null ? section.Count : 0U;
          EntryPoint[] entryPointArray = new EntryPoint[(int) celt];
          if (celt > 0U)
          {
            uint celtFetched = 0;
            IEntryPointEntry[] entryPointEntryArray = new IEntryPointEntry[(int) celt];
            Marshal.ThrowExceptionForHR(((System.Deployment.Internal.Isolation.IEnumUnknown) section._NewEnum).Next(celt, (object[]) entryPointEntryArray, ref celtFetched));
            if ((int) celtFetched != (int) celt)
              throw new InvalidDeploymentException(ExceptionTypes.Manifest, Resources.GetString("Ex_IsoEnumFetchNotEqualToCount"));
            for (uint index = 0; index < celt; ++index)
              entryPointArray[(int) index] = new EntryPoint(entryPointEntryArray[(int) index].AllData, this);
          }
          Interlocked.CompareExchange(ref this._entryPoints, (object) entryPointArray, (object) null);
        }
        return (EntryPoint[]) this._entryPoints;
      }
    }

    public DependentAssembly MainDependentAssembly
    {
      get
      {
        return this.DependentAssemblies[0];
      }
    }

    public DependentAssembly CLRDependentAssembly
    {
      get
      {
        if (!this._clrDependentAssemblyChecked)
        {
          foreach (DependentAssembly dependentAssembly in this.DependentAssemblies)
          {
            if (dependentAssembly.IsPreRequisite && PlatformDetector.IsCLRDependencyText(dependentAssembly.Identity.Name))
              this._clrDependentAssembly = dependentAssembly;
          }
          this._clrDependentAssemblyChecked = true;
        }
        return this._clrDependentAssembly;
      }
    }

    public bool RequiredHashMissing
    {
      get
      {
        if (!this._unhashedDependencyPresent)
          return this._unhashedFilePresent;
        return true;
      }
    }

    public bool Signed
    {
      get
      {
        return this._signed;
      }
    }

    public ManifestSourceFormat ManifestSourceFormat
    {
      get
      {
        return this._manifestSourceFormat;
      }
    }

    public AssemblyManifest(FileStream fileStream)
    {
      this.LoadCMSFromStream((Stream) fileStream);
      this._rawXmlFilePath = fileStream.Name;
      this._manifestSourceFormat = ManifestSourceFormat.XmlFile;
      this._sizeInBytes = (ulong) fileStream.Length;
    }

    public AssemblyManifest(Stream stream)
    {
      this.LoadCMSFromStream(stream);
      this._manifestSourceFormat = ManifestSourceFormat.Stream;
      this._sizeInBytes = (ulong) stream.Length;
    }

    public AssemblyManifest(string filePath)
    {
      string extension = Path.GetExtension(filePath);
      StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase;
      if (extension.Equals(".application", comparisonType) || extension.Equals(".manifest", comparisonType))
        this.LoadFromRawXmlFile(filePath);
      else if (extension.Equals(".dll", comparisonType) || extension.Equals(".exe", comparisonType))
        this.LoadFromInternalManifestFile(filePath);
      else
        this.LoadFromUnknownFormatFile(filePath);
    }

    public AssemblyManifest(ICMS cms)
    {
      if (cms == null)
        throw new ArgumentNullException("cms");
      this._cms = cms;
    }

    public void ValidateSemantics(AssemblyManifest.ManifestType manifestType)
    {
      if (manifestType != AssemblyManifest.ManifestType.Application)
      {
        if (manifestType != AssemblyManifest.ManifestType.Deployment)
          return;
        this.ValidateSemanticsForDeploymentRole();
      }
      else
        this.ValidateSemanticsForApplicationRole();
    }

    public File[] GetFilesInGroup(string group, bool optionalOnly)
    {
      StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase;
      ArrayList arrayList = new ArrayList();
      foreach (File file in this.Files)
      {
        if (group == null && !file.IsOptional || group != null && group.Equals(file.Group, comparisonType) && (file.IsOptional || !optionalOnly))
          arrayList.Add((object) file);
      }
      return (File[]) arrayList.ToArray(typeof (File));
    }

    private static bool IsResourceReference(DependentAssembly dependentAssembly)
    {
      return dependentAssembly.ResourceFallbackCulture != null && dependentAssembly.Identity != null && dependentAssembly.Identity.Culture == null;
    }

    public DependentAssembly[] GetPrivateAssembliesInGroup(string group, bool optionalOnly)
    {
      StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase;
      Hashtable hashtable = new Hashtable();
      foreach (DependentAssembly dependentAssembly1 in this.DependentAssemblies)
      {
        if (!dependentAssembly1.IsPreRequisite && (group == null && !dependentAssembly1.IsOptional || group != null && group.Equals(dependentAssembly1.Group, comparisonType) && (dependentAssembly1.IsOptional || !optionalOnly)))
        {
          if (AssemblyManifest.IsResourceReference(dependentAssembly1))
            throw new InvalidDeploymentException(ExceptionTypes.ManifestSemanticValidation, Resources.GetString("Ex_SatelliteResourcesNotSupported"));
          DependentAssembly dependentAssembly2 = dependentAssembly1;
          if (dependentAssembly2 != null && !hashtable.Contains((object) dependentAssembly2.Identity))
            hashtable.Add((object) dependentAssembly2.Identity, (object) dependentAssembly2);
        }
      }
      DependentAssembly[] dependentAssemblyArray = new DependentAssembly[hashtable.Count];
      hashtable.Values.CopyTo((Array) dependentAssemblyArray, 0);
      return dependentAssemblyArray;
    }

    public DependentAssembly GetDependentAssemblyByIdentity(IReferenceIdentity refid)
    {
      object ppUnknown = (object) null;
      try
      {
        ((ISectionWithReferenceIdentityKey) this._cms.AssemblyReferenceSection).Lookup(refid, out ppUnknown);
      }
      catch (ArgumentException ex)
      {
        return (DependentAssembly) null;
      }
      return new DependentAssembly(((IAssemblyReferenceEntry) ppUnknown).AllData);
    }

    public File GetFileFromName(string fileName)
    {
      object ppUnknown = (object) null;
      try
      {
        ((ISectionWithStringKey) this._cms.FileSection).Lookup(fileName, out ppUnknown);
      }
      catch (ArgumentException ex)
      {
        return (File) null;
      }
      return new File(((IFileEntry) ppUnknown).AllData);
    }

    public ulong CalculateDependenciesSize()
    {
      ulong num = 0;
      foreach (File file in this.GetFilesInGroup((string) null, true))
        num += file.Size;
      foreach (DependentAssembly dependentAssembly in this.GetPrivateAssembliesInGroup((string) null, true))
        num += dependentAssembly.Size;
      return num;
    }

    private void LoadCMSFromStream(Stream stream)
    {
      AssemblyManifest.ManifestParseErrors manifestParseErrors = new AssemblyManifest.ManifestParseErrors();
      int length;
      try
      {
        length = (int) stream.Length;
        this._rawXmlBytes = new byte[length];
        if (stream.CanSeek)
          stream.Seek(0L, SeekOrigin.Begin);
        stream.Read(this._rawXmlBytes, 0, length);
      }
      catch (IOException ex)
      {
        throw new InvalidDeploymentException(ExceptionTypes.ManifestParse, Resources.GetString("Ex_ManifestReadException"), (Exception) ex);
      }
      ICMS cmsFromXml;
      try
      {
        cmsFromXml = (ICMS) IsolationInterop.CreateCMSFromXml(this._rawXmlBytes, (uint) length, (IManifestParseErrorCallback) manifestParseErrors, ref IsolationInterop.IID_ICMS);
      }
      catch (COMException ex)
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (AssemblyManifest.ManifestParseErrors.ManifestParseError manifestParseError in manifestParseErrors)
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_ManifestParseCMSErrorMessage"), (object) manifestParseError.hr, (object) manifestParseError.StartLine, (object) manifestParseError.nStartColumn, (object) manifestParseError.ErrorStatusHostFile);
        throw new InvalidDeploymentException(ExceptionTypes.ManifestParse, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_ManifestCMSParsingException"), new object[1]
        {
          (object) stringBuilder.ToString()
        }), (Exception) ex);
      }
      catch (SEHException ex)
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (AssemblyManifest.ManifestParseErrors.ManifestParseError manifestParseError in manifestParseErrors)
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_ManifestParseCMSErrorMessage"), (object) manifestParseError.hr, (object) manifestParseError.StartLine, (object) manifestParseError.nStartColumn, (object) manifestParseError.ErrorStatusHostFile);
        throw new InvalidDeploymentException(ExceptionTypes.ManifestParse, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_ManifestCMSParsingException"), new object[1]
        {
          (object) stringBuilder.ToString()
        }), (Exception) ex);
      }
      catch (ArgumentException ex)
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (AssemblyManifest.ManifestParseErrors.ManifestParseError manifestParseError in manifestParseErrors)
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_ManifestParseCMSErrorMessage"), (object) manifestParseError.hr, (object) manifestParseError.StartLine, (object) manifestParseError.nStartColumn, (object) manifestParseError.ErrorStatusHostFile);
        throw new InvalidDeploymentException(ExceptionTypes.ManifestParse, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_ManifestCMSParsingException"), new object[1]
        {
          (object) stringBuilder.ToString()
        }), (Exception) ex);
      }
      if (cmsFromXml == null)
        throw new InvalidDeploymentException(ExceptionTypes.ManifestParse, Resources.GetString("Ex_IsoNullCmsCreated"));
      this._cms = cmsFromXml;
    }

    private void LoadFromRawXmlFile(string filePath)
    {
      using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
      {
        this.LoadCMSFromStream((Stream) fileStream);
        this._rawXmlFilePath = filePath;
        this._manifestSourceFormat = ManifestSourceFormat.XmlFile;
        this._sizeInBytes = (ulong) fileStream.Length;
      }
    }

    private bool LoadFromPEResources(string filePath)
    {
      byte[] buffer = (byte[]) null;
      try
      {
        buffer = SystemUtils.GetManifestFromPEResources(filePath);
      }
      catch (Win32Exception ex)
      {
        AssemblyManifest.ManifestLoadExceptionHelper((Exception) ex, filePath);
      }
      if (buffer != null)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          this.LoadCMSFromStream((Stream) memoryStream);
        this._id1Identity = (System.Deployment.Application.DefinitionIdentity) this.Identity.Clone();
        this._id1RequestedExecutionLevel = this.RequestedExecutionLevel;
        Logger.AddInternalState("_id1Identity = " + (this._id1Identity == null ? "null" : this._id1Identity.ToString()));
        this._manifestSourceFormat = ManifestSourceFormat.ID_1;
        return true;
      }
      Logger.AddInternalState("File does not contain ID_1 manifest.");
      return false;
    }

    private static System.Deployment.Application.DefinitionIdentity ExtractIdentityFromCompLibAssembly(string filePath)
    {
      Logger.AddMethodCall("AssemblyManifest.ExtractIdentityFromCompLibAssembly(" + filePath + ") called.");
      try
      {
        using (AssemblyMetaDataImport assemblyMetaDataImport = new AssemblyMetaDataImport(filePath))
        {
          AssemblyName name = assemblyMetaDataImport.Name;
          return SystemUtils.GetDefinitionIdentityFromManagedAssembly(filePath);
        }
      }
      catch (BadImageFormatException ex)
      {
        return (System.Deployment.Application.DefinitionIdentity) null;
      }
      catch (COMException ex)
      {
        return (System.Deployment.Application.DefinitionIdentity) null;
      }
      catch (SEHException ex)
      {
        return (System.Deployment.Application.DefinitionIdentity) null;
      }
    }

    private bool LoadFromCompLibAssembly(string filePath)
    {
      try
      {
        using (AssemblyMetaDataImport assemblyMetaDataImport = new AssemblyMetaDataImport(filePath))
        {
          AssemblyName name = assemblyMetaDataImport.Name;
          this._identity = (object) SystemUtils.GetDefinitionIdentityFromManagedAssembly(filePath);
          this._complibIdentity = (System.Deployment.Application.DefinitionIdentity) this.Identity.Clone();
          AssemblyModule[] files = assemblyMetaDataImport.Files;
          AssemblyReference[] references = assemblyMetaDataImport.References;
          File[] fileArray = new File[files.Length + 1];
          fileArray[0] = new File(Path.GetFileName(filePath), 0UL);
          for (int index = 0; index < files.Length; ++index)
            fileArray[index + 1] = new File(files[index].Name, files[index].Hash, 0UL);
          this._files = (object) fileArray;
          DependentAssembly[] dependentAssemblyArray = new DependentAssembly[references.Length];
          for (int index = 0; index < references.Length; ++index)
            dependentAssemblyArray[index] = new DependentAssembly(new System.Deployment.Application.ReferenceIdentity(references[index].Name.ToString()));
          this._dependentAssemblies = (object) dependentAssemblyArray;
          this._manifestSourceFormat = ManifestSourceFormat.CompLib;
          return true;
        }
      }
      catch (BadImageFormatException ex)
      {
        return false;
      }
      catch (COMException ex)
      {
        return false;
      }
      catch (SEHException ex)
      {
        return false;
      }
      catch (IOException ex)
      {
        return false;
      }
    }

    private void LoadFromInternalManifestFile(string filePath)
    {
      PEStream peStream = (PEStream) null;
      MemoryStream memoryStream = (MemoryStream) null;
      AssemblyManifest assemblyManifest = (AssemblyManifest) null;
      bool flag = true;
      try
      {
        peStream = new PEStream(filePath, true);
        byte[] manifestResource = peStream.GetDefaultId1ManifestResource();
        if (manifestResource != null)
        {
          memoryStream = new MemoryStream(manifestResource);
          assemblyManifest = new AssemblyManifest((Stream) memoryStream);
          Logger.AddInternalState("id1Manifest is parsed successfully.");
          this._id1ManifestPresent = true;
        }
        flag = peStream.IsImageFileDll;
      }
      catch (IOException ex)
      {
        AssemblyManifest.ManifestLoadExceptionHelper((Exception) ex, filePath);
      }
      catch (Win32Exception ex)
      {
        AssemblyManifest.ManifestLoadExceptionHelper((Exception) ex, filePath);
      }
      catch (InvalidDeploymentException ex)
      {
        AssemblyManifest.ManifestLoadExceptionHelper((Exception) ex, filePath);
      }
      finally
      {
        if (peStream != null)
          peStream.Close();
        if (memoryStream != null)
          memoryStream.Close();
      }
      if (assemblyManifest != null)
      {
        if (!assemblyManifest.Identity.IsEmpty)
        {
          if (!this.LoadFromPEResources(filePath))
            AssemblyManifest.ManifestLoadExceptionHelper((Exception) new DeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_CannotLoadInternalManifest")), filePath);
          this._complibIdentity = AssemblyManifest.ExtractIdentityFromCompLibAssembly(filePath);
          Logger.AddInternalState("_complibIdentity =" + (this._complibIdentity == null ? "null" : this._complibIdentity.ToString()));
        }
        else if (!flag)
        {
          if (!this.LoadFromCompLibAssembly(filePath))
            AssemblyManifest.ManifestLoadExceptionHelper((Exception) new DeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_CannotLoadInternalManifest")), filePath);
          this._id1Identity = assemblyManifest.Identity;
          this._id1RequestedExecutionLevel = assemblyManifest.RequestedExecutionLevel;
        }
        else
          AssemblyManifest.ManifestLoadExceptionHelper((Exception) new DeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_EmptyIdentityInternalManifest")), filePath);
      }
      else
      {
        if (this.LoadFromCompLibAssembly(filePath))
          return;
        AssemblyManifest.ManifestLoadExceptionHelper((Exception) new DeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_CannotLoadInternalManifest")), filePath);
      }
    }

    private void LoadFromUnknownFormatFile(string filePath)
    {
      try
      {
        this.LoadFromRawXmlFile(filePath);
      }
      catch (InvalidDeploymentException ex)
      {
        if (ex.SubType == ExceptionTypes.ManifestParse || ex.SubType == ExceptionTypes.ManifestSemanticValidation)
          this.LoadFromInternalManifestFile(filePath);
        else
          throw;
      }
    }

    internal void ValidateSignature(Stream s)
    {
      if (string.Equals(this.Identity.PublicKeyToken, "0000000000000000", StringComparison.Ordinal) && !PolicyKeys.RequireSignedManifests())
      {
        Logger.AddWarningInformation(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("UnsignedManifest"), new object[0]));
        Logger.AddInternalState("Manifest is unsigned.");
        this._signed = false;
      }
      else
      {
        XmlDocument manifestDom = new XmlDocument();
        manifestDom.PreserveWhitespace = true;
        if (s != null)
          manifestDom.Load(s);
        else
          manifestDom.Load(this._rawXmlFilePath);
        try
        {
          CryptoConfig.AddAlgorithm(typeof (System.Deployment.Internal.CodeSigning.RSAPKCS1SHA256SignatureDescription), "http://www.w3.org/2000/09/xmldsig#rsa-sha256");
          CryptoConfig.AddAlgorithm(typeof (SHA256Cng), "http://www.w3.org/2000/09/xmldsig#sha256");
          new SignedCmiManifest(manifestDom).Verify(CmiManifestVerifyFlags.StrongNameOnly);
        }
        catch (CryptographicException ex)
        {
          throw new InvalidDeploymentException(ExceptionTypes.SignatureValidation, Resources.GetString("Ex_InvalidXmlSignature"), (Exception) ex);
        }
        if (this.RequiredHashMissing)
          throw new InvalidDeploymentException(ExceptionTypes.SignatureValidation, Resources.GetString("Ex_SignedManifestUnhashedComponent"));
        this._signed = true;
      }
    }

    internal static void ReValidateManifestSignatures(AssemblyManifest depManifest, AssemblyManifest appManifest)
    {
      if (depManifest.Signed && !appManifest.Signed)
        throw new InvalidDeploymentException(ExceptionTypes.SignatureValidation, Resources.GetString("Ex_DepSignedAppUnsigned"));
      if (!depManifest.Signed && appManifest.Signed)
        throw new InvalidDeploymentException(ExceptionTypes.SignatureValidation, Resources.GetString("Ex_AppSignedDepUnsigned"));
    }

    internal void ValidateSemanticsForDeploymentRole()
    {
      try
      {
        AssemblyManifest.ValidateAssemblyIdentity(this.Identity);
        if (this.Identity.PublicKeyToken == null)
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_DepNotStronglyNamed"));
        if (!PlatformDetector.IsSupportedProcessorArchitecture(this.Identity.ProcessorArchitecture))
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_DepProcArchNotSupported"));
        if (this.Deployment == null)
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_DepMissingDeploymentSection"));
        if (this.UseManifestForTrust)
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_DepWithUseManifestForTrust"));
        if (this.Description == null || string.IsNullOrEmpty(this.Description.FilteredPublisher) || string.IsNullOrEmpty(this.Description.FilteredProduct))
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_DepPublisherProductRequired"));
        if (this.Description.FilteredPublisher.Length + this.Description.FilteredProduct.Length > 260)
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_PublisherProductNameTooLong"));
        if (this.EntryPoints.Length != 0)
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_DepEntryPointNotAllowed"));
        if (this.Files.Length != 0)
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_DepFileNotAllowed"));
        if (this.FileAssociations.Length != 0)
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_DepFileAssocNotAllowed"));
        if (this.Description.IconFile != null)
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_DepIconFileNotAllowed"));
        if (this.Deployment.DisallowUrlActivation && !this.Deployment.Install)
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_DepOnlineOnlyAndDisallowUrlActivation"));
        if (this.Deployment.DisallowUrlActivation && this.Deployment.TrustURLParameters)
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_DepTrustUrlAndDisallowUrlActivation"));
        if (this.CompatibleFrameworks != null)
        {
          if (this.CompatibleFrameworks.SupportUrl != (Uri) null)
          {
            if (!this.CompatibleFrameworks.SupportUrl.IsAbsoluteUri)
              throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, Resources.GetString("Ex_CompatibleFrameworksSupportUrlNoAbsolute"));
            if (!System.Deployment.Application.UriHelper.IsSupportedScheme(this.CompatibleFrameworks.SupportUrl))
              throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_CompatibleFrameworksSupportUrlNotSupportedUriScheme"));
            if (this.CompatibleFrameworks.SupportUrl.AbsoluteUri.Length > 16384)
              throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, Resources.GetString("Ex_CompatibleFrameworksSupportUrlTooLong"));
          }
          if (this.CompatibleFrameworks.Frameworks.Count < 1)
            throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_DepAtLeastOneFramework"));
          for (int index = 0; index < this.CompatibleFrameworks.Frameworks.Count; ++index)
          {
            CompatibleFramework framework = this.CompatibleFrameworks.Frameworks[index];
            Version version1;
            try
            {
              version1 = new Version(framework.TargetVersion);
            }
            catch (SystemException ex)
            {
              if (ExceptionUtility.IsHardException((Exception) ex))
                throw;
              else
                throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_DepInvalidTargetVersion"), new object[1]
                {
                  (object) framework.TargetVersion
                }), (Exception) ex);
            }
            try
            {
              Version version2 = new Version(framework.SupportedRuntime);
            }
            catch (SystemException ex)
            {
              if (ExceptionUtility.IsHardException((Exception) ex))
                throw;
              else
                throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_DepInvalidSupportedRuntime"), new object[1]
                {
                  (object) framework.SupportedRuntime
                }), (Exception) ex);
            }
            switch (version1.Major)
            {
              case 2:
                if (!string.IsNullOrEmpty(framework.Profile))
                  throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_DepUnsupportedFrameworkProfile"), new object[2]
                  {
                    (object) framework.Profile,
                    (object) framework.TargetVersion
                  }));
                break;
              case 3:
                if (version1.Minor >= 1 && version1.Minor < 5)
                  throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_DepUnsupportedFrameworkTargetVersion"), new object[1]
                  {
                    (object) framework.TargetVersion
                  }));
                if (!string.IsNullOrEmpty(framework.Profile) && (version1.Minor < 5 || !"Client".Equals(framework.Profile, StringComparison.OrdinalIgnoreCase)))
                  throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_DepUnsupportedFrameworkProfile"), new object[2]
                  {
                    (object) framework.Profile,
                    (object) framework.TargetVersion
                  }));
                break;
              default:
                if (version1.Major <= 1)
                  throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_DepUnsupportedFrameworkTargetVersion"), new object[1]
                  {
                    (object) framework.TargetVersion
                  }));
                if (string.IsNullOrEmpty(framework.Profile))
                  throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_DepFrameworkProfileRequired"), new object[1]
                  {
                    (object) framework.TargetVersion
                  }));
                break;
            }
          }
        }
        if (this.Deployment.Install)
        {
          if (this.Deployment.ProviderCodebaseUri != (Uri) null)
          {
            if (!this.Deployment.ProviderCodebaseUri.IsAbsoluteUri)
              throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_DepProviderNotAbsolute"));
            if (!System.Deployment.Application.UriHelper.IsSupportedScheme(this.Deployment.ProviderCodebaseUri))
              throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_DepProviderNotSupportedUriScheme"));
            if (this.Deployment.ProviderCodebaseUri.AbsoluteUri.Length > 16384)
              throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_DepProviderTooLong"));
          }
          if (this.Deployment.MinimumRequiredVersion != (Version) null && this.Deployment.MinimumRequiredVersion > this.Identity.Version)
            throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_MinimumRequiredVersionExceedDeployment"));
        }
        else if (this.Deployment.MinimumRequiredVersion != (Version) null)
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_DepNoMinVerForOnlineApps"));
        if (this.DependentAssemblies.Length != 1)
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_DepApplicationDependencyRequired"));
        this.ValidateApplicationDependency(this.DependentAssemblies[0]);
        if (this.DependentAssemblies[0].HashCollection.Count == 0)
          this._unhashedDependencyPresent = true;
        if (this.Deployment.DeploymentUpdate.BeforeApplicationStartup && this.Deployment.DeploymentUpdate.MaximumAgeSpecified)
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_DepBeforeStartupMaxAgeBothPresent"));
        if (this.Deployment.DeploymentUpdate.MaximumAgeSpecified && this.Deployment.DeploymentUpdate.MaximumAgeAllowed > TimeSpan.FromDays(365.0))
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_MaxAgeTooLarge"));
      }
      catch (InvalidDeploymentException ex)
      {
        throw new InvalidDeploymentException(ExceptionTypes.ManifestSemanticValidation, Resources.GetString("Ex_SemanticallyInvalidDeploymentManifest"), (Exception) ex);
      }
    }

    internal void ValidateSemanticsForApplicationRole()
    {
      try
      {
        AssemblyManifest.ValidateAssemblyIdentity(this.Identity);
        if (this.EntryPoints.Length != 1)
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_AppOneEntryPoint"));
        EntryPoint entryPoint = this.EntryPoints[0];
        if (!entryPoint.CustomHostSpecified && (entryPoint.Assembly == null || entryPoint.Assembly.IsOptional || (entryPoint.Assembly.IsPreRequisite || entryPoint.Assembly.Codebase == null) || (!System.Deployment.Application.UriHelper.IsValidRelativeFilePath(entryPoint.Assembly.Codebase) || System.Deployment.Application.UriHelper.PathContainDirectorySeparators(entryPoint.Assembly.Codebase) || (!System.Deployment.Application.UriHelper.IsValidRelativeFilePath(entryPoint.CommandFile) || System.Deployment.Application.UriHelper.PathContainDirectorySeparators(entryPoint.CommandFile))) || (!entryPoint.CommandFile.Equals(entryPoint.Assembly.Codebase, StringComparison.OrdinalIgnoreCase) || string.Compare(this.Identity.ProcessorArchitecture, entryPoint.Assembly.Identity.ProcessorArchitecture, StringComparison.OrdinalIgnoreCase) != 0)))
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_AppInvalidEntryPoint"));
        if (this.Application && entryPoint.CommandParameters != null)
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_AppInvalidEntryPointParameters"));
        if (this.DependentAssemblies == null || this.DependentAssemblies.Length == 0)
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_AppAtLeastOneDependency"));
        if (this.Deployment != null)
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_AppNoDeploymentAllowed"));
        if (this.CompatibleFrameworks != null)
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_AppNoCompatibleFrameworksAllowed"));
        if (this.UseManifestForTrust)
        {
          if (this.Description == null || this.Description != null && (this.Description.Publisher == null || this.Description.Product == null))
            throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_AppNoOverridePublisherProduct"));
        }
        else if (this.Description != null && (this.Description.Publisher != null || this.Description.Product != null))
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_AppNoPublisherProductAllowed"));
        if (this.Description != null && this.Description.IconFile != null && !System.Deployment.Application.UriHelper.IsValidRelativeFilePath(this.Description.IconFile))
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_AppInvalidIconFile"));
        if (this.Description != null && this.Description.SupportUri != (Uri) null)
        {
          if (!this.Description.SupportUri.IsAbsoluteUri)
            throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_DescriptionSupportUrlNotAbsolute"));
          if (!System.Deployment.Application.UriHelper.IsSupportedScheme(this.Description.SupportUri))
            throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_DescriptionSupportUrlNotSupportedUriScheme"));
          if (this.Description.SupportUri.AbsoluteUri.Length > 16384)
            throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_DescriptionSupportUrlTooLong"));
        }
        if (this.Files.Length > 24576)
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_TooManyFilesInManifest"));
        Hashtable hashtable1 = new Hashtable();
        foreach (File file in this.Files)
        {
          AssemblyManifest.ValidateFile(file);
          if (!file.IsOptional && !hashtable1.Contains((object) file.Name))
            hashtable1.Add((object) file.Name.ToLower(), (object) file);
          if (file.HashCollection.Count == 0)
            this._unhashedFilePresent = true;
        }
        if (this.FileAssociations.Length != 0 && entryPoint.HostInBrowser)
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_FileAssociationNotSupportedForHostInBrowser"));
        if (this.FileAssociations.Length != 0 && entryPoint.CustomHostSpecified)
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_FileAssociationNotSupportedForCustomHost"));
        if (this.FileAssociations.Length > 8)
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_TooManyFileAssociationsInManifest"), new object[1]{ (object) 8 }));
        Hashtable hashtable2 = new Hashtable();
        foreach (FileAssociation fileAssociation in this.FileAssociations)
        {
          if (string.IsNullOrEmpty(fileAssociation.Extension) || string.IsNullOrEmpty(fileAssociation.Description) || (string.IsNullOrEmpty(fileAssociation.ProgID) || string.IsNullOrEmpty(fileAssociation.DefaultIcon)))
            throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_FileExtensionInfoMissing"));
          if (fileAssociation.Extension.Length > 0 && (int) fileAssociation.Extension[0] != 46)
            throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_FileAssociationExtensionNoDot"), new object[1]
            {
              (object) fileAssociation.Extension
            }));
          if (!System.Deployment.Application.UriHelper.IsValidRelativeFilePath("file" + fileAssociation.Extension))
            throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_FileAssociationInvalid"), new object[1]
            {
              (object) fileAssociation.Extension
            }));
          if (fileAssociation.Extension.Length > 24)
            throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_FileExtensionTooLong"), new object[1]
            {
              (object) fileAssociation.Extension
            }));
          if (!hashtable1.Contains((object) fileAssociation.DefaultIcon.ToLower()))
            throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_FileAssociationIconFileNotFound"), new object[1]
            {
              (object) fileAssociation.DefaultIcon
            }));
          if (hashtable2.Contains((object) fileAssociation.Extension))
            throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_MultipleInstanceFileExtension"), new object[1]
            {
              (object) fileAssociation.Extension
            }));
          hashtable2.Add((object) fileAssociation.Extension, (object) fileAssociation);
        }
        if (this.DependentAssemblies.Length > 24576)
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_TooManyAssembliesInManifest"));
        bool flag = false;
        foreach (DependentAssembly dependentAssembly in this.DependentAssemblies)
        {
          AssemblyManifest.ValidateComponentDependency(dependentAssembly);
          if (dependentAssembly.IsPreRequisite && PlatformDetector.IsCLRDependencyText(dependentAssembly.Identity.Name))
          {
            flag = true;
            this._clrDependentAssembly = dependentAssembly;
          }
          if (!dependentAssembly.IsPreRequisite && dependentAssembly.HashCollection.Count == 0)
            this._unhashedDependencyPresent = true;
        }
        this._clrDependentAssemblyChecked = true;
        if (this.DependentOS != null && this.DependentOS.SupportUrl != (Uri) null)
        {
          if (!this.DependentOS.SupportUrl.IsAbsoluteUri)
            throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_DepenedentOSSupportUrlNotAbsolute"));
          if (!System.Deployment.Application.UriHelper.IsSupportedScheme(this.DependentOS.SupportUrl))
            throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_DepenedentOSSupportUrlNotSupportedUriScheme"));
          if (this.DependentOS.SupportUrl.AbsoluteUri.Length > 16384)
            throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, Resources.GetString("Ex_DepenedentOSSupportUrlTooLong"));
        }
        if (!flag)
          throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_AppNoCLRDependency"), new object[0]));
      }
      catch (InvalidDeploymentException ex)
      {
        throw new InvalidDeploymentException(ExceptionTypes.ManifestSemanticValidation, Resources.GetString("Ex_SemanticallyInvalidApplicationManifest"), (Exception) ex);
      }
    }

    internal static AssemblyManifest.CertificateStatus AnalyzeManifestCertificate(string manifestPath)
    {
      Logger.AddMethodCall("AnalyzeManifestCertificate called.");
      AssemblyManifest.CertificateStatus certificateStatus = AssemblyManifest.CertificateStatus.UnknownCertificateStatus;
      SignedCmiManifest signedCmiManifest = (SignedCmiManifest) null;
      try
      {
        XmlDocument manifestDom = new XmlDocument();
        manifestDom.PreserveWhitespace = true;
        manifestDom.Load(manifestPath);
        signedCmiManifest = new SignedCmiManifest(manifestDom);
        signedCmiManifest.Verify(CmiManifestVerifyFlags.None);
        certificateStatus = signedCmiManifest == null || signedCmiManifest.AuthenticodeSignerInfo == null ? AssemblyManifest.CertificateStatus.NoCertificate : AssemblyManifest.CertificateStatus.TrustedPublisher;
      }
      catch (Exception ex)
      {
        if (ExceptionUtility.IsHardException(ex))
        {
          throw;
        }
        else
        {
          if (ex is CryptographicException && signedCmiManifest.AuthenticodeSignerInfo != null)
          {
            switch (signedCmiManifest.AuthenticodeSignerInfo.ErrorCode)
            {
              case -2146762479:
                certificateStatus = AssemblyManifest.CertificateStatus.DistrustedPublisher;
                break;
              case -2146885616:
                certificateStatus = AssemblyManifest.CertificateStatus.RevokedCertificate;
                break;
              case -2146762748:
                certificateStatus = AssemblyManifest.CertificateStatus.AuthenticodedNotInTrustedList;
                break;
              default:
                certificateStatus = AssemblyManifest.CertificateStatus.NoCertificate;
                break;
            }
          }
          Logger.AddInternalState("Exception thrown : " + ex.GetType().ToString() + ":" + ex.Message);
        }
      }
      Logger.AddInternalState("Certificate Status=" + certificateStatus.ToString());
      return certificateStatus;
    }

    private static void ValidateAssemblyIdentity(System.Deployment.Application.DefinitionIdentity identity)
    {
      if (identity.Name != null && (identity.Name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || identity.Name.IndexOfAny(Path.GetInvalidPathChars()) >= 0 || identity.Name.IndexOfAny(AssemblyManifest.SpecificInvalidIdentityChars) >= 0))
      {
        string str = new string(Path.GetInvalidFileNameChars()) + " " + new string(Path.GetInvalidPathChars()) + " " + new string(AssemblyManifest.SpecificInvalidIdentityChars);
        Logger.AddInternalState(identity.Name + " contains an invalid character. InvalidIdentityChars=[" + str + "].");
        throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_IdentityWithInvalidChars"), new object[1]
        {
          (object) identity.Name
        }));
      }
      try
      {
        if (identity.ToString().Length > 2048)
          throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, Resources.GetString("Ex_IdentityTooLong"));
      }
      catch (COMException ex)
      {
        throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, Resources.GetString("Ex_IdentityIsNotValid"), (Exception) ex);
      }
      catch (SEHException ex)
      {
        throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, Resources.GetString("Ex_IdentityIsNotValid"), (Exception) ex);
      }
    }

    private static void ValidateAssemblyIdentity(System.Deployment.Application.ReferenceIdentity identity)
    {
      if (identity.Name != null && (identity.Name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || identity.Name.IndexOfAny(Path.GetInvalidPathChars()) >= 0 || identity.Name.IndexOfAny(AssemblyManifest.SpecificInvalidIdentityChars) >= 0))
      {
        string str = new string(Path.GetInvalidFileNameChars()) + " " + new string(Path.GetInvalidPathChars()) + " " + new string(AssemblyManifest.SpecificInvalidIdentityChars);
        Logger.AddInternalState(identity.Name + " contains an invalid character. InvalidIdentityChars= [" + str + "].");
        throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_IdentityWithInvalidChars"), new object[1]
        {
          (object) identity.Name
        }));
      }
      try
      {
        if (identity.ToString().Length > 2048)
          throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, Resources.GetString("Ex_IdentityTooLong"));
      }
      catch (COMException ex)
      {
        throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, Resources.GetString("Ex_IdentityIsNotValid"), (Exception) ex);
      }
      catch (SEHException ex)
      {
        throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, Resources.GetString("Ex_IdentityIsNotValid"), (Exception) ex);
      }
    }

    private void ValidateApplicationDependency(DependentAssembly da)
    {
      AssemblyManifest.ValidateAssemblyIdentity(da.Identity);
      if (da.Identity.PublicKeyToken == null)
        throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, Resources.GetString("Ex_DepAppRefNotStrongNamed"));
      if (AssemblyManifest.IsInvalidHash(da.HashCollection))
        throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, Resources.GetString("Ex_DepAppRefHashInvalid"));
      if (string.Compare(this.Identity.ProcessorArchitecture, da.Identity.ProcessorArchitecture, StringComparison.OrdinalIgnoreCase) != 0)
        throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_DepAppRefProcArchMismatched"), new object[2]
        {
          (object) da.Identity.ProcessorArchitecture,
          (object) this.Identity.ProcessorArchitecture
        }));
      if (da.ResourceFallbackCulture != null || da.IsPreRequisite || da.IsOptional)
        throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, Resources.GetString("Ex_DepAppRefPrereqOrOptionalOrResourceFallback"));
      Uri uri;
      try
      {
        uri = new Uri(da.Codebase, UriKind.RelativeOrAbsolute);
      }
      catch (UriFormatException ex)
      {
        throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, Resources.GetString("Ex_DepAppRefInvalidCodebaseUri"), (Exception) ex);
      }
      if (uri.IsAbsoluteUri && !System.Deployment.Application.UriHelper.IsSupportedScheme(uri))
        throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, Resources.GetString("Ex_DepAppRefInvalidCodebaseUri"));
      if (!System.Deployment.Application.UriHelper.IsValidRelativeFilePath(da.Identity.Name) || System.Deployment.Application.UriHelper.PathContainDirectorySeparators(da.Identity.Name))
        throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_DepAppRefInvalidIdentityName"), new object[1]
        {
          (object) da.Identity.Name
        }));
    }

    private static void ValidateComponentDependency(DependentAssembly da)
    {
      AssemblyManifest.ValidateAssemblyIdentity(da.Identity);
      if (!da.IsPreRequisite)
      {
        if (da.ResourceFallbackCulture == null)
        {
          if (AssemblyManifest.IsInvalidHash(da.HashCollection))
            throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_DependencyInvalidHash"), new object[1]
            {
              (object) da.Identity.ToString()
            }));
          if (da.Codebase == null)
            throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_DependencyNoCodebase"), new object[1]
            {
              (object) da.Identity.ToString()
            }));
          if (!System.Deployment.Application.UriHelper.IsValidRelativeFilePath(da.Codebase))
            throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_DependencyNotRelativePath"), new object[1]
            {
              (object) da.Identity.ToString()
            }));
          if (da.IsOptional && da.Group == null)
            throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_DependencyOptionalButNoGroup"), new object[1]
            {
              (object) da.Identity.ToString()
            }));
        }
        else if (da.Identity.Culture == null)
        {
          if (da.Codebase != null)
            throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_DependencyResourceWithCodebase"), new object[1]
            {
              (object) da.Identity.ToString()
            }));
          if (da.HashCollection.Count > 0)
            throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_DependencyResourceWithHash"), new object[1]
            {
              (object) da.Identity.ToString()
            }));
        }
        else
        {
          if (AssemblyManifest.IsInvalidHash(da.HashCollection))
            throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_DependencyInvalidHash"), new object[1]
            {
              (object) da.Identity.ToString()
            }));
          if (da.Codebase == null)
            throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_DependencyNoCodebase"), new object[1]
            {
              (object) da.Identity.ToString()
            }));
          if (!System.Deployment.Application.UriHelper.IsValidRelativeFilePath(da.Codebase))
            throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_DependencyNotRelativePath"), new object[1]
            {
              (object) da.Identity.ToString()
            }));
          if (da.ResourceFallbackCulture != null)
            throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_DependencyResourceWithFallback"), new object[1]
            {
              (object) da.Identity.ToString()
            }));
        }
      }
      else if (!PlatformDetector.IsCLRDependencyText(da.Identity.Name) && da.Identity.PublicKeyToken == null)
        throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_DependencyGACNoPKT"), new object[1]
        {
          (object) da.Identity.ToString()
        }));
      if (!(da.SupportUrl != (Uri) null))
        return;
      if (!da.SupportUrl.IsAbsoluteUri)
        throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_DependencySupportUrlNoAbsolute"), new object[1]
        {
          (object) da.Identity.ToString()
        }));
      if (!System.Deployment.Application.UriHelper.IsSupportedScheme(da.SupportUrl))
        throw new InvalidDeploymentException(ExceptionTypes.InvalidManifest, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_DependencySupportUrlNotSupportedUriScheme"), new object[1]
        {
          (object) da.Identity.ToString()
        }));
      if (da.SupportUrl.AbsoluteUri.Length > 16384)
        throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_DependencySupportUrlTooLong"), new object[1]
        {
          (object) da.Identity.ToString()
        }));
    }

    private static void ValidateFile(File f)
    {
      if (AssemblyManifest.IsInvalidHash(f.HashCollection))
        throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_InvalidFileHash"), new object[1]
        {
          (object) f.Name
        }));
      if (!System.Deployment.Application.UriHelper.IsValidRelativeFilePath(f.Name))
        throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_FilePathNotRelative"), new object[1]
        {
          (object) f.Name
        }));
      if (f.IsOptional && f.Group == null)
        throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_FileOptionalButNoGroup"), new object[1]
        {
          (object) f.Name
        }));
      if (f.IsOptional && f.IsData)
        throw new InvalidDeploymentException(ExceptionTypes.ManifestComponentSemanticValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_FileOptionalAndData"), new object[1]
        {
          (object) f.Name
        }));
    }

    private static bool IsInvalidHash(HashCollection hashCollection)
    {
      return !ComponentVerifier.IsVerifiableHashCollection(hashCollection);
    }

    internal static Uri UriFromMetadataEntry(string uriString, string exResourceStr)
    {
      try
      {
        return uriString != null ? new Uri(uriString) : (Uri) null;
      }
      catch (UriFormatException ex)
      {
        throw new InvalidDeploymentException(ExceptionTypes.Manifest, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString(exResourceStr), new object[1]
        {
          (object) uriString
        }), (Exception) ex);
      }
    }

    private static void ManifestLoadExceptionHelper(Exception exception, string filePath)
    {
      throw new InvalidDeploymentException(ExceptionTypes.ManifestLoad, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_ManifestLoadFromFile"), new object[1]
      {
        (object) Path.GetFileName(filePath)
      }), exception);
    }

    internal enum ManifestType
    {
      Application,
      Deployment,
    }

    protected class ManifestParseErrors : IManifestParseErrorCallback, IEnumerable
    {
      protected ArrayList _parsingErrors = new ArrayList();

      public void OnError(uint StartLine, uint nStartColumn, uint cCharacterCount, int hr, string ErrorStatusHostFile, uint ParameterCount, string[] Parameters)
      {
        this._parsingErrors.Add((object) new AssemblyManifest.ManifestParseErrors.ManifestParseError()
        {
          StartLine = StartLine,
          nStartColumn = nStartColumn,
          cCharacterCount = cCharacterCount,
          hr = hr,
          ErrorStatusHostFile = ErrorStatusHostFile,
          ParameterCount = ParameterCount,
          Parameters = Parameters
        });
      }

      public AssemblyManifest.ManifestParseErrors.ParseErrorEnumerator GetEnumerator()
      {
        return new AssemblyManifest.ManifestParseErrors.ParseErrorEnumerator(this);
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) this.GetEnumerator();
      }

      public class ManifestParseError
      {
        public uint StartLine;
        public uint nStartColumn;
        public uint cCharacterCount;
        public int hr;
        public string ErrorStatusHostFile;
        public uint ParameterCount;
        public string[] Parameters;
      }

      public class ParseErrorEnumerator : IEnumerator
      {
        private int _index;
        private AssemblyManifest.ManifestParseErrors _manifestParseErrors;

        public AssemblyManifest.ManifestParseErrors.ManifestParseError Current
        {
          get
          {
            return (AssemblyManifest.ManifestParseErrors.ManifestParseError) this._manifestParseErrors._parsingErrors[this._index];
          }
        }

        object IEnumerator.Current
        {
          get
          {
            return this._manifestParseErrors._parsingErrors[this._index];
          }
        }

        public ParseErrorEnumerator(AssemblyManifest.ManifestParseErrors manifestParseErrors)
        {
          this._manifestParseErrors = manifestParseErrors;
          this._index = -1;
        }

        public void Reset()
        {
          this._index = -1;
        }

        public bool MoveNext()
        {
          this._index = this._index + 1;
          return this._index < this._manifestParseErrors._parsingErrors.Count;
        }
      }
    }

    internal enum CertificateStatus
    {
      TrustedPublisher,
      AuthenticodedNotInTrustedList,
      NoCertificate,
      DistrustedPublisher,
      RevokedCertificate,
      UnknownCertificateStatus,
    }
  }
}
