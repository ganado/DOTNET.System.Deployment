// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.CodeSigning.ManifestSignedXml
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Security.Cryptography.Xml;
using System.Xml;

namespace System.Deployment.Internal.CodeSigning
{
  internal class ManifestSignedXml : SignedXml
  {
    private bool m_verify;

    internal ManifestSignedXml()
    {
    }

    internal ManifestSignedXml(XmlElement elem)
      : base(elem)
    {
    }

    internal ManifestSignedXml(XmlDocument document)
      : base(document)
    {
    }

    internal ManifestSignedXml(XmlDocument document, bool verify)
      : base(document)
    {
      this.m_verify = verify;
    }

    private static XmlElement FindIdElement(XmlElement context, string idValue)
    {
      if (context == null)
        return (XmlElement) null;
      return context.SelectSingleNode("//*[@Id=\"" + idValue + "\"]") as XmlElement ?? context.SelectSingleNode("//*[@id=\"" + idValue + "\"]") as XmlElement ?? context.SelectSingleNode("//*[@ID=\"" + idValue + "\"]") as XmlElement;
    }

    public override XmlElement GetIdElement(XmlDocument document, string idValue)
    {
      if (this.m_verify)
        return base.GetIdElement(document, idValue);
      KeyInfo keyInfo = this.KeyInfo;
      if (keyInfo.Id != idValue)
        return (XmlElement) null;
      return keyInfo.GetXml();
    }
  }
}
