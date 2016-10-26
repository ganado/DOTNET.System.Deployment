// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.AssemblyModule
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

namespace System.Deployment.Application
{
  internal class AssemblyModule
  {
    private string _name;
    private byte[] _hash;

    public string Name
    {
      get
      {
        return this._name;
      }
    }

    public byte[] Hash
    {
      get
      {
        return this._hash;
      }
    }

    public AssemblyModule(string name, byte[] hash)
    {
      this._name = name;
      this._hash = hash;
    }
  }
}
