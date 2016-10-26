// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.CodeSigning.SignedCmiManifest
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

namespace System.Deployment.Internal.CodeSigning
{
  internal class SignedCmiManifest
  {
    private static readonly char[] hexValues = new char[16]
    {
      '0',
      '1',
      '2',
      '3',
      '4',
      '5',
      '6',
      '7',
      '8',
      '9',
      'a',
      'b',
      'c',
      'd',
      'e',
      'f'
    };
    private XmlDocument m_manifestDom;
    private CmiStrongNameSignerInfo m_strongNameSignerInfo;
    private CmiAuthenticodeSignerInfo m_authenticodeSignerInfo;
    private const string AssemblyNamespaceUri = "urn:schemas-microsoft-com:asm.v1";
    private const string AssemblyV2NamespaceUri = "urn:schemas-microsoft-com:asm.v2";
    private const string MSRelNamespaceUri = "http://schemas.microsoft.com/windows/rel/2005/reldata";
    private const string LicenseNamespaceUri = "urn:mpeg:mpeg21:2003:01-REL-R-NS";
    private const string AuthenticodeNamespaceUri = "http://schemas.microsoft.com/windows/pki/2005/Authenticode";
    private const string licenseTemplate = "<r:license xmlns:r=\"urn:mpeg:mpeg21:2003:01-REL-R-NS\" xmlns:as=\"http://schemas.microsoft.com/windows/pki/2005/Authenticode\"><r:grant><as:ManifestInformation><as:assemblyIdentity /></as:ManifestInformation><as:SignedBy/><as:AuthenticodePublisher><as:X509SubjectName>CN=dummy</as:X509SubjectName></as:AuthenticodePublisher></r:grant><r:issuer></r:issuer></r:license>";

    internal CmiStrongNameSignerInfo StrongNameSignerInfo
    {
      get
      {
        return this.m_strongNameSignerInfo;
      }
    }

    internal CmiAuthenticodeSignerInfo AuthenticodeSignerInfo
    {
      get
      {
        return this.m_authenticodeSignerInfo;
      }
    }

    private SignedCmiManifest()
    {
    }

    internal SignedCmiManifest(XmlDocument manifestDom)
    {
      if (manifestDom == null)
        throw new ArgumentNullException("manifestDom");
      this.m_manifestDom = manifestDom;
    }

    internal void Sign(CmiManifestSigner signer)
    {
      this.Sign(signer, (string) null);
    }

    internal void Sign(CmiManifestSigner signer, string timeStampUrl)
    {
      this.m_strongNameSignerInfo = (CmiStrongNameSignerInfo) null;
      this.m_authenticodeSignerInfo = (CmiAuthenticodeSignerInfo) null;
      if (signer == null || signer.StrongNameKey == null)
        throw new ArgumentNullException("signer");
      SignedCmiManifest.RemoveExistingSignature(this.m_manifestDom);
      if ((signer.Flag & CmiManifestSignerFlag.DontReplacePublicKeyToken) == CmiManifestSignerFlag.None)
        SignedCmiManifest.ReplacePublicKeyToken(this.m_manifestDom, signer.StrongNameKey);
      XmlDocument licenseDom = (XmlDocument) null;
      if (signer.Certificate != null)
      {
        SignedCmiManifest.InsertPublisherIdentity(this.m_manifestDom, signer.Certificate);
        licenseDom = SignedCmiManifest.CreateLicenseDom(signer, this.ExtractPrincipalFromManifest(), SignedCmiManifest.ComputeHashFromManifest(this.m_manifestDom));
        SignedCmiManifest.AuthenticodeSignLicenseDom(licenseDom, signer, timeStampUrl);
      }
      SignedCmiManifest.StrongNameSignManifestDom(this.m_manifestDom, licenseDom, signer);
    }

