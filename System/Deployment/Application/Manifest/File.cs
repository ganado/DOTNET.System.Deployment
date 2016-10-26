// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.Manifest.File
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Deployment.Internal.Isolation;
using System.Deployment.Internal.Isolation.Manifest;
using System.Runtime.InteropServices;

namespace System.Deployment.Application.Manifest
{
  internal class File
  {
    private HashCollection _hashCollection = new HashCollection();
    private readonly string _name;
    private readonly string _loadFrom;
    private readonly ulong _size;
    private readonly string _group;
    private readonly bool _optional;
    private readonly bool _isData;
    private readonly string _nameFS;

    public string Name
    {
      get
      {
        return this._name;
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

    public bool IsOptional
    {
      get
      {
        return this._optional;
      }
    }

    public bool IsData
    {
      get
      {
        return this._isData;
      }
    }

    public string NameFS
    {
      get
      {
        return this._nameFS;
      }
    }

    public HashCollection HashCollection
    {
      get
      {
        return this._hashCollection;
      }
    }

    protected internal File(string name, ulong size)
    {
      this._name = name;
      this._size = size;
      this._nameFS = System.Deployment.Application.UriHelper.NormalizePathDirectorySeparators(this._name);
    }

    public File(string name, byte[] hash, ulong size)
    {
      this._name = name;
      this._hashCollection.AddHash(hash, CMS_HASH_DIGESTMETHOD.CMS_HASH_DIGESTMETHOD_SHA1, CMS_HASH_TRANSFORM.CMS_HASH_TRANSFORM_IDENTITY);
      this._size = size;
      this._nameFS = System.Deployment.Application.UriHelper.NormalizePathDirectorySeparators(this._name);
    }

    public File(FileEntry fileEntry)
    {
      this._name = fileEntry.Name;
      this._loadFrom = fileEntry.LoadFrom;
      this._size = fileEntry.Size;
      this._group = fileEntry.Group;
      this._optional = (fileEntry.Flags & 1U) > 0U;
      this._isData = (fileEntry.WritableType & 2U) > 0U;
      bool flag = false;
      ISection hashElements = fileEntry.HashElements;
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
      if (!flag && fileEntry.HashValueSize > 0U)
      {
        byte[] numArray = new byte[(int) fileEntry.HashValueSize];
        Marshal.Copy(fileEntry.HashValue, numArray, 0, (int) fileEntry.HashValueSize);
        this._hashCollection.AddHash(numArray, (CMS_HASH_DIGESTMETHOD) fileEntry.HashAlgorithm, CMS_HASH_TRANSFORM.CMS_HASH_TRANSFORM_IDENTITY);
      }
      this._nameFS = System.Deployment.Application.UriHelper.NormalizePathDirectorySeparators(this._name);
    }
  }
}
