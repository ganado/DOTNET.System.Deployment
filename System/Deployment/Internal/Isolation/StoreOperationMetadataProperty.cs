// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.StoreOperationMetadataProperty
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;

namespace System.Deployment.Internal.Isolation
{
  internal struct StoreOperationMetadataProperty
  {
    public Guid GuidPropertySet;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string Name;
    [MarshalAs(UnmanagedType.SysUInt)]
    public IntPtr ValueSize;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string Value;

    public StoreOperationMetadataProperty(Guid PropertySet, string Name)
    {
      this = new StoreOperationMetadataProperty(PropertySet, Name, (string) null);
    }

    public StoreOperationMetadataProperty(Guid PropertySet, string Name, string Value)
    {
      this.GuidPropertySet = PropertySet;
      this.Name = Name;
      this.Value = Value;
      this.ValueSize = Value != null ? new IntPtr((Value.Length + 1) * 2) : IntPtr.Zero;
    }
  }
}
