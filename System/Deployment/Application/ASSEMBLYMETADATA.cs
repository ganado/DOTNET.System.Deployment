﻿// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.ASSEMBLYMETADATA
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

namespace System.Deployment.Application
{
  internal struct ASSEMBLYMETADATA
  {
    public ushort usMajorVersion;
    public ushort usMinorVersion;
    public ushort usBuildNumber;
    public ushort usRevisionNumber;
    public IntPtr rpLocale;
    public uint cchLocale;
    public IntPtr rpProcessors;
    public uint cProcessors;
    public IntPtr rOses;
    public uint cOses;
  }
}
