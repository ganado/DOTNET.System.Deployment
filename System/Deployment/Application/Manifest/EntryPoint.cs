// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.Manifest.EntryPoint
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Deployment.Internal.Isolation.Manifest;

namespace System.Deployment.Application.Manifest
{
  internal class EntryPoint
  {
    private readonly string _name;
    private readonly string _commandLineFile;
    private readonly string _commandLineParamater;
    private readonly DependentAssembly _dependentAssembly;
    private readonly bool _hostInBrowser;
    private readonly bool _customHostSpecified;
    private readonly bool _customUX;

    public DependentAssembly Assembly
    {
      get
      {
        return this._dependentAssembly;
      }
    }

    public string CommandFile
    {
      get
      {
        return this._commandLineFile;
      }
    }

    public bool HostInBrowser
    {
      get
      {
        return this._hostInBrowser;
      }
    }

    public bool CustomHostSpecified
    {
      get
      {
        return this._customHostSpecified;
      }
    }

    public bool CustomUX
    {
      get
      {
        return this._customUX;
      }
    }

    public string CommandParameters
    {
      get
      {
        return this._commandLineParamater;
      }
    }

    public EntryPoint(EntryPointEntry entryPointEntry, AssemblyManifest manifest)
    {
      this._name = entryPointEntry.Name;
      this._commandLineFile = entryPointEntry.CommandLine_File;
      this._commandLineParamater = entryPointEntry.CommandLine_Parameters;
      this._hostInBrowser = (entryPointEntry.Flags & 1U) > 0U;
      this._customHostSpecified = (entryPointEntry.Flags & 2U) > 0U;
      this._customUX = (entryPointEntry.Flags & 4U) > 0U;
      if (this._customHostSpecified)
        return;
      if (entryPointEntry.Identity != null)
        this._dependentAssembly = manifest.GetDependentAssemblyByIdentity(entryPointEntry.Identity);
      if (this._dependentAssembly == null)
        throw new InvalidDeploymentException(ExceptionTypes.ManifestParse, Resources.GetString("Ex_NoMatchingAssemblyForEntryPoint"));
    }
  }
}
