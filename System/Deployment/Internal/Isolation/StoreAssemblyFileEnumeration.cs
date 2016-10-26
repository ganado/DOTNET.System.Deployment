// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.StoreAssemblyFileEnumeration
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Collections;
using System.Security;

namespace System.Deployment.Internal.Isolation
{
  internal class StoreAssemblyFileEnumeration : IEnumerator
  {
    private IEnumSTORE_ASSEMBLY_FILE _enum;
    private bool _fValid;
    private STORE_ASSEMBLY_FILE _current;

    object IEnumerator.Current
    {
      get
      {
        return (object) this.GetCurrent();
      }
    }

    public STORE_ASSEMBLY_FILE Current
    {
      get
      {
        return this.GetCurrent();
      }
    }

    public StoreAssemblyFileEnumeration(IEnumSTORE_ASSEMBLY_FILE pI)
    {
      this._enum = pI;
    }

    public IEnumerator GetEnumerator()
    {
      return (IEnumerator) this;
    }

    private STORE_ASSEMBLY_FILE GetCurrent()
    {
      if (!this._fValid)
        throw new InvalidOperationException();
      return this._current;
    }

    [SecuritySafeCritical]
    public bool MoveNext()
    {
      STORE_ASSEMBLY_FILE[] rgelt = new STORE_ASSEMBLY_FILE[1];
      uint num = this._enum.Next(1U, rgelt);
      if ((int) num == 1)
        this._current = rgelt[0];
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
