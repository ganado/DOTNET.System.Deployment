// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.ManifestGenerator
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Collections;
using System.Deployment.Application.Manifest;
using System.Deployment.Internal.Isolation;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml;

namespace System.Deployment.Application
{
  internal static class ManifestGenerator
  {
    private const string AssemblyTemplateResource = "AssemblyTemplate.xml";
    private static object assemblyTemplateDoc;
    private static object GACDetectionTempManifestAsmId;

    public static DefinitionIdentity GenerateManifest(ReferenceIdentity suggestedReferenceIdentity, AssemblyManifest manifest, string outputManifest)
    {
      DefinitionIdentity asmId = manifest.Identity;
      if (manifest.RawXmlBytes != null)
      {
        using (FileStream fileStream = System.IO.File.Open(outputManifest, FileMode.CreateNew, FileAccess.Write))
          fileStream.Write(manifest.RawXmlBytes, 0, manifest.RawXmlBytes.Length);
      }
      else
      {
        XmlDocument document = ManifestGenerator.CloneAssemblyTemplate();
        asmId = new DefinitionIdentity(suggestedReferenceIdentity);
        ManifestGenerator.InjectIdentityXml(document, asmId);
        ManifestGenerator.AddFiles(document, manifest.Files);
        ManifestGenerator.AddDependencies(document, manifest.DependentAssemblies);
        using (FileStream fileStream = System.IO.File.Open(outputManifest, FileMode.CreateNew, FileAccess.Write))
          document.Save((Stream) fileStream);
      }
      return asmId;
    }

    public static void GenerateGACDetectionManifest(ReferenceIdentity refId, string outputManifest)
    {
      XmlDocument document = ManifestGenerator.CloneAssemblyTemplate();
      if (ManifestGenerator.GACDetectionTempManifestAsmId == null)
        Interlocked.CompareExchange(ref ManifestGenerator.GACDetectionTempManifestAsmId, (object) new DefinitionIdentity("GACDetectionTempManifest, version=1.0.0.0, type=win32"), (object) null);
      ManifestGenerator.InjectIdentityXml(document, (DefinitionIdentity) ManifestGenerator.GACDetectionTempManifestAsmId);
      ManifestGenerator.AddDependencies(document, new DependentAssembly[1]
      {
        new DependentAssembly(refId)
      });
      using (FileStream fileStream = System.IO.File.Open(outputManifest, FileMode.CreateNew, FileAccess.Write))
        document.Save((Stream) fileStream);
    }

    private static void AddFiles(XmlDocument document, System.Deployment.Application.Manifest.File[] files)
    {
      XmlNamespaceManager namespaceMgr = ManifestGenerator.GetNamespaceMgr(document);
      XmlElement assemblyNode = (XmlElement) document.SelectSingleNode("/asmv1:assembly", namespaceMgr);
      foreach (System.Deployment.Application.Manifest.File file in files)
        ManifestGenerator.AddFile(document, assemblyNode, file);
    }

    private static void AddFile(XmlDocument document, XmlElement assemblyNode, System.Deployment.Application.Manifest.File file)
    {
      XmlElement element = document.CreateElement("file", "urn:schemas-microsoft-com:asm.v1");
      assemblyNode.AppendChild((XmlNode) element);
      element.SetAttributeNode("name", (string) null).Value = file.Name;
    }

    private static void AddDependencies(XmlDocument document, DependentAssembly[] dependentAssemblies)
    {
      Hashtable hashtable = new Hashtable();
      XmlNamespaceManager namespaceMgr = ManifestGenerator.GetNamespaceMgr(document);
      XmlElement xmlElement = (XmlElement) document.SelectSingleNode("/asmv1:assembly", namespaceMgr);
      foreach (DependentAssembly dependentAssembly in dependentAssemblies)
      {
        if (!hashtable.Contains((object) dependentAssembly.Identity))
        {
          XmlElement element1 = document.CreateElement("dependency", "urn:schemas-microsoft-com:asm.v1");
          xmlElement.AppendChild((XmlNode) element1);
          XmlElement element2 = document.CreateElement("dependentAssembly", "urn:schemas-microsoft-com:asm.v1");
          element1.AppendChild((XmlNode) element2);
          ReferenceIdentity identity = dependentAssembly.Identity;
          DefinitionIdentity asmId = new DefinitionIdentity(identity);
          XmlElement assemblyIdentityElement = ManifestGenerator.CreateAssemblyIdentityElement(document, asmId);
          element2.AppendChild((XmlNode) assemblyIdentityElement);
          hashtable.Add((object) identity, (object) asmId);
        }
      }
    }

    private static void InjectIdentityXml(XmlDocument document, DefinitionIdentity asmId)
    {
      XmlElement assemblyIdentityElement = ManifestGenerator.CreateAssemblyIdentityElement(document, asmId);
      document.DocumentElement.AppendChild((XmlNode) assemblyIdentityElement);
    }

    private static XmlElement CreateAssemblyIdentityElement(XmlDocument document, DefinitionIdentity asmId)
    {
      XmlElement element = document.CreateElement("assemblyIdentity", "urn:schemas-microsoft-com:asm.v1");
      IDENTITY_ATTRIBUTE[] attributes = asmId.Attributes;
      StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase;
      foreach (IDENTITY_ATTRIBUTE identityAttribute in attributes)
      {
        string namespaceURI = identityAttribute.Namespace;
        string localName = identityAttribute.Name;
        if (namespaceURI == null)
        {
          if (localName.Equals("name", comparisonType))
            localName = "name";
          else if (localName.Equals("version", comparisonType))
            localName = "version";
          else if (localName.Equals("processorArchitecture", comparisonType))
            localName = "processorArchitecture";
          else if (localName.Equals("publicKeyToken", comparisonType))
            localName = "publicKeyToken";
          else if (localName.Equals("type", comparisonType))
            localName = "type";
          else if (localName.Equals("culture", comparisonType))
            localName = "language";
        }
        element.SetAttribute(localName, namespaceURI, identityAttribute.Value);
      }
      return element;
    }

    private static XmlDocument CloneAssemblyTemplate()
    {
      if (ManifestGenerator.assemblyTemplateDoc == null)
      {
        Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssemblyTemplate.xml");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(manifestResourceStream);
        Interlocked.CompareExchange(ref ManifestGenerator.assemblyTemplateDoc, (object) xmlDocument, (object) null);
      }
      return (XmlDocument) ((XmlNode) ManifestGenerator.assemblyTemplateDoc).Clone();
    }

    private static XmlNamespaceManager GetNamespaceMgr(XmlDocument document)
    {
      XmlNamespaceManager namespaceManager = new XmlNamespaceManager(document.NameTable);
      namespaceManager.AddNamespace("asmv1", "urn:schemas-microsoft-com:asm.v1");
      namespaceManager.AddNamespace("asmv2", "urn:schemas-microsoft-com:asm.v2");
      namespaceManager.AddNamespace("dsig", "http://www.w3.org/2000/09/xmldsig#");
      return namespaceManager;
    }
  }
}
