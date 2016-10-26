// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.CodeSigning.CmiManifestSigner
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace System.Deployment.Internal.CodeSigning
{
  internal class CmiManifestSigner
  {
    private AsymmetricAlgorithm m_strongNameKey;
    private X509Certificate2 m_certificate;
    private string m_description;
    private string m_url;
    private X509Certificate2Collection m_certificates;
    private X509IncludeOption m_includeOption;
    private CmiManifestSignerFlag m_signerFlag;
    internal const uint CimManifestSignerFlagMask = 1;

    internal AsymmetricAlgorithm StrongNameKey
    {
      get
      {
        return this.m_strongNameKey;
      }
    }

    internal X509Certificate2 Certificate
    {
      get
      {
        return this.m_certificate;
      }
    }

    internal string Description
    {
      get
      {
        return this.m_description;
      }
      set
      {
        this.m_description = value;
      }
    }

    internal string DescriptionUrl
    {
      get
      {
        return this.m_url;
      }
      set
      {
        this.m_url = value;
      }
    }

    internal X509Certificate2Collection ExtraStore
    {
      get
      {
        return this.m_certificates;
      }
    }

    internal X509IncludeOption IncludeOption
    {
      get
      {
        return this.m_includeOption;
      }
      set
      {
        if (value < X509IncludeOption.None || value > X509IncludeOption.WholeChain)
          throw new ArgumentException("value");
        if (this.m_includeOption == X509IncludeOption.None)
          throw new NotSupportedException();
        this.m_includeOption = value;
      }
    }

    internal CmiManifestSignerFlag Flag
    {
      get
      {
        return this.m_signerFlag;
      }
      set
      {
        if ((value & ~CmiManifestSignerFlag.DontReplacePublicKeyToken) != CmiManifestSignerFlag.None)
          throw new ArgumentException("value");
        this.m_signerFlag = value;
      }
    }

    private CmiManifestSigner()
    {
    }

    internal CmiManifestSigner(AsymmetricAlgorithm strongNameKey)
      : this(strongNameKey, (X509Certificate2) null)
    {
    }

    internal CmiManifestSigner(AsymmetricAlgorithm strongNameKey, X509Certificate2 certificate)
    {
      if (strongNameKey == null)
        throw new ArgumentNullException("strongNameKey");
      if (!(strongNameKey is RSA))
        throw new ArgumentNullException("strongNameKey");
      this.m_strongNameKey = strongNameKey;
      this.m_certificate = certificate;
      this.m_certificates = new X509Certificate2Collection();
      this.m_includeOption = X509IncludeOption.ExcludeRoot;
      this.m_signerFlag = CmiManifestSignerFlag.None;
    }
  }
}
