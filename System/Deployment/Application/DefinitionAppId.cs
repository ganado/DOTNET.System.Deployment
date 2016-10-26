// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.DefinitionAppId
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Deployment.Internal.Isolation;
using System.Runtime.InteropServices;

namespace System.Deployment.Application
{
  internal class DefinitionAppId
  {
    private IDefinitionAppId _idComPtr;

    public ulong Hash
    {
      get
      {
        return IsolationInterop.AppIdAuthority.HashDefinition(0U, this._idComPtr);
      }
    }

    public IDefinitionAppId ComPointer
    {
      get
      {
        return this._idComPtr;
      }
    }

    public string Codebase
    {
      get
      {
        return this._idComPtr.get_Codebase();
      }
    }

    public DefinitionIdentity DeploymentIdentity
    {
      get
      {
        return this.PathComponent(0U);
      }
    }

    public DefinitionIdentity ApplicationIdentity
    {
      get
      {
        return this.PathComponent(1U);
      }
    }

    public DefinitionAppId()
    {
      this._idComPtr = IsolationInterop.AppIdAuthority.CreateDefinition();
    }

    public DefinitionAppId(params DefinitionIdentity[] idPath)
      : this((string) null, idPath)
    {
    }

    public DefinitionAppId(string codebase, params DefinitionIdentity[] idPath)
    {
      uint length = (uint) idPath.Length;
      IDefinitionIdentity[] DefinitionIdentity = new IDefinitionIdentity[(int) length];
      for (uint index = 0; index < length; ++index)
        DefinitionIdentity[(int) index] = idPath[(int) index].ComPointer;
      this._idComPtr = IsolationInterop.AppIdAuthority.CreateDefinition();
      this._idComPtr.put_Codebase(codebase);
      this._idComPtr.SetAppPath(length, DefinitionIdentity);
    }

    public DefinitionAppId(string text)
    {
      this._idComPtr = IsolationInterop.AppIdAuthority.TextToDefinition(0U, text);
    }

    public DefinitionAppId(IDefinitionAppId idComPtr)
    {
      this._idComPtr = idComPtr;
    }

    public DefinitionAppId ToDeploymentAppId()
    {
      return new DefinitionAppId(this.Codebase, new DefinitionIdentity[1]
      {
        this.DeploymentIdentity
      });
    }

    public System.ApplicationIdentity ToApplicationIdentity()
    {
      return new System.ApplicationIdentity(IsolationInterop.AppIdAuthority.DefinitionToText(0U, this._idComPtr));
    }

    public override bool Equals(object obj)
    {
      if (obj is DefinitionAppId)
        return IsolationInterop.AppIdAuthority.AreDefinitionsEqual(0U, this.ComPointer, ((DefinitionAppId) obj).ComPointer);
      return false;
    }

    public override int GetHashCode()
    {
      return (int) this.Hash;
    }

    public override string ToString()
    {
      return IsolationInterop.AppIdAuthority.DefinitionToText(0U, this._idComPtr);
    }

    private DefinitionIdentity PathComponent(uint index)
    {
      IEnumDefinitionIdentity definitionIdentity = (IEnumDefinitionIdentity) null;
      try
      {
        definitionIdentity = this._idComPtr.EnumAppPath();
        if (index > 0U)
          definitionIdentity.Skip(index);
        IDefinitionIdentity[] DefinitionIdentity = new IDefinitionIdentity[1];
        return (int) definitionIdentity.Next(1U, DefinitionIdentity) == 1 ? new DefinitionIdentity(DefinitionIdentity[0]) : (DefinitionIdentity) null;
      }
      finally
      {
        if (definitionIdentity != null)
          Marshal.ReleaseComObject((object) definitionIdentity);
      }
    }
  }
}
