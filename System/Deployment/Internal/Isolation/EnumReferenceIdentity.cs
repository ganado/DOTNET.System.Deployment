// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.EnumReferenceIdentity
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Collections;

namespace System.Deployment.Internal.Isolation
{
  internal sealed class EnumReferenceIdentity : IEnumerator
  {
    private IReferenceIdentity[] _fetchList = new IReferenceIdentity[1];
    private IEnumReferenceIdentity _enum;
    private IReferenceIdentity _current;

    object IEnumerator.Current
    {
      get
      {
        return (object) this.GetCurrent();
      }
    }

    public ReferenceIdentity Current
    {
      get
      {
        return this.GetCurrent();
      }
    }

    internal EnumReferenceIdentity(IEnumReferenceIdentity e)
    {
      this._enum = e;
    }

    private ReferenceIdentity GetCurrent()
    {
      if (this._current == null)
        throw new InvalidOperationException();
      return new ReferenceIdentity(this._current);
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
      this._current = (IReferenceIdentity) null;
      return false;
    }

    public void Reset()
    {
      this._current = (IReferenceIdentity) null;
      this._enum.Reset();
    }
  }
}
