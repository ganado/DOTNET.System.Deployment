// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.ManifestValidatingReader
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace System.Deployment.Application
{
  internal static class ManifestValidatingReader
  {
    private static string[] _manifestSchemas = new string[1]
    {
      "manifest.2.0.0.15-pre.adaptive.xsd"
    };
    private static XmlSchemaSet _manifestSchemaSet = (XmlSchemaSet) null;
    private static object _manifestSchemaSetLock = new object();

    private static XmlSchemaSet ManifestSchemaSet
    {
      get
      {
        if (ManifestValidatingReader._manifestSchemaSet == null)
        {
          lock (ManifestValidatingReader._manifestSchemaSetLock)
          {
            if (ManifestValidatingReader._manifestSchemaSet == null)
              ManifestValidatingReader._manifestSchemaSet = ManifestValidatingReader.MakeSchemaSet(ManifestValidatingReader._manifestSchemas);
          }
        }
        return ManifestValidatingReader._manifestSchemaSet;
      }
    }

    public static XmlReader Create(Stream stream)
    {
      return ManifestValidatingReader.Create(stream, ManifestValidatingReader.ManifestSchemaSet);
    }

    private static XmlReader Create(Stream stream, XmlSchemaSet schemaSet)
    {
      return XmlReader.Create((XmlReader) new ManifestValidatingReader.XmlFilteredReader(stream), new XmlReaderSettings()
      {
        Schemas = schemaSet,
        ValidationType = ValidationType.Schema,
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = (XmlResolver) null
      });
    }

    private static XmlSchemaSet MakeSchemaSet(string[] schemas)
    {
      XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
      Assembly executingAssembly = Assembly.GetExecutingAssembly();
      xmlSchemaSet.XmlResolver = (XmlResolver) new ManifestValidatingReader.ResourceResolver(executingAssembly);
      for (int index = 0; index < schemas.Length; ++index)
      {
        using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(schemas[index]))
          xmlSchemaSet.Add((string) null, (XmlReader) new XmlTextReader(manifestResourceStream));
      }
      return xmlSchemaSet;
    }

    private class ResourceResolver : XmlUrlResolver
    {
      private const string Prefix = "df://resources/";
      private Assembly _assembly;

      public ResourceResolver(Assembly assembly)
      {
        this._assembly = assembly;
      }

      public override Uri ResolveUri(Uri baseUri, string relativeUri)
      {
        if (baseUri == (Uri) null || baseUri.ToString() == string.Empty || baseUri.IsAbsoluteUri && baseUri.AbsoluteUri.StartsWith("df://resources/", StringComparison.Ordinal))
          return new Uri("df://resources/" + relativeUri);
        return base.ResolveUri(baseUri, relativeUri);
      }

      public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
      {
        if (!absoluteUri.AbsoluteUri.StartsWith("df://resources/", StringComparison.Ordinal))
          return base.GetEntity(absoluteUri, role, ofObjectToReturn);
        if (ofObjectToReturn != (Type) null && ofObjectToReturn != typeof (Stream))
          throw new XmlException(Resources.GetString("Ex_OnlyStreamTypeSupported"));
        if (absoluteUri.ToString() == "df://resources/-//W3C//DTD XMLSCHEMA 200102//EN")
          return (object) this._assembly.GetManifestResourceStream("XMLSchema.dtd");
        if (absoluteUri.ToString() == "df://resources/xs-datatypes")
          return (object) this._assembly.GetManifestResourceStream("datatypes.dtd");
        return (object) this._assembly.GetManifestResourceStream(absoluteUri.AbsoluteUri.Remove(0, "df://resources/".Length));
      }
    }

    private class XmlFilteredReader : XmlTextReader
    {
      private static StringCollection KnownNamespaces = new StringCollection();

      static XmlFilteredReader()
      {
        ManifestValidatingReader.XmlFilteredReader.KnownNamespaces.Add("urn:schemas-microsoft-com:asm.v1");
        ManifestValidatingReader.XmlFilteredReader.KnownNamespaces.Add("urn:schemas-microsoft-com:asm.v2");
        ManifestValidatingReader.XmlFilteredReader.KnownNamespaces.Add("http://www.w3.org/2000/09/xmldsig#");
      }

      public XmlFilteredReader(Stream stream)
        : base(stream)
      {
        this.DtdProcessing = DtdProcessing.Prohibit;
      }

      public override bool Read()
      {
        bool flag = base.Read();
        if (this.NodeType == XmlNodeType.Element && !ManifestValidatingReader.XmlFilteredReader.KnownNamespaces.Contains(this.NamespaceURI))
          this.Skip();
        return flag;
      }
    }
  }
}
