// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.CodeSigning.RSAPKCS1SHA256SignatureDescription
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Security.Cryptography;

namespace System.Deployment.Internal.CodeSigning
{
  /// <summary>Creates <see cref="T:System.Security.Cryptography.RSAPKCS1SignatureFormatter" /> and <see cref="T:System.Security.Cryptography.RSAPKCS1SignatureDeformatter" /> objects.</summary>
  public sealed class RSAPKCS1SHA256SignatureDescription : SignatureDescription
  {
    /// <summary>Initializes a new instance of the <see cref="T:System.Deployment.Internal.CodeSigning.RSAPKCS1SHA256SignatureDescription" /> class.</summary>
    public RSAPKCS1SHA256SignatureDescription()
    {
      this.KeyAlgorithm = typeof (RSACryptoServiceProvider).FullName;
      this.DigestAlgorithm = typeof (SHA256Cng).FullName;
      this.FormatterAlgorithm = typeof (RSAPKCS1SignatureFormatter).FullName;
      this.DeformatterAlgorithm = typeof (RSAPKCS1SignatureDeformatter).FullName;
    }

    /// <summary>Creates an asymmetric signature deformatter instance that has the specified key.</summary>
    /// <returns>An asymmetric signature deformatter object.</returns>
    /// <param name="key">The key to use in the deformatter. </param>
    public override AsymmetricSignatureDeformatter CreateDeformatter(AsymmetricAlgorithm key)
    {
      if (key == null)
        throw new ArgumentNullException("key");
      RSAPKCS1SignatureDeformatter signatureDeformatter = new RSAPKCS1SignatureDeformatter(key);
      signatureDeformatter.SetHashAlgorithm("SHA256");
      return (AsymmetricSignatureDeformatter) signatureDeformatter;
    }

    /// <summary>Creates an asymmetric signature formatter instance that has the specified key.</summary>
    /// <returns>An asymmetric signature formatter object.</returns>
    /// <param name="key">The key to use in the formatter. </param>
    public override AsymmetricSignatureFormatter CreateFormatter(AsymmetricAlgorithm key)
    {
      if (key == null)
        throw new ArgumentNullException("key");
      RSAPKCS1SignatureFormatter signatureFormatter = new RSAPKCS1SignatureFormatter(key);
      signatureFormatter.SetHashAlgorithm("SHA256");
      return (AsymmetricSignatureFormatter) signatureFormatter;
    }
  }
}