    internal void Verify(CmiManifestVerifyFlags verifyFlags)
    {
      this.m_strongNameSignerInfo = (CmiStrongNameSignerInfo) null;
      this.m_authenticodeSignerInfo = (CmiAuthenticodeSignerInfo) null;
      XmlNamespaceManager nsmgr = new XmlNamespaceManager(this.m_manifestDom.NameTable);
      nsmgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
      XmlElement xmlElement1 = this.m_manifestDom.SelectSingleNode("//ds:Signature", nsmgr) as XmlElement;
      if (xmlElement1 == null)
        throw new CryptographicException(-2146762496);
      string name = "Id";
      if (!xmlElement1.HasAttribute(name))
      {
        name = "id";
        if (!xmlElement1.HasAttribute(name))
        {
          name = "ID";
          if (!xmlElement1.HasAttribute(name))
            throw new CryptographicException(-2146762749);
        }
      }
      string attribute1 = xmlElement1.GetAttribute(name);
      if (attribute1 == null || string.Compare(attribute1, "StrongNameSignature", StringComparison.Ordinal) != 0)
        throw new CryptographicException(-2146762749);
      bool oldFormat = false;
      bool flag1 = false;
      foreach (XmlNode selectNode in xmlElement1.SelectNodes("ds:SignedInfo/ds:Reference", nsmgr))
      {
        XmlElement xmlElement2 = selectNode as XmlElement;
        if (xmlElement2 != null && xmlElement2.HasAttribute("URI"))
        {
          string attribute2 = xmlElement2.GetAttribute("URI");
          if (attribute2 != null)
          {
            if (attribute2.Length == 0)
            {
              XmlNode xmlNode = xmlElement2.SelectSingleNode("ds:Transforms", nsmgr);
              if (xmlNode == null)
                throw new CryptographicException(-2146762749);
              XmlNodeList xmlNodeList = xmlNode.SelectNodes("ds:Transform", nsmgr);
              if (xmlNodeList.Count < 2)
                throw new CryptographicException(-2146762749);
              bool flag2 = false;
              bool flag3 = false;
              for (int index = 0; index < xmlNodeList.Count; ++index)
              {
                string attribute3 = (xmlNodeList[index] as XmlElement).GetAttribute("Algorithm");
                if (attribute3 != null)
                {
                  if (string.Compare(attribute3, "http://www.w3.org/2001/10/xml-exc-c14n#", StringComparison.Ordinal) != 0)
                  {
                    flag2 = true;
                    if (flag3)
                    {
                      flag1 = true;
                      break;
                    }
                  }
                  else if (string.Compare(attribute3, "http://www.w3.org/2000/09/xmldsig#enveloped-signature", StringComparison.Ordinal) != 0)
                  {
                    flag3 = true;
                    if (flag2)
                    {
                      flag1 = true;
                      break;
                    }
                  }
                }
                else
                  break;
              }
            }
            else if (string.Compare(attribute2, "#StrongNameKeyInfo", StringComparison.Ordinal) == 0)
            {
              oldFormat = true;
              XmlNode xmlNode = selectNode.SelectSingleNode("ds:Transforms", nsmgr);
              if (xmlNode == null)
                throw new CryptographicException(-2146762749);
              XmlNodeList xmlNodeList = xmlNode.SelectNodes("ds:Transform", nsmgr);
              if (xmlNodeList.Count < 1)
                throw new CryptographicException(-2146762749);
              for (int index = 0; index < xmlNodeList.Count; ++index)
              {
                string attribute3 = (xmlNodeList[index] as XmlElement).GetAttribute("Algorithm");
                if (attribute3 != null)
                {
                  if (string.Compare(attribute3, "http://www.w3.org/2001/10/xml-exc-c14n#", StringComparison.Ordinal) != 0)
                  {
                    flag1 = true;
                    break;
                  }
                }
                else
                  break;
              }
            }
          }
        }
      }
      if (!flag1)
        throw new CryptographicException(-2146762749);
      this.m_strongNameSignerInfo = new CmiStrongNameSignerInfo(-2146762485, this.VerifyPublicKeyToken());
      ManifestSignedXml manifestSignedXml = new ManifestSignedXml(this.m_manifestDom, true);
      manifestSignedXml.LoadXml(xmlElement1);
      AsymmetricAlgorithm signingKey = (AsymmetricAlgorithm) null;
      bool flag4 = manifestSignedXml.CheckSignatureReturningKey(out signingKey);
      this.m_strongNameSignerInfo.PublicKey = signingKey;
      if (!flag4)
      {
        this.m_strongNameSignerInfo.ErrorCode = -2146869232;
        throw new CryptographicException(-2146869232);
      }
      if ((verifyFlags & CmiManifestVerifyFlags.StrongNameOnly) == CmiManifestVerifyFlags.StrongNameOnly)
        return;
      this.VerifyLicense(verifyFlags, oldFormat);
    }

