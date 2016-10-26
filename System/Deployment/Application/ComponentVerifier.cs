// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.ComponentVerifier
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using Microsoft.Runtime.Hosting;
using System.Collections;
using System.Deployment.Application.Manifest;
using System.Deployment.Internal.Isolation.Manifest;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace System.Deployment.Application
{
  internal class ComponentVerifier
  {
    protected static CMS_HASH_DIGESTMETHOD[] _supportedDigestMethods = new CMS_HASH_DIGESTMETHOD[4]
    {
      CMS_HASH_DIGESTMETHOD.CMS_HASH_DIGESTMETHOD_SHA1,
      CMS_HASH_DIGESTMETHOD.CMS_HASH_DIGESTMETHOD_SHA256,
      CMS_HASH_DIGESTMETHOD.CMS_HASH_DIGESTMETHOD_SHA384,
      CMS_HASH_DIGESTMETHOD.CMS_HASH_DIGESTMETHOD_SHA512
    };
    protected static CMS_HASH_TRANSFORM[] _supportedTransforms = new CMS_HASH_TRANSFORM[2]
    {
      CMS_HASH_TRANSFORM.CMS_HASH_TRANSFORM_MANIFESTINVARIANT,
      CMS_HASH_TRANSFORM.CMS_HASH_TRANSFORM_IDENTITY
    };
    protected ArrayList _verificationComponents = new ArrayList();

    public static CMS_HASH_TRANSFORM[] VerifiableTransformTypes
    {
      get
      {
        return ComponentVerifier._supportedTransforms;
      }
    }

    public static CMS_HASH_DIGESTMETHOD[] VerifiableDigestMethods
    {
      get
      {
        return ComponentVerifier._supportedDigestMethods;
      }
    }

    public void AddFileForVerification(string filePath, HashCollection verificationHashCollection)
    {
      this._verificationComponents.Add((object) new ComponentVerifier.FileComponent(filePath, verificationHashCollection));
    }

    public void AddSimplyNamedAssemblyForVerification(string filePath, AssemblyManifest assemblyManifest)
    {
      this._verificationComponents.Add((object) new ComponentVerifier.SimplyNamedAssemblyComponent(filePath, assemblyManifest));
    }

    public void AddStrongNameAssemblyForVerification(string filePath, AssemblyManifest assemblyManifest)
    {
      this._verificationComponents.Add((object) new ComponentVerifier.StrongNameAssemblyComponent(filePath, assemblyManifest));
    }

    public void VerifyComponents()
    {
      foreach (ComponentVerifier.VerificationComponent verificationComponent in this._verificationComponents)
        verificationComponent.Verify();
    }

    public static void VerifyFileHash(string filePath, HashCollection hashCollection)
    {
      string fileName = Path.GetFileName(filePath);
      if (hashCollection.Count == 0)
      {
        if (!PolicyKeys.RequireHashInManifests())
          Logger.AddWarningInformation(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("NoHashFile"), new object[1]
          {
            (object) fileName
          }));
        else
          throw new InvalidDeploymentException(ExceptionTypes.HashValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_HashNotSpecified"), new object[1]
          {
            (object) fileName
          }));
      }
      foreach (Hash hash in hashCollection)
        ComponentVerifier.VerifyFileHash(filePath, hash);
    }

    public static void VerifyFileHash(string filePath, Hash hash)
    {
      string fileName = Path.GetFileName(filePath);
      byte[] digestValue1;
      try
      {
        digestValue1 = ComponentVerifier.GenerateDigestValue(filePath, hash.DigestMethod, hash.Transform);
      }
      catch (IOException ex)
      {
        throw new InvalidDeploymentException(ExceptionTypes.HashValidation, Resources.GetString("Ex_HashValidationException"), (Exception) ex);
      }
      byte[] digestValue2 = hash.DigestValue;
      bool flag = false;
      if (digestValue1.Length == digestValue2.Length)
      {
        int index = 0;
        while (index < digestValue2.Length && (int) digestValue2[index] == (int) digestValue1[index])
          ++index;
        if (index >= digestValue2.Length)
          flag = true;
      }
      if (!flag)
      {
        Logger.AddInternalState("File," + fileName + ", has a different computed hash than specified in manifest. Computed hash is " + Encoding.UTF8.GetString(digestValue1) + ". Specified hash is " + Encoding.UTF8.GetString(digestValue2));
        throw new InvalidDeploymentException(ExceptionTypes.HashValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_DifferentHashes"), new object[1]
        {
          (object) fileName
        }));
      }
    }

    public static byte[] GenerateDigestValue(string filePath, CMS_HASH_DIGESTMETHOD digestMethod, CMS_HASH_TRANSFORM transform)
    {
      Stream inputStream = (Stream) null;
      try
      {
        HashAlgorithm hashAlgorithm = ComponentVerifier.GetHashAlgorithm(digestMethod);
        inputStream = ComponentVerifier.GetTransformedStream(filePath, transform);
        return hashAlgorithm.ComputeHash(inputStream);
      }
      finally
      {
        if (inputStream != null)
          inputStream.Close();
      }
    }

    public static bool IsVerifiableHashCollection(HashCollection hashCollection)
    {
      foreach (Hash hash in hashCollection)
      {
        if (!ComponentVerifier.IsVerifiableHash(hash))
          return false;
      }
      return true;
    }

    public static bool IsVerifiableHash(Hash hash)
    {
      return Array.IndexOf<CMS_HASH_TRANSFORM>(ComponentVerifier.VerifiableTransformTypes, hash.Transform) >= 0 && Array.IndexOf<CMS_HASH_DIGESTMETHOD>(ComponentVerifier.VerifiableDigestMethods, hash.DigestMethod) >= 0 && (hash.DigestValue != null && hash.DigestValue.Length != 0);
    }

    public static HashAlgorithm GetHashAlgorithm(CMS_HASH_DIGESTMETHOD digestMethod)
    {
      if (digestMethod == CMS_HASH_DIGESTMETHOD.CMS_HASH_DIGESTMETHOD_SHA1)
        return (HashAlgorithm) new SHA1CryptoServiceProvider();
      if (digestMethod == CMS_HASH_DIGESTMETHOD.CMS_HASH_DIGESTMETHOD_SHA256)
      {
        if (PlatformSpecific.OnWindows2003 || PlatformSpecific.OnVistaOrAbove)
          return CryptoConfig.CreateFromName("System.Security.Cryptography.SHA256CryptoServiceProvider") as HashAlgorithm;
        return (HashAlgorithm) new SHA256Managed();
      }
      if (digestMethod == CMS_HASH_DIGESTMETHOD.CMS_HASH_DIGESTMETHOD_SHA384)
      {
        if (PlatformSpecific.OnWindows2003 || PlatformSpecific.OnVistaOrAbove)
          return CryptoConfig.CreateFromName("System.Security.Cryptography.SHA384CryptoServiceProvider") as HashAlgorithm;
        return (HashAlgorithm) new SHA384Managed();
      }
      if (digestMethod == CMS_HASH_DIGESTMETHOD.CMS_HASH_DIGESTMETHOD_SHA512)
      {
        if (PlatformSpecific.OnWindows2003 || PlatformSpecific.OnVistaOrAbove)
          return CryptoConfig.CreateFromName("System.Security.Cryptography.SHA512CryptoServiceProvider") as HashAlgorithm;
        return (HashAlgorithm) new SHA512Managed();
      }
      throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_DigestMethodNotSupported"), new object[1]
      {
        (object) digestMethod.ToString()
      }));
    }

    public static Stream GetTransformedStream(string filePath, CMS_HASH_TRANSFORM transform)
    {
      Stream stream = (Stream) null;
      if (transform == CMS_HASH_TRANSFORM.CMS_HASH_TRANSFORM_MANIFESTINVARIANT)
      {
        PEStream peStream = (PEStream) null;
        try
        {
          peStream = new PEStream(filePath, true);
          peStream.ZeroOutOptionalHeaderCheckSum();
          peStream.ZeroOutDefaultId1ManifestResource();
          stream = (Stream) peStream;
          return stream;
        }
        finally
        {
          if (peStream != stream && peStream != null)
            peStream.Close();
        }
      }
      else
      {
        if (transform == CMS_HASH_TRANSFORM.CMS_HASH_TRANSFORM_IDENTITY)
          return (Stream) new FileStream(filePath, FileMode.Open, FileAccess.Read);
        throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_TransformAlgorithmNotSupported"), new object[1]
        {
          (object) transform.ToString()
        }));
      }
    }

    public static void VerifySimplyNamedAssembly(string filePath, AssemblyManifest assemblyManifest)
    {
      string fileName = Path.GetFileName(filePath);
      if (assemblyManifest.Identity.PublicKeyToken != null)
        throw new InvalidDeploymentException(ExceptionTypes.Validation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_SimplyNamedAsmWithPKT"), new object[1]
        {
          (object) fileName
        }));
      if (assemblyManifest.ManifestSourceFormat == ManifestSourceFormat.ID_1 && assemblyManifest.ComplibIdentity != null && assemblyManifest.ComplibIdentity.PublicKeyToken != null)
        throw new InvalidDeploymentException(ExceptionTypes.IdentityMatchValidationForMixedModeAssembly, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_SimplyNamedAsmWithStrongNameComplib"), new object[1]
        {
          (object) fileName
        }));
    }

    public static void VerifyStrongNameAssembly(string filePath, AssemblyManifest assemblyManifest)
    {
      string fileName = Path.GetFileName(filePath);
      if (assemblyManifest.Identity.PublicKeyToken == null)
        throw new InvalidDeploymentException(ExceptionTypes.Validation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_StrongNameAsmWithNoPKT"), new object[1]
        {
          (object) fileName
        }));
      bool ignoreSelfReferentialFileHash = false;
      if (assemblyManifest.ManifestSourceFormat == ManifestSourceFormat.XmlFile)
        assemblyManifest.ValidateSignature((Stream) null);
      else if (assemblyManifest.ManifestSourceFormat == ManifestSourceFormat.ID_1)
      {
        if (assemblyManifest.ComplibIdentity == null)
        {
          PEStream peStream = (PEStream) null;
          MemoryStream memoryStream = (MemoryStream) null;
          try
          {
            peStream = new PEStream(filePath, true);
            byte[] manifestResource = peStream.GetDefaultId1ManifestResource();
            if (manifestResource != null)
              memoryStream = new MemoryStream(manifestResource);
            if (memoryStream != null)
              assemblyManifest.ValidateSignature((Stream) memoryStream);
            else
              throw new InvalidDeploymentException(ExceptionTypes.StronglyNamedAssemblyVerification, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_StronglyNamedAssemblyNotVerifiable"), new object[1]
              {
                (object) fileName
              }));
          }
          finally
          {
            if (peStream != null)
              peStream.Close();
            if (memoryStream != null)
              memoryStream.Close();
          }
        }
        else
        {
          if (!assemblyManifest.ComplibIdentity.Equals((object) assemblyManifest.Identity))
            throw new InvalidDeploymentException(ExceptionTypes.IdentityMatchValidationForMixedModeAssembly, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_IdentitiesDoNotMatchForMixedModeAssembly"), new object[1]
            {
              (object) fileName
            }));
          bool pfWasVerified;
          if (!StrongNameHelpers.StrongNameSignatureVerificationEx(filePath, false, out pfWasVerified))
            throw new InvalidDeploymentException(ExceptionTypes.SignatureValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_StrongNameSignatureInvalid"), new object[1]
            {
              (object) fileName
            }));
          ignoreSelfReferentialFileHash = true;
        }
      }
      else if (assemblyManifest.ManifestSourceFormat == ManifestSourceFormat.CompLib)
      {
        bool pfWasVerified;
        if (!StrongNameHelpers.StrongNameSignatureVerificationEx(filePath, false, out pfWasVerified))
          throw new InvalidDeploymentException(ExceptionTypes.SignatureValidation, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_StrongNameSignatureInvalid"), new object[1]
          {
            (object) fileName
          }));
        ignoreSelfReferentialFileHash = true;
      }
      else
        throw new InvalidDeploymentException(ExceptionTypes.StronglyNamedAssemblyVerification, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_StronglyNamedAssemblyNotVerifiable"), new object[1]
        {
          (object) fileName
        }));
      ComponentVerifier.VerifyManifestComponentFiles(assemblyManifest, filePath, ignoreSelfReferentialFileHash);
    }

    protected static void VerifyManifestComponentFiles(AssemblyManifest manifest, string componentPath, bool ignoreSelfReferentialFileHash)
    {
      string directoryName = Path.GetDirectoryName(componentPath);
      foreach (System.Deployment.Application.Manifest.File file in manifest.Files)
      {
        string str = Path.Combine(directoryName, file.NameFS);
        if ((!ignoreSelfReferentialFileHash || string.Compare(componentPath, str, StringComparison.OrdinalIgnoreCase) != 0) && System.IO.File.Exists(str))
          ComponentVerifier.VerifyFileHash(str, file.HashCollection);
      }
    }

    protected abstract class VerificationComponent
    {
      public abstract void Verify();
    }

    protected class FileComponent : ComponentVerifier.VerificationComponent
    {
      protected string _filePath;
      protected HashCollection _hashCollection;

      public FileComponent(string filePath, HashCollection hashCollection)
      {
        this._filePath = filePath;
        this._hashCollection = hashCollection;
      }

      public override void Verify()
      {
        ComponentVerifier.VerifyFileHash(this._filePath, this._hashCollection);
      }
    }

    protected class SimplyNamedAssemblyComponent : ComponentVerifier.VerificationComponent
    {
      protected string _filePath;
      protected AssemblyManifest _assemblyManifest;

      public SimplyNamedAssemblyComponent(string filePath, AssemblyManifest assemblyManifest)
      {
        this._filePath = filePath;
        this._assemblyManifest = assemblyManifest;
      }

      public override void Verify()
      {
        ComponentVerifier.VerifySimplyNamedAssembly(this._filePath, this._assemblyManifest);
      }
    }

    protected class StrongNameAssemblyComponent : ComponentVerifier.VerificationComponent
    {
      protected string _filePath;
      protected AssemblyManifest _assemblyManifest;

      public StrongNameAssemblyComponent(string filePath, AssemblyManifest assemblyManifest)
      {
        this._filePath = filePath;
        this._assemblyManifest = assemblyManifest;
      }

      public override void Verify()
      {
        ComponentVerifier.VerifyStrongNameAssembly(this._filePath, this._assemblyManifest);
      }
    }
  }
}
