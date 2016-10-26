// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.BindCompletedEventArgs
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.ComponentModel;

namespace System.Deployment.Application
{
  internal class BindCompletedEventArgs : AsyncCompletedEventArgs
  {
    private readonly ActivationContext _actCtx;
    private readonly string _name;
    private readonly bool _cached;

    public ActivationContext ActivationContext
    {
      get
      {
        this.RaiseExceptionIfNecessary();
        return this._actCtx;
      }
    }

    public string FriendlyName
    {
      get
      {
        this.RaiseExceptionIfNecessary();
        return this._name;
      }
    }

    public bool IsCached
    {
      get
      {
        this.RaiseExceptionIfNecessary();
        return this._cached;
      }
    }

    internal BindCompletedEventArgs(Exception error, bool cancelled, object userState, ActivationContext actCtx, string name, bool cached)
      : base(error, cancelled, userState)
    {
      this._actCtx = actCtx;
      this._name = name;
      this._cached = cached;
    }
  }
}