    private unsafe void VerifyLicense(CmiManifestVerifyFlags verifyFlags, bool oldFormat)
    {
      XmlNamespaceManager namespaceManager = new XmlNamespaceManager(this.m_manifestDom.NameTable);
      namespaceManager.AddNamespace("asm", "urn:schemas-microsoft-com:asm.v1");
      namespaceManager.AddNamespace("asm2", "urn:schemas-microsoft-com:asm.v2");
      namespaceManager.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
      namespaceManager.AddNamespace("msrel", "http://schemas.microsoft.com/windows/rel/2005/reldata");
      namespaceManager.AddNamespace("r", "urn:mpeg:mpeg21:2003:01-REL-R-NS");
      namespaceManager.AddNamespace("as", "http://schemas.microsoft.com/windows/pki/2005/Authenticode");
      XmlElement xmlElement = this.m_manifestDom.SelectSingleNode("asm:assembly/ds:Signature/ds:KeyInfo/msrel:RelData/r:license", namespaceManager) as XmlElement;
      if (xmlElement == null)
        return;
      this.VerifyAssemblyIdentity(namespaceManager);
      this.m_authenticodeSignerInfo = new CmiAuthenticodeSignerInfo(-2146762485);
      byte[] bytes = Encoding.UTF8.GetBytes(xmlElement.OuterXml);
      fixed (byte* numPtr = bytes)
      {
        Win32.AXL_SIGNER_INFO pSignerInfo = new Win32.AXL_SIGNER_INFO();
        pSignerInfo.cbSize = (uint) Marshal.SizeOf(typeof (Win32.AXL_SIGNER_INFO));
        Win32.AXL_TIMESTAMPER_INFO pTimestamperInfo = new Win32.AXL_TIMESTAMPER_INFO();
        pTimestamperInfo.cbSize = (uint) Marshal.SizeOf(typeof (Win32.AXL_TIMESTAMPER_INFO));
        Win32.CRYPT_DATA_BLOB pLicenseBlob = new Win32.CRYPT_DATA_BLOB();
        IntPtr num = new IntPtr((void*) numPtr);
        pLicenseBlob.cbData = (uint) bytes.Length;
        pLicenseBlob.pbData = num;
        int hr = Win32.CertVerifyAuthenticodeLicense(ref pLicenseBlob, (uint) verifyFlags, out pSignerInfo, out pTimestamperInfo);
        if (-2146762496 != (int) pSignerInfo.dwError)
          this.m_authenticodeSignerInfo = new CmiAuthenticodeSignerInfo(pSignerInfo, pTimestamperInfo);
        Win32.CertFreeAuthenticodeSignerInfo(ref pSignerInfo);
        Win32.CertFreeAuthenticodeTimestamperInfo(ref pTimestamperInfo);
        if (hr != 0)
          throw new CryptographicException(hr);
      }
      if (oldFormat)
        return;
      this.VerifyPublisherIdentity(namespaceManager);
    }

    private XmlElement ExtractPrincipalFromManifest()
    {
      XmlNamespaceManager nsmgr = new XmlNamespaceManager(this.m_manifestDom.NameTable);
      nsmgr.AddNamespace("asm", "urn:schemas-microsoft-com:asm.v1");
      XmlNode xmlNode = this.m_manifestDom.SelectSingleNode("asm:assembly/asm:assemblyIdentity", nsmgr);
      if (xmlNode == null)
        throw new CryptographicException(-2146762749);
      return xmlNode as XmlElement;
    }

    private void VerifyAssemblyIdentity(XmlNamespaceManager nsm)
    {
      XmlElement xmlElement1 = this.m_manifestDom.SelectSingleNode("asm:assembly/asm:assemblyIdentity", nsm) as XmlElement;
      XmlElement xmlElement2 = this.m_manifestDom.SelectSingleNode("asm:assembly/ds:Signature/ds:KeyInfo/msrel:RelData/r:license/r:grant/as:ManifestInformation/as:assemblyIdentity", nsm) as XmlElement;
      if (xmlElement1 == null || xmlElement2 == null || (!xmlElement1.HasAttributes || !xmlElement2.HasAttributes))
        throw new CryptographicException(-2146762749);
      XmlAttributeCollection attributes = xmlElement1.Attributes;
      if (attributes.Count == 0 || attributes.Count != xmlElement2.Attributes.Count)
        throw new CryptographicException(-2146762749);
      foreach (XmlAttribute xmlAttribute in (XmlNamedNodeMap) attributes)
      {
        if (!xmlElement2.HasAttribute(xmlAttribute.LocalName) || xmlAttribute.Value != xmlElement2.GetAttribute(xmlAttribute.LocalName))
          throw new CryptographicException(-2146762749);
      }
      this.VerifyHash(nsm);
    }

