// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.Manifest.DependentAssembly
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Deployment.Internal.Isolation;
using System.Deployment.Internal.Isolation.Manifest;
using System.Runtime.InteropServices;

namespace System.Deployment.Application.Manifest
{
  internal class DependentAssembly
  {
    private HashCollection _hashCollection = new HashCollection();
    private readonly ulong _size;
    private readonly string _codebase;
    private readonly System.Deployment.Application.ReferenceIdentity _identity;
    private readonly string _group;
    private readonly string _codebaseFS;
    private readonly string _description;
    private readonly Uri _supportUrl;
    private readonly string _resourceFallbackCulture;
    private readonly bool _resourceFallbackCultureInternal;
    private readonly bool _optional;
    private readonly bool _visible;
    private readonly bool _preRequisite;

    public System.Deployment.Application.ReferenceIdentity Identity
    {
      get
      {
        return this._identity;
      }
    }

    public string Codebase
    {
      get
      {
        return this._codebase;
      }
    }

    public ulong Size
    {
      get
      {
        return this._size;
      }
    }

    public string Group
    {
      get
      {
        return this._group;
      }
    }

    public string CodebaseFS
    {
      get
      {
        return this._codebaseFS;
      }
    }

    public string Description
    {
      get
      {
        return this._description;
      }
    }

    public Uri SupportUrl
    {
      get
      {
        return this._supportUrl;
      }
    }

    public string ResourceFallbackCulture
    {
      get
      {
        return this._resourceFallbackCulture;
      }
    }

    public bool IsPreRequisite
    {
      get
      {
        return this._preRequisite;
      }
    }

    public bool IsOptional
    {
      get
      {
        return this._optional;
      }
    }

    public HashCollection HashCollection
    {
      get
      {
        return this._hashCollection;
      }
    }

    public DependentAssembly(System.Deployment.Application.ReferenceIdentity refId)
    {
      this._identity = refId;
    }

    public DependentAssembly(AssemblyReferenceEntry assemblyReferenceEntry)
    {
      AssemblyReferenceDependentAssemblyEntry dependentAssembly = assemblyReferenceEntry.DependentAssembly;
      this._size = dependentAssembly.Size;
      this._codebase = dependentAssembly.Codebase;
      this._group = dependentAssembly.Group;
      bool flag = false;
      ISection hashElements = dependentAssembly.HashElements;
      uint celt = hashElements != null ? hashElements.Count : 0U;
      if (celt > 0U)
      {
        uint celtFetched = 0;
        IHashElementEntry[] hashElementEntryArray = new IHashElementEntry[(int) celt];
        Marshal.ThrowExceptionForHR(((System.Deployment.Internal.Isolation.IEnumUnknown) hashElements._NewEnum).Next(celt, (object[]) hashElementEntryArray, ref celtFetched));
        if ((int) celtFetched != (int) celt)
          throw new InvalidDeploymentException(ExceptionTypes.Manifest, Resources.GetString("Ex_IsoEnumFetchNotEqualToCount"));
        for (uint index = 0; index < celt; ++index)
        {
          HashElementEntry allData = hashElementEntryArray[(int) index].AllData;
          if (allData.DigestValueSize > 0U)
          {
            byte[] numArray = new byte[(int) allData.DigestValueSize];
            Marshal.Copy(allData.DigestValue, numArray, 0, (int) allData.DigestValueSize);
            this._hashCollection.AddHash(numArray, (CMS_HASH_DIGESTMETHOD) allData.DigestMethod, (CMS_HASH_TRANSFORM) allData.Transform);
            flag = true;
          }
        }
      }
      if (!flag && dependentAssembly.HashValueSize > 0U)
      {
        byte[] numArray = new byte[(int) dependentAssembly.HashValueSize];
        Marshal.Copy(dependentAssembly.HashValue, numArray, 0, (int) dependentAssembly.HashValueSize);
        this._hashCollection.AddHash(numArray, (CMS_HASH_DIGESTMETHOD) dependentAssembly.HashAlgorithm, CMS_HASH_TRANSFORM.CMS_HASH_TRANSFORM_IDENTITY);
      }
      this._preRequisite = (dependentAssembly.Flags & 4U) > 0U;
      this._optional = (assemblyReferenceEntry.Flags & 1U) > 0U;
      this._visible = (dependentAssembly.Flags & 2U) > 0U;
      this._resourceFallbackCultureInternal = (dependentAssembly.Flags & 8U) > 0U;
      this._resourceFallbackCulture = dependentAssembly.ResourceFallbackCulture;
      this._description = dependentAssembly.Description;
      this._supportUrl = AssemblyManifest.UriFromMetadataEntry(dependentAssembly.SupportUrl, "Ex_DependencySupportUrlNotValid");
      this._identity = new System.Deployment.Application.ReferenceIdentity(assemblyReferenceEntry.ReferenceIdentity);
      this._codebaseFS = System.Deployment.Application.UriHelper.NormalizePathDirectorySeparators(this._codebase);
    }
  }
}
