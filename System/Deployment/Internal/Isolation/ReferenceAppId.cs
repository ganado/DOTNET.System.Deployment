// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.ReferenceAppId
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

namespace System.Deployment.Internal.Isolation
{
  internal sealed class ReferenceAppId
  {
    internal IReferenceAppId _id;

    public string SubscriptionId
    {
      get
      {
        return this._id.get_SubscriptionId();
      }
      set
      {
        this._id.put_SubscriptionId(value);
      }
    }

    public string Codebase
    {
      get
      {
        return this._id.get_Codebase();
      }
      set
      {
        this._id.put_Codebase(value);
      }
    }

    public EnumReferenceIdentity AppPath
    {
      get
      {
        return new EnumReferenceIdentity(this._id.EnumAppPath());
      }
    }

    internal ReferenceAppId(IReferenceAppId id)
    {
      if (id == null)
        throw new ArgumentNullException();
      this._id = id;
    }
  }
}