    private void VerifyPublisherIdentity(XmlNamespaceManager nsm)
    {
      if (this.m_authenticodeSignerInfo.ErrorCode == -2146762496)
        return;
      X509Certificate2 certificate = this.m_authenticodeSignerInfo.SignerChain.ChainElements[0].Certificate;
      XmlElement xmlElement = this.m_manifestDom.SelectSingleNode("asm:assembly/asm2:publisherIdentity", nsm) as XmlElement;
      if (xmlElement == null || !xmlElement.HasAttributes)
        throw new CryptographicException(-2146762749);
      if (!xmlElement.HasAttribute("name") || !xmlElement.HasAttribute("issuerKeyHash"))
        throw new CryptographicException(-2146762749);
      string attribute1 = xmlElement.GetAttribute("name");
      string attribute2 = xmlElement.GetAttribute("issuerKeyHash");
      IntPtr ppwszPublicKeyHash = new IntPtr();
      int issuerPublicKeyHash = Win32._AxlGetIssuerPublicKeyHash(certificate.Handle, out ppwszPublicKeyHash);
      if (issuerPublicKeyHash != 0)
        throw new CryptographicException(issuerPublicKeyHash);
      string stringUni = Marshal.PtrToStringUni(ppwszPublicKeyHash);
      Win32.HeapFree(Win32.GetProcessHeap(), 0U, ppwszPublicKeyHash);
      if (string.Compare(attribute1, certificate.SubjectName.Name, StringComparison.Ordinal) != 0 || string.Compare(attribute2, stringUni, StringComparison.Ordinal) != 0)
        throw new CryptographicException(-2146762485);
    }

    private void VerifyHash(XmlNamespaceManager nsm)
    {
      new XmlDocument().PreserveWhitespace = true;
      XmlDocument manifestDom = (XmlDocument) this.m_manifestDom.Clone();
      XmlElement xmlElement1 = manifestDom.SelectSingleNode("asm:assembly/ds:Signature/ds:KeyInfo/msrel:RelData/r:license/r:grant/as:ManifestInformation", nsm) as XmlElement;
      if (xmlElement1 == null)
        throw new CryptographicException(-2146762749);
      if (!xmlElement1.HasAttribute("Hash"))
        throw new CryptographicException(-2146762749);
      string attribute = xmlElement1.GetAttribute("Hash");
      if (attribute == null || attribute.Length == 0)
        throw new CryptographicException(-2146762749);
      XmlElement xmlElement2 = manifestDom.SelectSingleNode("asm:assembly/ds:Signature", nsm) as XmlElement;
      if (xmlElement2 == null)
        throw new CryptographicException(-2146762749);
      xmlElement2.ParentNode.RemoveChild((XmlNode) xmlElement2);
      byte[] bytes = SignedCmiManifest.HexStringToBytes(xmlElement1.GetAttribute("Hash"));
      byte[] hashFromManifest1 = SignedCmiManifest.ComputeHashFromManifest(manifestDom);
      if (bytes.Length == 0 || bytes.Length != hashFromManifest1.Length)
      {
        byte[] hashFromManifest2 = SignedCmiManifest.ComputeHashFromManifest(manifestDom, true);
        if (bytes.Length == 0 || bytes.Length != hashFromManifest2.Length)
          throw new CryptographicException(-2146869232);
        for (int index = 0; index < bytes.Length; ++index)
        {
          if ((int) bytes[index] != (int) hashFromManifest2[index])
            throw new CryptographicException(-2146869232);
        }
      }
      for (int index = 0; index < bytes.Length; ++index)
      {
        if ((int) bytes[index] != (int) hashFromManifest1[index])
        {
          byte[] hashFromManifest2 = SignedCmiManifest.ComputeHashFromManifest(manifestDom, true);
          if (bytes.Length == 0 || bytes.Length != hashFromManifest2.Length)
            throw new CryptographicException(-2146869232);
          for (index = 0; index < bytes.Length; ++index)
          {
            if ((int) bytes[index] != (int) hashFromManifest2[index])
              throw new CryptographicException(-2146869232);
          }
        }
      }
    }

