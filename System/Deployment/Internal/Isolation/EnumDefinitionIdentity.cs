// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.EnumDefinitionIdentity
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Collections;

namespace System.Deployment.Internal.Isolation
{
  internal sealed class EnumDefinitionIdentity : IEnumerator
  {
    private IDefinitionIdentity[] _fetchList = new IDefinitionIdentity[1];
    private IEnumDefinitionIdentity _enum;
    private IDefinitionIdentity _current;

    object IEnumerator.Current
    {
      get
      {
        return (object) this.GetCurrent();
      }
    }

    public DefinitionIdentity Current
    {
      get
      {
        return this.GetCurrent();
      }
    }

    internal EnumDefinitionIdentity(IEnumDefinitionIdentity e)
    {
      if (e == null)
        throw new ArgumentNullException();
      this._enum = e;
    }

    private DefinitionIdentity GetCurrent()
    {
      if (this._current == null)
        throw new InvalidOperationException();
      return new DefinitionIdentity(this._current);
    }

    public IEnumerator GetEnumerator()
    {
      return (IEnumerator) this;
    }

    public bool MoveNext()
    {
      if ((int) this._enum.Next(1U, this._fetchList) == 1)
      {
        this._current = this._fetchList[0];
        return true;
      }
      this._current = (IDefinitionIdentity) null;
      return false;
    }

    public void Reset()
    {
      this._current = (IDefinitionIdentity) null;
      this._enum.Reset();
    }
  }
}
