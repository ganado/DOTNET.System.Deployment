// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.Isolation.ApplicationContext
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Deployment.Internal.Isolation.Manifest;

namespace System.Deployment.Internal.Isolation
{
  internal class ApplicationContext
  {
    private IActContext _appcontext;

    public DefinitionAppId Identity
    {
      get
      {
        object AppId;
        this._appcontext.GetAppId(out AppId);
        return new DefinitionAppId(AppId as IDefinitionAppId);
      }
    }

    public string BasePath
    {
      get
      {
        string ApplicationPath;
        this._appcontext.ApplicationBasePath(0U, out ApplicationPath);
        return ApplicationPath;
      }
    }

    public EnumDefinitionIdentity Components
    {
      get
      {
        object ppIdentityEnum;
        this._appcontext.EnumComponents(0U, out ppIdentityEnum);
        return new EnumDefinitionIdentity(ppIdentityEnum as IEnumDefinitionIdentity);
      }
    }

    public string StateLocation
    {
      get
      {
        string ppszPath;
        this._appcontext.GetApplicationStateFilesystemLocation(0U, UIntPtr.Zero, IntPtr.Zero, out ppszPath);
        return ppszPath;
      }
    }

    internal ApplicationContext(IActContext a)
    {
      if (a == null)
        throw new ArgumentNullException();
      this._appcontext = a;
    }

    public ApplicationContext(DefinitionAppId appid)
    {
      if (appid == null)
        throw new ArgumentNullException();
      this._appcontext = IsolationInterop.CreateActContext(appid._id);
    }

    public ApplicationContext(ReferenceAppId appid)
    {
      if (appid == null)
        throw new ArgumentNullException();
      this._appcontext = IsolationInterop.CreateActContext(appid._id);
    }

    public string ReplaceStrings(string culture, string toreplace)
    {
      string Replaced;
      this._appcontext.ReplaceStringMacros(0U, culture, toreplace, out Replaced);
      return Replaced;
    }

    internal ICMS GetComponentManifest(DefinitionIdentity component)
    {
      object ManifestInteface;
      this._appcontext.GetComponentManifest(0U, component._id, ref IsolationInterop.IID_ICMS, out ManifestInteface);
      return ManifestInteface as ICMS;
    }

    internal string GetComponentManifestPath(DefinitionIdentity component)
    {
      object ManifestInteface;
      this._appcontext.GetComponentManifest(0U, component._id, ref IsolationInterop.IID_IManifestInformation, out ManifestInteface);
      string FullPath;
      ((IManifestInformation) ManifestInteface).get_FullPath(out FullPath);
      return FullPath;
    }

    public string GetComponentPath(DefinitionIdentity component)
    {
      string PayloadPath;
      this._appcontext.GetComponentPayloadPath(0U, component._id, out PayloadPath);
      return PayloadPath;
    }

    public DefinitionIdentity MatchReference(ReferenceIdentity TheRef)
    {
      object MatchedDefinition;
      this._appcontext.FindReferenceInContext(0U, TheRef._id, out MatchedDefinition);
      return new DefinitionIdentity(MatchedDefinition as IDefinitionIdentity);
    }

    public void PrepareForExecution()
    {
      this._appcontext.PrepareForExecution(IntPtr.Zero, IntPtr.Zero);
    }

    public ApplicationContext.ApplicationStateDisposition SetApplicationState(ApplicationContext.ApplicationState s)
    {
      uint ulDisposition;
      this._appcontext.SetApplicationRunningState(0U, (uint) s, out ulDisposition);
      return (ApplicationContext.ApplicationStateDisposition) ulDisposition;
    }

    public enum ApplicationState
    {
      Undefined,
      Starting,
      Running,
    }

    public enum ApplicationStateDisposition
    {
      Undefined = 0,
      Starting = 1,
      Running = 2,
      Starting_Migrated = 65537,
      Running_FirstTime = 131074,
    }
  }
}