    private unsafe string VerifyPublicKeyToken()
    {
      XmlNamespaceManager nsmgr = new XmlNamespaceManager(this.m_manifestDom.NameTable);
      nsmgr.AddNamespace("asm", "urn:schemas-microsoft-com:asm.v1");
      nsmgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
      XmlElement xmlElement1 = this.m_manifestDom.SelectSingleNode("asm:assembly/ds:Signature/ds:KeyInfo/ds:KeyValue/ds:RSAKeyValue/ds:Modulus", nsmgr) as XmlElement;
      XmlElement xmlElement2 = this.m_manifestDom.SelectSingleNode("asm:assembly/ds:Signature/ds:KeyInfo/ds:KeyValue/ds:RSAKeyValue/ds:Exponent", nsmgr) as XmlElement;
      if (xmlElement1 == null || xmlElement2 == null)
        throw new CryptographicException(-2146762749);
      byte[] bytes1 = Encoding.UTF8.GetBytes(xmlElement1.InnerXml);
      byte[] bytes2 = Encoding.UTF8.GetBytes(xmlElement2.InnerXml);
      string publicKeyToken1 = SignedCmiManifest.GetPublicKeyToken(this.m_manifestDom);
      byte[] bytes3 = SignedCmiManifest.HexStringToBytes(publicKeyToken1);
      byte[] bytes4;
      fixed (byte* numPtr1 = bytes1)
        fixed (byte* numPtr2 = bytes2)
        {
          Win32.CRYPT_DATA_BLOB pModulusBlob = new Win32.CRYPT_DATA_BLOB();
          Win32.CRYPT_DATA_BLOB pExponentBlob = new Win32.CRYPT_DATA_BLOB();
          IntPtr ppwszPublicKeyToken = new IntPtr();
          pModulusBlob.cbData = (uint) bytes1.Length;
          pModulusBlob.pbData = new IntPtr((void*) numPtr1);
          pExponentBlob.cbData = (uint) bytes2.Length;
          pExponentBlob.pbData = new IntPtr((void*) numPtr2);
          int publicKeyToken2 = Win32._AxlRSAKeyValueToPublicKeyToken(ref pModulusBlob, ref pExponentBlob, out ppwszPublicKeyToken);
          if (publicKeyToken2 != 0)
            throw new CryptographicException(publicKeyToken2);
          bytes4 = SignedCmiManifest.HexStringToBytes(Marshal.PtrToStringUni(ppwszPublicKeyToken));
          Win32.HeapFree(Win32.GetProcessHeap(), 0U, ppwszPublicKeyToken);
        }
      if (bytes3.Length == 0 || bytes3.Length != bytes4.Length)
        throw new CryptographicException(-2146762485);
      for (int index = 0; index < bytes3.Length; ++index)
      {
        if ((int) bytes3[index] != (int) bytes4[index])
          throw new CryptographicException(-2146762485);
      }
      return publicKeyToken1;
    }

    private static void InsertPublisherIdentity(XmlDocument manifestDom, X509Certificate2 signerCert)
    {
      XmlNamespaceManager nsmgr = new XmlNamespaceManager(manifestDom.NameTable);
      nsmgr.AddNamespace("asm", "urn:schemas-microsoft-com:asm.v1");
      nsmgr.AddNamespace("asm2", "urn:schemas-microsoft-com:asm.v2");
      nsmgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
      XmlElement xmlElement1 = manifestDom.SelectSingleNode("asm:assembly", nsmgr) as XmlElement;
      if (!(manifestDom.SelectSingleNode("asm:assembly/asm:assemblyIdentity", nsmgr) is XmlElement))
        throw new CryptographicException(-2146762749);
      XmlElement xmlElement2 = manifestDom.SelectSingleNode("asm:assembly/asm2:publisherIdentity", nsmgr) as XmlElement ?? manifestDom.CreateElement("publisherIdentity", "urn:schemas-microsoft-com:asm.v2");
      IntPtr ppwszPublicKeyHash = new IntPtr();
      int issuerPublicKeyHash = Win32._AxlGetIssuerPublicKeyHash(signerCert.Handle, out ppwszPublicKeyHash);
      if (issuerPublicKeyHash != 0)
        throw new CryptographicException(issuerPublicKeyHash);
      string stringUni = Marshal.PtrToStringUni(ppwszPublicKeyHash);
      Win32.HeapFree(Win32.GetProcessHeap(), 0U, ppwszPublicKeyHash);
      xmlElement2.SetAttribute("name", signerCert.SubjectName.Name);
      xmlElement2.SetAttribute("issuerKeyHash", stringUni);
      XmlElement xmlElement3 = manifestDom.SelectSingleNode("asm:assembly/ds:Signature", nsmgr) as XmlElement;
      if (xmlElement3 != null)
        xmlElement1.InsertBefore((XmlNode) xmlElement2, (XmlNode) xmlElement3);
      else
        xmlElement1.AppendChild((XmlNode) xmlElement2);
    }

    private static void RemoveExistingSignature(XmlDocument manifestDom)
    {
      XmlNamespaceManager nsmgr = new XmlNamespaceManager(manifestDom.NameTable);
      nsmgr.AddNamespace("asm", "urn:schemas-microsoft-com:asm.v1");
      nsmgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
      XmlNode oldChild = manifestDom.SelectSingleNode("asm:assembly/ds:Signature", nsmgr);
      if (oldChild == null)
        return;
      oldChild.ParentNode.RemoveChild(oldChild);
    }

