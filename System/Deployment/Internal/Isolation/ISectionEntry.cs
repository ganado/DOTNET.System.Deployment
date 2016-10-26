// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.ISectionEntry
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;

namespace System.Deployment.Internal.Isolation
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("285a8861-c84a-11d7-850f-005cd062464f")]
  [ComImport]
  internal interface ISectionEntry
  {
    object GetField(uint fieldId);

    string GetFieldName(uint fieldId);
  }
}
