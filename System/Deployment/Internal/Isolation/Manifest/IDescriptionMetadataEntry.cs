// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.Manifest.IDescriptionMetadataEntry
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("CB73147E-5FC2-4c31-B4E6-58D13DBE1A08")]
  [ComImport]
  internal interface IDescriptionMetadataEntry
  {
    DescriptionMetadataEntry AllData { [SecurityCritical] get; }

    string Publisher { [SecurityCritical] get; }

    string Product { [SecurityCritical] get; }

    string SupportUrl { [SecurityCritical] get; }

    string IconFile { [SecurityCritical] get; }

    string ErrorReportUrl { [SecurityCritical] get; }

    string SuiteName { [SecurityCritical] get; }
  }
}