    private static unsafe void ReplacePublicKeyToken(XmlDocument manifestDom, AsymmetricAlgorithm snKey)
    {
      XmlNamespaceManager nsmgr = new XmlNamespaceManager(manifestDom.NameTable);
      nsmgr.AddNamespace("asm", "urn:schemas-microsoft-com:asm.v1");
      XmlElement xmlElement = manifestDom.SelectSingleNode("asm:assembly/asm:assemblyIdentity", nsmgr) as XmlElement;
      if (xmlElement == null)
        throw new CryptographicException(-2146762749);
      if (!xmlElement.HasAttribute("publicKeyToken"))
        throw new CryptographicException(-2146762749);
      byte[] numArray = ((RSACryptoServiceProvider) snKey).ExportCspBlob(false);
      if (numArray == null || numArray.Length == 0)
        throw new CryptographicException(-2146893821);
      fixed (byte* numPtr = numArray)
      {
        Win32.CRYPT_DATA_BLOB pCspPublicKeyBlob = new Win32.CRYPT_DATA_BLOB();
        pCspPublicKeyBlob.cbData = (uint) numArray.Length;
        pCspPublicKeyBlob.pbData = new IntPtr((void*) numPtr);
        IntPtr ppwszPublicKeyToken = new IntPtr();
        int publicKeyToken = Win32._AxlPublicKeyBlobToPublicKeyToken(ref pCspPublicKeyBlob, out ppwszPublicKeyToken);
        if (publicKeyToken != 0)
          throw new CryptographicException(publicKeyToken);
        string stringUni = Marshal.PtrToStringUni(ppwszPublicKeyToken);
        Win32.HeapFree(Win32.GetProcessHeap(), 0U, ppwszPublicKeyToken);
        xmlElement.SetAttribute("publicKeyToken", stringUni);
      }
    }

    private static string GetPublicKeyToken(XmlDocument manifestDom)
    {
      XmlNamespaceManager nsmgr = new XmlNamespaceManager(manifestDom.NameTable);
      nsmgr.AddNamespace("asm", "urn:schemas-microsoft-com:asm.v1");
      nsmgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
      XmlElement xmlElement = manifestDom.SelectSingleNode("asm:assembly/asm:assemblyIdentity", nsmgr) as XmlElement;
      if (xmlElement == null || !xmlElement.HasAttribute("publicKeyToken"))
        throw new CryptographicException(-2146762749);
      return xmlElement.GetAttribute("publicKeyToken");
    }

    private static byte[] ComputeHashFromManifest(XmlDocument manifestDom)
    {
      return SignedCmiManifest.ComputeHashFromManifest(manifestDom, false);
    }

    private static byte[] ComputeHashFromManifest(XmlDocument manifestDom, bool oldFormat)
    {
      if (oldFormat)
      {
        XmlDsigExcC14NTransform excC14Ntransform = new XmlDsigExcC14NTransform();
        excC14Ntransform.LoadInput((object) manifestDom);
        using (SHA1CryptoServiceProvider cryptoServiceProvider = new SHA1CryptoServiceProvider())
        {
          byte[] hash = cryptoServiceProvider.ComputeHash((Stream) (excC14Ntransform.GetOutput() as MemoryStream));
          if (hash == null)
            throw new CryptographicException(-2146869232);
          return hash;
        }
      }
      else
      {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.PreserveWhitespace = true;
        using (TextReader input = (TextReader) new StringReader(manifestDom.OuterXml))
        {
          XmlReader reader = XmlReader.Create(input, new XmlReaderSettings()
          {
            DtdProcessing = DtdProcessing.Parse
          }, manifestDom.BaseURI);
          xmlDocument.Load(reader);
        }
        XmlDsigExcC14NTransform excC14Ntransform = new XmlDsigExcC14NTransform();
        excC14Ntransform.LoadInput((object) xmlDocument);
        using (SHA1CryptoServiceProvider cryptoServiceProvider = new SHA1CryptoServiceProvider())
        {
          byte[] hash = cryptoServiceProvider.ComputeHash((Stream) (excC14Ntransform.GetOutput() as MemoryStream));
          if (hash == null)
            throw new CryptographicException(-2146869232);
          return hash;
        }
      }
    }

