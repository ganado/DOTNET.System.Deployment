// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.StoreOperationSetDeploymentMetadata
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation
{
  internal struct StoreOperationSetDeploymentMetadata
  {
    [MarshalAs(UnmanagedType.U4)]
    public uint Size;
    [MarshalAs(UnmanagedType.U4)]
    public StoreOperationSetDeploymentMetadata.OpFlags Flags;
    [MarshalAs(UnmanagedType.Interface)]
    public IDefinitionAppId Deployment;
    [MarshalAs(UnmanagedType.SysInt)]
    public IntPtr InstallerReference;
    [MarshalAs(UnmanagedType.SysInt)]
    public IntPtr cPropertiesToTest;
    [MarshalAs(UnmanagedType.SysInt)]
    public IntPtr PropertiesToTest;
    [MarshalAs(UnmanagedType.SysInt)]
    public IntPtr cPropertiesToSet;
    [MarshalAs(UnmanagedType.SysInt)]
    public IntPtr PropertiesToSet;

    public StoreOperationSetDeploymentMetadata(IDefinitionAppId Deployment, StoreApplicationReference Reference, StoreOperationMetadataProperty[] SetProperties)
    {
      this = new StoreOperationSetDeploymentMetadata(Deployment, Reference, SetProperties, (StoreOperationMetadataProperty[]) null);
    }

    [SecuritySafeCritical]
    public StoreOperationSetDeploymentMetadata(IDefinitionAppId Deployment, StoreApplicationReference Reference, StoreOperationMetadataProperty[] SetProperties, StoreOperationMetadataProperty[] TestProperties)
    {
      this.Size = (uint) Marshal.SizeOf(typeof (StoreOperationSetDeploymentMetadata));
      this.Flags = StoreOperationSetDeploymentMetadata.OpFlags.Nothing;
      this.Deployment = Deployment;
      if (SetProperties != null)
      {
        this.PropertiesToSet = StoreOperationSetDeploymentMetadata.MarshalProperties(SetProperties);
        this.cPropertiesToSet = new IntPtr(SetProperties.Length);
      }
      else
      {
        this.PropertiesToSet = IntPtr.Zero;
        this.cPropertiesToSet = IntPtr.Zero;
      }
      if (TestProperties != null)
      {
        this.PropertiesToTest = StoreOperationSetDeploymentMetadata.MarshalProperties(TestProperties);
        this.cPropertiesToTest = new IntPtr(TestProperties.Length);
      }
      else
      {
        this.PropertiesToTest = IntPtr.Zero;
        this.cPropertiesToTest = IntPtr.Zero;
      }
      this.InstallerReference = Reference.ToIntPtr();
    }

    [SecurityCritical]
    public void Destroy()
    {
      if (this.PropertiesToSet != IntPtr.Zero)
      {
        StoreOperationSetDeploymentMetadata.DestroyProperties(this.PropertiesToSet, (ulong) this.cPropertiesToSet.ToInt64());
        this.PropertiesToSet = IntPtr.Zero;
        this.cPropertiesToSet = IntPtr.Zero;
      }
      if (this.PropertiesToTest != IntPtr.Zero)
      {
        StoreOperationSetDeploymentMetadata.DestroyProperties(this.PropertiesToTest, (ulong) this.cPropertiesToTest.ToInt64());
        this.PropertiesToTest = IntPtr.Zero;
        this.cPropertiesToTest = IntPtr.Zero;
      }
      if (!(this.InstallerReference != IntPtr.Zero))
        return;
      StoreApplicationReference.Destroy(this.InstallerReference);
      this.InstallerReference = IntPtr.Zero;
    }

    [SecurityCritical]
    private static void DestroyProperties(IntPtr rgItems, ulong iItems)
    {
      if (!(rgItems != IntPtr.Zero))
        return;
      ulong num = (ulong) Marshal.SizeOf(typeof (StoreOperationMetadataProperty));
      for (ulong index = 0; index < iItems; ++index)
        Marshal.DestroyStructure(new IntPtr((long) index * (long) num + rgItems.ToInt64()), typeof (StoreOperationMetadataProperty));
      Marshal.FreeCoTaskMem(rgItems);
    }

    [SecurityCritical]
    private static IntPtr MarshalProperties(StoreOperationMetadataProperty[] Props)
    {
      if (Props == null || Props.Length == 0)
        return IntPtr.Zero;
      int num1 = Marshal.SizeOf(typeof (StoreOperationMetadataProperty));
      IntPtr num2 = Marshal.AllocCoTaskMem(num1 * Props.Length);
      for (int index = 0; index != Props.Length; ++index)
        Marshal.StructureToPtr((object) Props[index], new IntPtr((long) (index * num1) + num2.ToInt64()), false);
      return num2;
    }

    [System.Flags]
    public enum OpFlags
    {
      Nothing = 0,
    }

    public enum Disposition
    {
      Failed = 0,
      Set = 2,
    }
  }
}
