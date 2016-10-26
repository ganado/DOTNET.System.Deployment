// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.CodeSigning.CmiManifestVerifyFlags
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

namespace System.Deployment.Internal.CodeSigning
{
  [Flags]
  internal enum CmiManifestVerifyFlags
  {
    None = 0,
    RevocationNoCheck = 1,
    RevocationCheckEndCertOnly = 2,
    RevocationCheckEntireChain = 4,
    UrlCacheOnlyRetrieval = 8,
    LifetimeSigning = 16,
    TrustMicrosoftRootOnly = 32,
    StrongNameOnly = 65536,
  }
}