    private static XmlDocument CreateLicenseDom(CmiManifestSigner signer, XmlElement principal, byte[] hash)
    {
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.PreserveWhitespace = true;
      xmlDocument.LoadXml("<r:license xmlns:r=\"urn:mpeg:mpeg21:2003:01-REL-R-NS\" xmlns:as=\"http://schemas.microsoft.com/windows/pki/2005/Authenticode\"><r:grant><as:ManifestInformation><as:assemblyIdentity /></as:ManifestInformation><as:SignedBy/><as:AuthenticodePublisher><as:X509SubjectName>CN=dummy</as:X509SubjectName></as:AuthenticodePublisher></r:grant><r:issuer></r:issuer></r:license>");
      XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
      nsmgr.AddNamespace("r", "urn:mpeg:mpeg21:2003:01-REL-R-NS");
      nsmgr.AddNamespace("as", "http://schemas.microsoft.com/windows/pki/2005/Authenticode");
      XmlElement xmlElement1 = xmlDocument.SelectSingleNode("r:license/r:grant/as:ManifestInformation/as:assemblyIdentity", nsmgr) as XmlElement;
      xmlElement1.RemoveAllAttributes();
      foreach (XmlAttribute attribute in (XmlNamedNodeMap) principal.Attributes)
        xmlElement1.SetAttribute(attribute.Name, attribute.Value);
      XmlElement xmlElement2 = xmlDocument.SelectSingleNode("r:license/r:grant/as:ManifestInformation", nsmgr) as XmlElement;
      xmlElement2.SetAttribute("Hash", hash.Length == 0 ? "" : SignedCmiManifest.BytesToHexString(hash, 0, hash.Length));
      xmlElement2.SetAttribute("Description", signer.Description == null ? "" : signer.Description);
      xmlElement2.SetAttribute("Url", signer.DescriptionUrl == null ? "" : signer.DescriptionUrl);
      (xmlDocument.SelectSingleNode("r:license/r:grant/as:AuthenticodePublisher/as:X509SubjectName", nsmgr) as XmlElement).InnerText = signer.Certificate.SubjectName.Name;
      return xmlDocument;
    }

    private static void AuthenticodeSignLicenseDom(XmlDocument licenseDom, CmiManifestSigner signer, string timeStampUrl)
    {
      if (signer.Certificate.PublicKey.Key.GetType() != typeof (RSACryptoServiceProvider))
        throw new NotSupportedException();
      ManifestSignedXml manifestSignedXml = new ManifestSignedXml(licenseDom);
      manifestSignedXml.SigningKey = signer.Certificate.PrivateKey;
      manifestSignedXml.SignedInfo.CanonicalizationMethod = "http://www.w3.org/2001/10/xml-exc-c14n#";
      manifestSignedXml.KeyInfo.AddClause((KeyInfoClause) new RSAKeyValue(signer.Certificate.PublicKey.Key as RSA));
      manifestSignedXml.KeyInfo.AddClause((KeyInfoClause) new KeyInfoX509Data((X509Certificate) signer.Certificate, signer.IncludeOption));
      Reference reference = new Reference();
      reference.Uri = "";
      reference.AddTransform((Transform) new XmlDsigEnvelopedSignatureTransform());
      reference.AddTransform((Transform) new XmlDsigExcC14NTransform());
      manifestSignedXml.AddReference(reference);
      manifestSignedXml.ComputeSignature();
      XmlElement xml = manifestSignedXml.GetXml();
      xml.SetAttribute("Id", "AuthenticodeSignature");
      XmlNamespaceManager nsmgr = new XmlNamespaceManager(licenseDom.NameTable);
      nsmgr.AddNamespace("r", "urn:mpeg:mpeg21:2003:01-REL-R-NS");
      (licenseDom.SelectSingleNode("r:license/r:issuer", nsmgr) as XmlElement).AppendChild(licenseDom.ImportNode((XmlNode) xml, true));
      if (timeStampUrl != null && timeStampUrl.Length != 0)
        SignedCmiManifest.TimestampSignedLicenseDom(licenseDom, timeStampUrl);
      licenseDom.DocumentElement.ParentNode.InnerXml = "<msrel:RelData xmlns:msrel=\"http://schemas.microsoft.com/windows/rel/2005/reldata\">" + licenseDom.OuterXml + "</msrel:RelData>";
    }

    private static unsafe void TimestampSignedLicenseDom(XmlDocument licenseDom, string timeStampUrl)
    {
      Win32.CRYPT_DATA_BLOB pTimestampSignatureBlob = new Win32.CRYPT_DATA_BLOB();
      XmlNamespaceManager nsmgr = new XmlNamespaceManager(licenseDom.NameTable);
      nsmgr.AddNamespace("r", "urn:mpeg:mpeg21:2003:01-REL-R-NS");
      nsmgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
      nsmgr.AddNamespace("as", "http://schemas.microsoft.com/windows/pki/2005/Authenticode");
      byte[] bytes = Encoding.UTF8.GetBytes(licenseDom.OuterXml);
      fixed (byte* numPtr = bytes)
      {
        Win32.CRYPT_DATA_BLOB pSignedLicenseBlob = new Win32.CRYPT_DATA_BLOB();
        IntPtr num = new IntPtr((void*) numPtr);
        pSignedLicenseBlob.cbData = (uint) bytes.Length;
        pSignedLicenseBlob.pbData = num;
        int hr = Win32.CertTimestampAuthenticodeLicense(ref pSignedLicenseBlob, timeStampUrl, out pTimestampSignatureBlob);
        if (hr != 0)
          throw new CryptographicException(hr);
      }
      byte[] numArray = new byte[(int) pTimestampSignatureBlob.cbData];
      Marshal.Copy(pTimestampSignatureBlob.pbData, numArray, 0, numArray.Length);
      Win32.HeapFree(Win32.GetProcessHeap(), 0U, pTimestampSignatureBlob.pbData);
      XmlElement element1 = licenseDom.CreateElement("as", "Timestamp", "http://schemas.microsoft.com/windows/pki/2005/Authenticode");
      element1.InnerText = Encoding.UTF8.GetString(numArray);
      XmlElement element2 = licenseDom.CreateElement("Object", "http://www.w3.org/2000/09/xmldsig#");
      element2.AppendChild((XmlNode) element1);
      (licenseDom.SelectSingleNode("r:license/r:issuer/ds:Signature", nsmgr) as XmlElement).AppendChild((XmlNode) element2);
    }

