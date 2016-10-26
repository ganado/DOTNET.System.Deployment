// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.ReferenceIdentity
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Collections;
using System.Deployment.Internal.Isolation;
using System.Runtime.InteropServices;

namespace System.Deployment.Application
{
  internal class ReferenceIdentity : ICloneable
  {
    private IReferenceIdentity _idComPtr;

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

    public string Name
    {
      get
      {
        return this["name"];
      }
    }

    public string Culture
    {
      get
      {
        return this["culture"];
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
      set
      {
        this["processorArchitecture"] = value;
      }
    }

    public ulong Hash
    {
      get
      {
        return IsolationInterop.IdentityAuthority.HashReference(0U, this._idComPtr);
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

    public IReferenceIdentity ComPointer
    {
      get
      {
        return this._idComPtr;
      }
    }

    public ReferenceIdentity()
    {
      this._idComPtr = IsolationInterop.IdentityAuthority.CreateReference();
    }

    public ReferenceIdentity(string text)
    {
      this._idComPtr = IsolationInterop.IdentityAuthority.TextToReference(0U, text);
    }

    public ReferenceIdentity(IReferenceIdentity idComPtr)
    {
      this._idComPtr = idComPtr;
    }

    public override bool Equals(object obj)
    {
      if (obj is ReferenceIdentity)
        return IsolationInterop.IdentityAuthority.AreReferencesEqual(0U, this.ComPointer, ((ReferenceIdentity) obj).ComPointer);
      return false;
    }

    public override int GetHashCode()
    {
      return (int) this.Hash;
    }

    public override string ToString()
    {
      return IsolationInterop.IdentityAuthority.ReferenceToText(0U, this._idComPtr);
    }

    public object Clone()
    {
      return (object) new ReferenceIdentity(this._idComPtr.Clone(IntPtr.Zero, (IDENTITY_ATTRIBUTE[]) null));
    }
  }
}
