// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.DefinitionIdentity
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Collections;
using System.Deployment.Internal.Isolation;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Deployment.Application
{
  internal class DefinitionIdentity : ICloneable
  {
    private IDefinitionIdentity _idComPtr;

    public string this[string name]
    {
      get
      {
        return this._idComPtr.GetAttribute((string) null, name);
      }
      set
      {
        this._idComPtr.SetAttribute((string) null, name, value);
      }
    }

    public string this[string ns, string name]
    {
      set
      {
        this._idComPtr.SetAttribute(ns, name, value);
      }
    }

    public string Name
    {
      get
      {
        return this["name"];
      }
      set
      {
        this["name"] = value;
      }
    }

    public Version Version
    {
      get
      {
        string version = this["version"];
        if (version == null)
          return (Version) null;
        return new Version(version);
      }
    }

    public string PublicKeyToken
    {
      get
      {
        return this["publicKeyToken"];
      }
    }

    public string ProcessorArchitecture
    {
      get
      {
        return this["processorArchitecture"];
      }
    }

    public ulong Hash
    {
      get
      {
        return IsolationInterop.IdentityAuthority.HashDefinition(0U, this._idComPtr);
      }
    }

    public string KeyForm
    {
      get
      {
        return IsolationInterop.IdentityAuthority.GenerateDefinitionKey(0U, this._idComPtr);
      }
    }

    public IDENTITY_ATTRIBUTE[] Attributes
    {
      get
      {
        IEnumIDENTITY_ATTRIBUTE identityAttribute = (IEnumIDENTITY_ATTRIBUTE) null;
        try
        {
          ArrayList arrayList = new ArrayList();
          identityAttribute = this._idComPtr.EnumAttributes();
          IDENTITY_ATTRIBUTE[] rgAttributes = new IDENTITY_ATTRIBUTE[1];
          while ((int) identityAttribute.Next(1U, rgAttributes) == 1)
            arrayList.Add((object) rgAttributes[0]);
          return (IDENTITY_ATTRIBUTE[]) arrayList.ToArray(typeof (IDENTITY_ATTRIBUTE));
        }
        finally
        {
          if (identityAttribute != null)
            Marshal.ReleaseComObject((object) identityAttribute);
        }
      }
    }

    public bool IsEmpty
    {
      get
      {
        foreach (IDENTITY_ATTRIBUTE attribute in this.Attributes)
        {
          if (!string.IsNullOrEmpty(attribute.Value))
            return false;
        }
        return true;
      }
    }

    public IDefinitionIdentity ComPointer
    {
      get
      {
        return this._idComPtr;
      }
    }

    public DefinitionIdentity()
    {
      this._idComPtr = IsolationInterop.IdentityAuthority.CreateDefinition();
    }

    public DefinitionIdentity(string text)
    {
      this._idComPtr = IsolationInterop.IdentityAuthority.TextToDefinition(0U, text);
    }

    public DefinitionIdentity(IDefinitionIdentity idComPtr)
    {
      this._idComPtr = idComPtr;
    }

    public DefinitionIdentity(ReferenceIdentity refId)
    {
      this._idComPtr = IsolationInterop.IdentityAuthority.CreateDefinition();
      foreach (IDENTITY_ATTRIBUTE attribute in refId.Attributes)
        this[attribute.Namespace, attribute.Name] = attribute.Value;
    }

    public DefinitionIdentity(AssemblyName asmName)
    {
      this._idComPtr = IsolationInterop.IdentityAuthority.CreateDefinition();
      this["name"] = asmName.Name;
      this["version"] = asmName.Version.ToString();
      if (asmName.CultureInfo != null)
        this["culture"] = asmName.CultureInfo.Name;
      byte[] publicKeyToken = asmName.GetPublicKeyToken();
      if (publicKeyToken == null || publicKeyToken.Length == 0)
        return;
      this["publicKeyToken"] = HexString.FromBytes(publicKeyToken);
    }

    public bool Matches(ReferenceIdentity refId, bool exact)
    {
      if (IsolationInterop.IdentityAuthority.DoesDefinitionMatchReference(exact ? 1U : 0U, this._idComPtr, refId.ComPointer))
        return this.Version == refId.Version;
      return false;
    }

    public DefinitionIdentity ToSubscriptionId()
    {
      DefinitionIdentity definitionIdentity = (DefinitionIdentity) this.Clone();
      definitionIdentity["version"] = (string) null;
      return definitionIdentity;
    }

    public DefinitionIdentity ToPKTGroupId()
    {
      DefinitionIdentity definitionIdentity = (DefinitionIdentity) this.Clone();
      definitionIdentity["version"] = (string) null;
      definitionIdentity["publicKeyToken"] = (string) null;
      return definitionIdentity;
    }

    public override bool Equals(object obj)
    {
      if (obj is DefinitionIdentity)
        return IsolationInterop.IdentityAuthority.AreDefinitionsEqual(0U, this.ComPointer, ((DefinitionIdentity) obj).ComPointer);
      return false;
    }

    public override int GetHashCode()
    {
      return (int) this.Hash;
    }

    public override string ToString()
    {
      return IsolationInterop.IdentityAuthority.DefinitionToText(0U, this._idComPtr);
    }

    public object Clone()
    {
      return (object) new DefinitionIdentity(this._idComPtr.Clone(IntPtr.Zero, (IDENTITY_ATTRIBUTE[]) null));
    }
  }
}
