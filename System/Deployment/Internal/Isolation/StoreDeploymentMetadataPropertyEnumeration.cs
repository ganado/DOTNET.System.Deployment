﻿// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.StoreDeploymentMetadataPropertyEnumeration
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Collections;
using System.Security;

namespace System.Deployment.Internal.Isolation
{
  internal class StoreDeploymentMetadataPropertyEnumeration : IEnumerator
  {
    private IEnumSTORE_DEPLOYMENT_METADATA_PROPERTY _enum;
    private bool _fValid;
    private StoreOperationMetadataProperty _current;

    object IEnumerator.Current
    {
      get
      {
        return (object) this.GetCurrent();
      }
    }

    public StoreOperationMetadataProperty Current
    {
      get
      {
        return this.GetCurrent();
      }
    }

    public StoreDeploymentMetadataPropertyEnumeration(IEnumSTORE_DEPLOYMENT_METADATA_PROPERTY pI)
    {
      this._enum = pI;
    }

    private StoreOperationMetadataProperty GetCurrent()
    {
      if (!this._fValid)
        throw new InvalidOperationException();
      return this._current;
    }

    public IEnumerator GetEnumerator()
    {
      return (IEnumerator) this;
    }

    [SecuritySafeCritical]
    public bool MoveNext()
    {
      StoreOperationMetadataProperty[] AppIds = new StoreOperationMetadataProperty[1];
      uint num = this._enum.Next(1U, AppIds);
      if ((int) num == 1)
        this._current = AppIds[0];
      return this._fValid = (int) num == 1;
    }

    [SecuritySafeCritical]
    public void Reset()
    {
      this._fValid = false;
      this._enum.Reset();
    }
  }
}
