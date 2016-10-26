// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.Store
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Deployment.Internal.Isolation.Manifest;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation
{
  internal class Store
  {
    private IStore _pStore;

    public IStore InternalStore
    {
      get
      {
        return this._pStore;
      }
    }

    public Store(IStore pStore)
    {
      if (pStore == null)
        throw new ArgumentNullException("pStore");
      this._pStore = pStore;
    }

    [SecuritySafeCritical]
    public uint[] Transact(StoreTransactionOperation[] operations)
    {
      if (operations == null || operations.Length == 0)
        throw new ArgumentException("operations");
      uint[] rgDispositions = new uint[operations.Length];
      int[] rgResults = new int[operations.Length];
      this._pStore.Transact(new IntPtr(operations.Length), operations, rgDispositions, rgResults);
      return rgDispositions;
    }

    public void Transact(StoreTransactionOperation[] operations, uint[] rgDispositions, int[] rgResults)
    {
      if (operations == null || operations.Length == 0)
        throw new ArgumentException("operations");
      this._pStore.Transact(new IntPtr(operations.Length), operations, rgDispositions, rgResults);
    }

    [SecuritySafeCritical]
    public IDefinitionIdentity BindReferenceToAssemblyIdentity(uint Flags, IReferenceIdentity ReferenceIdentity, uint cDeploymentsToIgnore, IDefinitionIdentity[] DefinitionIdentity_DeploymentsToIgnore)
    {
      Guid idefinitionIdentity = IsolationInterop.IID_IDefinitionIdentity;
      return (IDefinitionIdentity) this._pStore.BindReferenceToAssembly(Flags, ReferenceIdentity, cDeploymentsToIgnore, DefinitionIdentity_DeploymentsToIgnore, ref idefinitionIdentity);
    }

    [SecuritySafeCritical]
    public void CalculateDelimiterOfDeploymentsBasedOnQuota(uint dwFlags, uint cDeployments, IDefinitionAppId[] rgpIDefinitionAppId_Deployments, ref StoreApplicationReference InstallerReference, ulong ulonglongQuota, ref uint Delimiter, ref ulong SizeSharedWithExternalDeployment, ref ulong SizeConsumedByInputDeploymentArray)
    {
      IntPtr Delimiter1 = IntPtr.Zero;
      this._pStore.CalculateDelimiterOfDeploymentsBasedOnQuota(dwFlags, new IntPtr((long) cDeployments), rgpIDefinitionAppId_Deployments, ref InstallerReference, ulonglongQuota, out Delimiter1, out SizeSharedWithExternalDeployment, out SizeConsumedByInputDeploymentArray);
      Delimiter = (uint) Delimiter1.ToInt64();
    }

    [SecuritySafeCritical]
    public ICMS BindReferenceToAssemblyManifest(uint Flags, IReferenceIdentity ReferenceIdentity, uint cDeploymentsToIgnore, IDefinitionIdentity[] DefinitionIdentity_DeploymentsToIgnore)
    {
      Guid iidIcms = IsolationInterop.IID_ICMS;
      return (ICMS) this._pStore.BindReferenceToAssembly(Flags, ReferenceIdentity, cDeploymentsToIgnore, DefinitionIdentity_DeploymentsToIgnore, ref iidIcms);
    }

    [SecuritySafeCritical]
    public ICMS GetAssemblyManifest(uint Flags, IDefinitionIdentity DefinitionIdentity)
    {
      Guid iidIcms = IsolationInterop.IID_ICMS;
      return (ICMS) this._pStore.GetAssemblyInformation(Flags, DefinitionIdentity, ref iidIcms);
    }

    [SecuritySafeCritical]
    public IDefinitionIdentity GetAssemblyIdentity(uint Flags, IDefinitionIdentity DefinitionIdentity)
    {
      Guid idefinitionIdentity = IsolationInterop.IID_IDefinitionIdentity;
      return (IDefinitionIdentity) this._pStore.GetAssemblyInformation(Flags, DefinitionIdentity, ref idefinitionIdentity);
    }

    public StoreAssemblyEnumeration EnumAssemblies(Store.EnumAssembliesFlags Flags)
    {
      return this.EnumAssemblies(Flags, (IReferenceIdentity) null);
    }

    [SecuritySafeCritical]
    public StoreAssemblyEnumeration EnumAssemblies(Store.EnumAssembliesFlags Flags, IReferenceIdentity refToMatch)
    {
      Guid guidOfType = IsolationInterop.GetGuidOfType(typeof (IEnumSTORE_ASSEMBLY));
      return new StoreAssemblyEnumeration((IEnumSTORE_ASSEMBLY) this._pStore.EnumAssemblies((uint) Flags, refToMatch, ref guidOfType));
    }

    [SecuritySafeCritical]
    public StoreAssemblyFileEnumeration EnumFiles(Store.EnumAssemblyFilesFlags Flags, IDefinitionIdentity Assembly)
    {
      Guid guidOfType = IsolationInterop.GetGuidOfType(typeof (IEnumSTORE_ASSEMBLY_FILE));
      return new StoreAssemblyFileEnumeration((IEnumSTORE_ASSEMBLY_FILE) this._pStore.EnumFiles((uint) Flags, Assembly, ref guidOfType));
    }

    [SecuritySafeCritical]
    public StoreAssemblyFileEnumeration EnumPrivateFiles(Store.EnumApplicationPrivateFiles Flags, IDefinitionAppId Application, IDefinitionIdentity Assembly)
    {
      Guid guidOfType = IsolationInterop.GetGuidOfType(typeof (IEnumSTORE_ASSEMBLY_FILE));
      return new StoreAssemblyFileEnumeration((IEnumSTORE_ASSEMBLY_FILE) this._pStore.EnumPrivateFiles((uint) Flags, Application, Assembly, ref guidOfType));
    }

    [SecuritySafeCritical]
    public IEnumSTORE_ASSEMBLY_INSTALLATION_REFERENCE EnumInstallationReferences(Store.EnumAssemblyInstallReferenceFlags Flags, IDefinitionIdentity Assembly)
    {
      Guid guidOfType = IsolationInterop.GetGuidOfType(typeof (IEnumSTORE_ASSEMBLY_INSTALLATION_REFERENCE));
      return (IEnumSTORE_ASSEMBLY_INSTALLATION_REFERENCE) this._pStore.EnumInstallationReferences((uint) Flags, Assembly, ref guidOfType);
    }

    [SecuritySafeCritical]
    public Store.IPathLock LockAssemblyPath(IDefinitionIdentity asm)
    {
      IntPtr Cookie;
      string path = this._pStore.LockAssemblyPath(0U, asm, out Cookie);
      return (Store.IPathLock) new Store.AssemblyPathLock(this._pStore, Cookie, path);
    }

    [SecuritySafeCritical]
    public Store.IPathLock LockApplicationPath(IDefinitionAppId app)
    {
      IntPtr Cookie;
      string path = this._pStore.LockApplicationPath(0U, app, out Cookie);
      return (Store.IPathLock) new Store.ApplicationPathLock(this._pStore, Cookie, path);
    }

    [SecuritySafeCritical]
    public ulong QueryChangeID(IDefinitionIdentity asm)
    {
      return this._pStore.QueryChangeID(asm);
    }

    [SecuritySafeCritical]
    public StoreCategoryEnumeration EnumCategories(Store.EnumCategoriesFlags Flags, IReferenceIdentity CategoryMatch)
    {
      Guid guidOfType = IsolationInterop.GetGuidOfType(typeof (IEnumSTORE_CATEGORY));
      return new StoreCategoryEnumeration((IEnumSTORE_CATEGORY) this._pStore.EnumCategories((uint) Flags, CategoryMatch, ref guidOfType));
    }

    public StoreSubcategoryEnumeration EnumSubcategories(Store.EnumSubcategoriesFlags Flags, IDefinitionIdentity CategoryMatch)
    {
      return this.EnumSubcategories(Flags, CategoryMatch, (string) null);
    }

    [SecuritySafeCritical]
    public StoreSubcategoryEnumeration EnumSubcategories(Store.EnumSubcategoriesFlags Flags, IDefinitionIdentity Category, string SearchPattern)
    {
      Guid guidOfType = IsolationInterop.GetGuidOfType(typeof (IEnumSTORE_CATEGORY_SUBCATEGORY));
      return new StoreSubcategoryEnumeration((IEnumSTORE_CATEGORY_SUBCATEGORY) this._pStore.EnumSubcategories((uint) Flags, Category, SearchPattern, ref guidOfType));
    }

    [SecuritySafeCritical]
    public StoreCategoryInstanceEnumeration EnumCategoryInstances(Store.EnumCategoryInstancesFlags Flags, IDefinitionIdentity Category, string SubCat)
    {
      Guid guidOfType = IsolationInterop.GetGuidOfType(typeof (IEnumSTORE_CATEGORY_INSTANCE));
      return new StoreCategoryInstanceEnumeration((IEnumSTORE_CATEGORY_INSTANCE) this._pStore.EnumCategoryInstances((uint) Flags, Category, SubCat, ref guidOfType));
    }

    [SecurityCritical]
    public byte[] GetDeploymentProperty(Store.GetPackagePropertyFlags Flags, IDefinitionAppId Deployment, StoreApplicationReference Reference, Guid PropertySet, string PropertyName)
    {
      BLOB blob = new BLOB();
      byte[] destination = (byte[]) null;
      try
      {
        this._pStore.GetDeploymentProperty((uint) Flags, Deployment, ref Reference, ref PropertySet, PropertyName, out blob);
        destination = new byte[(int) blob.Size];
        Marshal.Copy(blob.BlobData, destination, 0, (int) blob.Size);
      }
      finally
      {
        blob.Dispose();
      }
      return destination;
    }

    [SecuritySafeCritical]
    public StoreDeploymentMetadataEnumeration EnumInstallerDeployments(Guid InstallerId, string InstallerName, string InstallerMetadata, IReferenceAppId DeploymentFilter)
    {
      StoreApplicationReference Reference = new StoreApplicationReference(InstallerId, InstallerName, InstallerMetadata);
      return new StoreDeploymentMetadataEnumeration((IEnumSTORE_DEPLOYMENT_METADATA) this._pStore.EnumInstallerDeploymentMetadata(0U, ref Reference, DeploymentFilter, ref IsolationInterop.IID_IEnumSTORE_DEPLOYMENT_METADATA));
    }

    [SecuritySafeCritical]
    public StoreDeploymentMetadataPropertyEnumeration EnumInstallerDeploymentProperties(Guid InstallerId, string InstallerName, string InstallerMetadata, IDefinitionAppId Deployment)
    {
      StoreApplicationReference Reference = new StoreApplicationReference(InstallerId, InstallerName, InstallerMetadata);
      return new StoreDeploymentMetadataPropertyEnumeration((IEnumSTORE_DEPLOYMENT_METADATA_PROPERTY) this._pStore.EnumInstallerDeploymentMetadataProperties(0U, ref Reference, Deployment, ref IsolationInterop.IID_IEnumSTORE_DEPLOYMENT_METADATA_PROPERTY));
    }

    [Flags]
    public enum EnumAssembliesFlags
    {
      Nothing = 0,
      VisibleOnly = 1,
      MatchServicing = 2,
      ForceLibrarySemantics = 4,
    }

    [Flags]
    public enum EnumAssemblyFilesFlags
    {
      Nothing = 0,
      IncludeInstalled = 1,
      IncludeMissing = 2,
    }

    [Flags]
    public enum EnumApplicationPrivateFiles
    {
      Nothing = 0,
      IncludeInstalled = 1,
      IncludeMissing = 2,
    }

    [Flags]
    public enum EnumAssemblyInstallReferenceFlags
    {
      Nothing = 0,
    }

    public interface IPathLock : IDisposable
    {
      string Path { get; }
    }

    private class AssemblyPathLock : Store.IPathLock, IDisposable
    {
      private IntPtr _pLockCookie = IntPtr.Zero;
      private IStore _pSourceStore;
      private string _path;

      public string Path
      {
        get
        {
          return this._path;
        }
      }

      public AssemblyPathLock(IStore s, IntPtr c, string path)
      {
        this._pSourceStore = s;
        this._pLockCookie = c;
        this._path = path;
      }

      ~AssemblyPathLock()
      {
        this.Dispose(false);
      }

      [SecuritySafeCritical]
      private void Dispose(bool fDisposing)
      {
        if (fDisposing)
          GC.SuppressFinalize((object) this);
        if (!(this._pLockCookie != IntPtr.Zero))
          return;
        this._pSourceStore.ReleaseAssemblyPath(this._pLockCookie);
        this._pLockCookie = IntPtr.Zero;
      }

      void IDisposable.Dispose()
      {
        this.Dispose(true);
      }
    }

    private class ApplicationPathLock : Store.IPathLock, IDisposable
    {
      private IntPtr _pLockCookie = IntPtr.Zero;
      private IStore _pSourceStore;
      private string _path;

      public string Path
      {
        get
        {
          return this._path;
        }
      }

      public ApplicationPathLock(IStore s, IntPtr c, string path)
      {
        this._pSourceStore = s;
        this._pLockCookie = c;
        this._path = path;
      }

      ~ApplicationPathLock()
      {
        this.Dispose(false);
      }

      [SecuritySafeCritical]
      private void Dispose(bool fDisposing)
      {
        if (fDisposing)
          GC.SuppressFinalize((object) this);
        if (!(this._pLockCookie != IntPtr.Zero))
          return;
        this._pSourceStore.ReleaseApplicationPath(this._pLockCookie);
        this._pLockCookie = IntPtr.Zero;
      }

      void IDisposable.Dispose()
      {
        this.Dispose(true);
      }
    }

    [Flags]
    public enum EnumCategoriesFlags
    {
      Nothing = 0,
    }

    [Flags]
    public enum EnumSubcategoriesFlags
    {
      Nothing = 0,
    }

    [Flags]
    public enum EnumCategoryInstancesFlags
    {
      Nothing = 0,
    }

    [Flags]
    public enum GetPackagePropertyFlags
    {
      Nothing = 0,
    }
  }
}
