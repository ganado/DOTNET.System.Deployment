// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.StoreOperationScavenge
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;

namespace System.Deployment.Internal.Isolation
{
  internal struct StoreOperationScavenge
  {
    [MarshalAs(UnmanagedType.U4)]
    public uint Size;
    [MarshalAs(UnmanagedType.U4)]
    public StoreOperationScavenge.OpFlags Flags;
    [MarshalAs(UnmanagedType.U8)]
    public ulong SizeReclaimationLimit;
    [MarshalAs(UnmanagedType.U8)]
    public ulong RuntimeLimit;
    [MarshalAs(UnmanagedType.U4)]
    public uint ComponentCountLimit;

    public StoreOperationScavenge(bool Light, ulong SizeLimit, ulong RunLimit, uint ComponentLimit)
    {
      this.Size = (uint) Marshal.SizeOf(typeof (StoreOperationScavenge));
      this.Flags = StoreOperationScavenge.OpFlags.Nothing;
      if (Light)
        this.Flags = this.Flags | StoreOperationScavenge.OpFlags.Light;
      this.SizeReclaimationLimit = SizeLimit;
      if ((long) SizeLimit != 0L)
        this.Flags = this.Flags | StoreOperationScavenge.OpFlags.LimitSize;
      this.RuntimeLimit = RunLimit;
      if ((long) RunLimit != 0L)
        this.Flags = this.Flags | StoreOperationScavenge.OpFlags.LimitTime;
      this.ComponentCountLimit = ComponentLimit;
      if ((int) ComponentLimit == 0)
        return;
      this.Flags = this.Flags | StoreOperationScavenge.OpFlags.LimitCount;
    }

    public StoreOperationScavenge(bool Light)
    {
      this = new StoreOperationScavenge(Light, 0UL, 0UL, 0U);
    }

    public void Destroy()
    {
    }

    [System.Flags]
    public enum OpFlags
    {
      Nothing = 0,
      Light = 1,
      LimitSize = 2,
      LimitTime = 4,
      LimitCount = 8,
    }
  }
}
