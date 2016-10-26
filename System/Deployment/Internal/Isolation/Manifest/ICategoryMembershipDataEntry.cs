// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.Manifest.ICategoryMembershipDataEntry
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("DA0C3B27-6B6B-4b80-A8F8-6CE14F4BC0A4")]
  [ComImport]
  internal interface ICategoryMembershipDataEntry
  {
    CategoryMembershipDataEntry AllData { [SecurityCritical] get; }

    uint index { [SecurityCritical] get; }

    string Xml { [SecurityCritical] get; }

    string Description { [SecurityCritical] get; }
  }
}
