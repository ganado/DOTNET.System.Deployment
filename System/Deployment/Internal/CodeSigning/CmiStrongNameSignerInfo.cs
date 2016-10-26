// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.CodeSigning.CmiStrongNameSignerInfo
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Security.Cryptography;

namespace System.Deployment.Internal.CodeSigning
{
  internal class CmiStrongNameSignerInfo
  {
    private int m_error;
    private string m_publicKeyToken;
    private AsymmetricAlgorithm m_snKey;

    internal int ErrorCode
    {
      get
      {
        return this.m_error;
      }
      set
      {
        this.m_error = value;
      }
    }

    internal string PublicKeyToken
    {
      get
      {
        return this.m_publicKeyToken;
      }
      set
      {
        this.m_publicKeyToken = value;
      }
    }

    internal AsymmetricAlgorithm PublicKey
    {
      get
      {
        return this.m_snKey;
      }
      set
      {
        this.m_snKey = value;
      }
    }

    internal CmiStrongNameSignerInfo()
    {
    }

    internal CmiStrongNameSignerInfo(int errorCode, string publicKeyToken)
    {
      this.m_error = errorCode;
      this.m_publicKeyToken = publicKeyToken;
    }
  }
}
