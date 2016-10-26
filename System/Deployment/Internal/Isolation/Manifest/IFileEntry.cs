// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.Manifest.IFileEntry
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("A2A55FAD-349B-469b-BF12-ADC33D14A937")]
  [ComImport]
  internal interface IFileEntry
  {
    FileEntry AllData { [SecurityCritical] get; }

    string Name { [SecurityCritical] get; }

    uint HashAlgorithm { [SecurityCritical] get; }

    string LoadFrom { [SecurityCritical] get; }

    string SourcePath { [SecurityCritical] get; }

    string ImportPath { [SecurityCritical] get; }

    string SourceName { [SecurityCritical] get; }

    string Location { [SecurityCritical] get; }

    object HashValue { [SecurityCritical] get; }

    ulong Size { [SecurityCritical] get; }

    string Group { [SecurityCritical] get; }

    uint Flags { [SecurityCritical] get; }

    IMuiResourceMapEntry MuiMapping { [SecurityCritical] get; }

    uint WritableType { [SecurityCritical] get; }

    ISection HashElements { [SecurityCritical] get; }
  }
}
