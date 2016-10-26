// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.ServerInformation
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

namespace System.Deployment.Application
{
  internal class ServerInformation
  {
    private string _server;
    private string _poweredBy;
    private string _aspNetVersion;

    public string Server
    {
      get
      {
        return this._server;
      }
      set
      {
        this._server = value;
      }
    }

    public string PoweredBy
    {
      get
      {
        return this._poweredBy;
      }
      set
      {
        this._poweredBy = value;
      }
    }

    public string AspNetVersion
    {
      get
      {
        return this._aspNetVersion;
      }
      set
      {
        this._aspNetVersion = value;
      }
    }
  }
}
