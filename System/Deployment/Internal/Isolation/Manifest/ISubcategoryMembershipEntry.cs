// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.Manifest.ISubcategoryMembershipEntry
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("5A7A54D7-5AD5-418e-AB7A-CF823A8D48D0")]
  [ComImport]
  internal interface ISubcategoryMembershipEntry
  {
    SubcategoryMembershipEntry AllData { [SecurityCritical] get; }

    string Subcategory { [SecurityCritical] get; }

    ISection CategoryMembershipData { [SecurityCritical] get; }
  }
}
