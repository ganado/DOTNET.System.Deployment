// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.ManifestReader
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Deployment.Application.Manifest;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace System.Deployment.Application
{
  internal static class ManifestReader
  {
    internal static AssemblyManifest FromDocument(string localPath, AssemblyManifest.ManifestType manifestType, Uri sourceUri)
    {
      CodeMarker_Singleton.Instance.CodeMarker(7302);
      Logger.AddMethodCall("ManifestReader.FromDocument(" + localPath + ") called.");
      if (new FileInfo(localPath).Length > 16777216L)
        throw new DeploymentException(Resources.GetString("Ex_ManifestFileTooLarge"));
      AssemblyManifest assemblyManifest;
      using (FileStream fileStream = new FileStream(localPath, FileMode.Open, FileAccess.Read))
      {
        try
        {
          XmlReader xmlReader = PolicyKeys.SkipSchemaValidation() ? XmlReader.Create((Stream) fileStream, new XmlReaderSettings()
          {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = (XmlResolver) null
          }) : ManifestValidatingReader.Create((Stream) fileStream);
          do
            ;
          while (xmlReader.Read());
          Logger.AddInternalState("Schema validation passed.");
          assemblyManifest = new AssemblyManifest(fileStream);
          Logger.AddInternalState("Manifest is parsed successfully.");
          if (!PolicyKeys.SkipSemanticValidation())
            assemblyManifest.ValidateSemantics(manifestType);
          Logger.AddInternalState("Semantic validation passed.");
          if (!PolicyKeys.SkipSignatureValidation())
          {
            fileStream.Position = 0L;
            assemblyManifest.ValidateSignature((Stream) fileStream);
          }
          Logger.AddInternalState("Signature validation passed.");
        }
        catch (XmlException ex)
        {
          throw new InvalidDeploymentException(ExceptionTypes.ManifestParse, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_ManifestFromDocument"), new object[1]
          {
            (object) (sourceUri != (Uri) null ? sourceUri.AbsoluteUri : Path.GetFileName(localPath))
          }), (Exception) ex);
        }
        catch (XmlSchemaValidationException ex)
        {
          throw new InvalidDeploymentException(ExceptionTypes.ManifestParse, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_ManifestFromDocument"), new object[1]
          {
            (object) (sourceUri != (Uri) null ? sourceUri.AbsoluteUri : Path.GetFileName(localPath))
          }), (Exception) ex);
        }
        catch (InvalidDeploymentException ex)
        {
          throw new InvalidDeploymentException(ExceptionTypes.ManifestParse, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_ManifestFromDocument"), new object[1]
          {
            (object) (sourceUri != (Uri) null ? sourceUri.AbsoluteUri : Path.GetFileName(localPath))
          }), (Exception) ex);
        }
      }
      CodeMarker_Singleton.Instance.CodeMarker(7303);
      return assemblyManifest;
    }

    internal static AssemblyManifest FromDocumentNoValidation(string localPath)
    {
      CodeMarker_Singleton.Instance.CodeMarker(7302);
      Logger.AddMethodCall("ManifestReader.FromDocumentNoValidation(" + localPath + ") called.");
      if (new FileInfo(localPath).Length > 16777216L)
        throw new DeploymentException(Resources.GetString("Ex_ManifestFileTooLarge"));
      AssemblyManifest assemblyManifest;
      using (FileStream fileStream = new FileStream(localPath, FileMode.Open, FileAccess.Read))
        assemblyManifest = new AssemblyManifest(fileStream);
      CodeMarker_Singleton.Instance.CodeMarker(7303);
      return assemblyManifest;
    }
  }
}
