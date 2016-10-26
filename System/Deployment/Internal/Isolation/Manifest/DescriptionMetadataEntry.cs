// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.Manifest.DescriptionMetadataEntry
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;

namespace System.Deployment.Internal.Isolation.Manifest
{
  [StructLayout(LayoutKind.Sequential)]
  internal class DescriptionMetadataEntry
  {
    [MarshalAs(UnmanagedType.LPWStr)]
    public string Publisher;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string Product;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string SupportUrl;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string IconFile;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string ErrorReportUrl;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string SuiteName;
  }
}
