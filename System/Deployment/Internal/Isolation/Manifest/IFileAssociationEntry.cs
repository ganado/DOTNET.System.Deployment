// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.Manifest.IFileAssociationEntry
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("0C66F299-E08E-48c5-9264-7CCBEB4D5CBB")]
  [ComImport]
  internal interface IFileAssociationEntry
  {
    FileAssociationEntry AllData { [SecurityCritical] get; }

    string Extension { [SecurityCritical] get; }

    string Description { [SecurityCritical] get; }

    string ProgID { [SecurityCritical] get; }

    string DefaultIcon { [SecurityCritical] get; }

    string Parameter { [SecurityCritical] get; }
  }
}