    private static void StrongNameSignManifestDom(XmlDocument manifestDom, XmlDocument licenseDom, CmiManifestSigner signer)
    {
      RSA strongNameKey = signer.StrongNameKey as RSA;
      if (strongNameKey == null)
        throw new NotSupportedException();
      XmlNamespaceManager nsmgr = new XmlNamespaceManager(manifestDom.NameTable);
      nsmgr.AddNamespace("asm", "urn:schemas-microsoft-com:asm.v1");
      XmlElement elem = manifestDom.SelectSingleNode("asm:assembly", nsmgr) as XmlElement;
      if (elem == null)
        throw new CryptographicException(-2146762749);
      ManifestSignedXml manifestSignedXml = new ManifestSignedXml(elem);
      manifestSignedXml.SigningKey = signer.StrongNameKey;
      manifestSignedXml.SignedInfo.CanonicalizationMethod = "http://www.w3.org/2001/10/xml-exc-c14n#";
      manifestSignedXml.KeyInfo.AddClause((KeyInfoClause) new RSAKeyValue(strongNameKey));
      if (licenseDom != null)
        manifestSignedXml.KeyInfo.AddClause((KeyInfoClause) new KeyInfoNode(licenseDom.DocumentElement));
      manifestSignedXml.KeyInfo.Id = "StrongNameKeyInfo";
      Reference reference = new Reference();
      reference.Uri = "";
      reference.AddTransform((Transform) new XmlDsigEnvelopedSignatureTransform());
      reference.AddTransform((Transform) new XmlDsigExcC14NTransform());
      manifestSignedXml.AddReference(reference);
      manifestSignedXml.ComputeSignature();
      XmlElement xml = manifestSignedXml.GetXml();
      xml.SetAttribute("Id", "StrongNameSignature");
      elem.AppendChild((XmlNode) xml);
    }

    private static string BytesToHexString(byte[] array, int start, int end)
    {
      string str = (string) null;
      if (array != null)
      {
        char[] chArray1 = new char[(end - start) * 2];
        int index1 = end;
        int num1 = 0;
        while (index1-- > start)
        {
          int index2 = ((int) array[index1] & 240) >> 4;
          char[] chArray2 = chArray1;
          int index3 = num1;
          int num2 = 1;
          int num3 = index3 + num2;
          int hexValue1 = (int) SignedCmiManifest.hexValues[index2];
          chArray2[index3] = (char) hexValue1;
          int index4 = (int) array[index1] & 15;
          char[] chArray3 = chArray1;
          int index5 = num3;
          int num4 = 1;
          num1 = index5 + num4;
          int hexValue2 = (int) SignedCmiManifest.hexValues[index4];
          chArray3[index5] = (char) hexValue2;
        }
        str = new string(chArray1);
      }
      return str;
    }

    private static byte[] HexStringToBytes(string hexString)
    {
      uint num = (uint) hexString.Length / 2U;
      byte[] numArray = new byte[(int) num];
      int index1 = hexString.Length - 2;
      for (int index2 = 0; (long) index2 < (long) num; ++index2)
      {
        numArray[index2] = (byte) ((uint) SignedCmiManifest.HexToByte(hexString[index1]) << 4 | (uint) SignedCmiManifest.HexToByte(hexString[index1 + 1]));
        index1 -= 2;
      }
      return numArray;
    }

    private static byte HexToByte(char val)
    {
      if ((int) val <= 57 && (int) val >= 48)
        return (byte) ((uint) val - 48U);
      if ((int) val >= 97 && (int) val <= 102)
        return (byte) ((int) val - 97 + 10);
      if ((int) val >= 65 && (int) val <= 70)
        return (byte) ((int) val - 65 + 10);
      return byte.MaxValue;
    }
  }
}
