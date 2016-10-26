// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.ReferenceIdentity
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

namespace System.Deployment.Internal.Isolation
{
  internal sealed class ReferenceIdentity
  {
    internal IReferenceIdentity _id;

    internal ReferenceIdentity(IReferenceIdentity i)
    {
      if (i == null)
        throw new ArgumentNullException();
      this._id = i;
    }

    private string GetAttribute(string ns, string n)
    {
      return this._id.GetAttribute(ns, n);
    }

    private string GetAttribute(string n)
    {
      return this._id.GetAttribute((string) null, n);
    }

    private void SetAttribute(string ns, string n, string v)
    {
      this._id.SetAttribute(ns, n, v);
    }

    private void SetAttribute(string n, string v)
    {
      this.SetAttribute((string) null, n, v);
    }

    private void DeleteAttribute(string ns, string n)
    {
      this.SetAttribute(ns, n, (string) null);
    }

    private void DeleteAttribute(string n)
    {
      this.SetAttribute((string) null, n, (string) null);
    }
  }
}
