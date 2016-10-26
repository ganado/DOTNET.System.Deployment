// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.STORE_ASSEMBLY_STATUS_FLAGS
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

namespace System.Deployment.Internal.Isolation
{
  [Flags]
  internal enum STORE_ASSEMBLY_STATUS_FLAGS
  {
    STORE_ASSEMBLY_STATUS_MANIFEST_ONLY = 1,
    STORE_ASSEMBLY_STATUS_PAYLOAD_RESIDENT = 2,
    STORE_ASSEMBLY_STATUS_PARTIAL_INSTALL = 4,
  }
}
