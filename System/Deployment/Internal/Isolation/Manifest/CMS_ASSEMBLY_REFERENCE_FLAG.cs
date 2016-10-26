// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.Manifest.CMS_ASSEMBLY_REFERENCE_FLAG
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

namespace System.Deployment.Internal.Isolation.Manifest
{
  internal enum CMS_ASSEMBLY_REFERENCE_FLAG
  {
    CMS_ASSEMBLY_REFERENCE_FLAG_OPTIONAL = 1,
    CMS_ASSEMBLY_REFERENCE_FLAG_VISIBLE = 2,
    CMS_ASSEMBLY_REFERENCE_FLAG_FOLLOW = 4,
    CMS_ASSEMBLY_REFERENCE_FLAG_IS_PLATFORM = 8,
    CMS_ASSEMBLY_REFERENCE_FLAG_CULTURE_WILDCARDED = 16,
    CMS_ASSEMBLY_REFERENCE_FLAG_PROCESSOR_ARCHITECTURE_WILDCARDED = 32,
    CMS_ASSEMBLY_REFERENCE_FLAG_PREREQUISITE = 128,
  }
}
