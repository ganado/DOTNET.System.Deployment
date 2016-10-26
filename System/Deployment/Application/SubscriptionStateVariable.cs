// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.SubscriptionStateVariable
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

namespace System.Deployment.Application
{
  internal class SubscriptionStateVariable
  {
    public string PropertyName;
    public object NewValue;
    public object OldValue;

    public bool IsUnchanged
    {
      get
      {
        if (this.NewValue == null)
          return this.OldValue == null;
        return this.NewValue.Equals(this.OldValue);
      }
    }

    public SubscriptionStateVariable(string name, object newValue, object oldValue)
    {
      this.PropertyName = name;
      this.NewValue = newValue;
      this.OldValue = oldValue;
    }
  }
}
