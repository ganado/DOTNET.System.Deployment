// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.GetManifestCompletedEventArgs
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.ComponentModel;
using System.IO;
using System.Xml;

namespace System.Deployment.Application
{
  /// <summary>Provides data for the <see cref="E:System.Deployment.Application.InPlaceHostingManager.GetManifestCompleted" /> event of <see cref="T:System.Deployment.Application.InPlaceHostingManager" />.</summary>
  public class GetManifestCompletedEventArgs : AsyncCompletedEventArgs
  {
    private ActivationDescription _activationDescription;
    private Version _version;
    private ApplicationIdentity _applicationIdentity;
    private DefinitionIdentity _subId;
    private bool _isCached;
    private string _name;
    private Uri _support;
    private string _logFilePath;
    private byte[] _rawApplicationManifest;
    private byte[] _rawDeploymentManifest;
    private ActivationContext _actContext;

    /// <summary>Gets a description of the ClickOnce application. </summary>
    /// <returns>An <see cref="T:System.ApplicationIdentity" /> object.</returns>
    public ApplicationIdentity ApplicationIdentity
    {
      get
      {
        this.RaiseExceptionIfNecessary();
        return this._applicationIdentity;
      }
    }

    /// <summary>Gets the version of the update for the ClickOnce application.</summary>
    /// <returns>A <see cref="T:System.Version" /> representing the version number contained within the downloaded manifest.</returns>
    public Version Version
    {
      get
      {
        this.RaiseExceptionIfNecessary();
        return this._version;
      }
    }

    /// <summary>Gets a value indicating whether this ClickOnce application is cached.</summary>
    /// <returns>true if the application is cached; otherwise, false.</returns>
    public bool IsCached
    {
      get
      {
        this.RaiseExceptionIfNecessary();
        return this._isCached;
      }
    }

    /// <summary>Gets the name of the ClickOnce application.</summary>
    /// <returns>A <see cref="T:System.String" /> representing the product name of the ClickOnce application, as stored in the assembly metadata of the application's main executable file.</returns>
    public string ProductName
    {
      get
      {
        this.RaiseExceptionIfNecessary();
        return this._name;
      }
    }

    /// <summary>Gets the location of a Web page users can visit to obtain product support for the ClickOnce application.</summary>
    /// <returns>A <see cref="T:System.Uri" /> containing the value found in the supportUrl attribute of the deployment manifest's &lt;description&gt; tag. For more information, see ClickOnce Deployment Manifest.</returns>
    public Uri SupportUri
    {
      get
      {
        this.RaiseExceptionIfNecessary();
        return this._support;
      }
    }

    /// <summary>Gets the location of the ClickOnce error log.</summary>
    /// <returns>A <see cref="T:System.String" /> containing the location of the ClickOnce error log.</returns>
    public string LogFilePath
    {
      get
      {
        return this._logFilePath;
      }
    }

    /// <summary>Gets the ClickOnce deployment manifest for this deployment.</summary>
    /// <returns>An <see cref="T:System.Xml.XmlReader" /> representing the deployment manifest.</returns>
    public XmlReader DeploymentManifest
    {
      get
      {
        this.RaiseExceptionIfNecessary();
        return GetManifestCompletedEventArgs.ManifestToXml(this.RawDeploymentManifest);
      }
    }

    /// <summary>Gets the ClickOnce application manifest for this deployment.</summary>
    /// <returns>An <see cref="T:System.Xml.XmlReader" /> representing the application manifest.</returns>
    public XmlReader ApplicationManifest
    {
      get
      {
        this.RaiseExceptionIfNecessary();
        return GetManifestCompletedEventArgs.ManifestToXml(this.RawApplicationManifest);
      }
    }

    /// <summary>Gets the context for the current ClickOnce application.</summary>
    /// <returns>An <see cref="T:System.ActivationContext" /> object representing the context for the current application.</returns>
    public ActivationContext ActivationContext
    {
      get
      {
        this.RaiseExceptionIfNecessary();
        return this._actContext;
      }
    }

    /// <summary>Gets a string identifying the subscription.</summary>
    /// <returns>A string with information identifying the subscription.</returns>
    public string SubscriptionIdentity
    {
      get
      {
        this.RaiseExceptionIfNecessary();
        return this._subId.ToString();
      }
    }

    private byte[] RawDeploymentManifest
    {
      get
      {
        if (this._rawDeploymentManifest == null)
          this._rawDeploymentManifest = this._activationDescription.DeployManifest.RawXmlBytes;
        return this._rawDeploymentManifest;
      }
    }

    private byte[] RawApplicationManifest
    {
      get
      {
        if (this._rawApplicationManifest == null)
          this._rawApplicationManifest = this._activationDescription.AppManifest.RawXmlBytes;
        return this._rawApplicationManifest;
      }
    }

    internal GetManifestCompletedEventArgs(BindCompletedEventArgs e, ActivationDescription activationDescription, string logFilePath, Logger.LogIdentity log)
      : base(e.Error, e.Cancelled, e.UserState)
    {
      this._applicationIdentity = e.ActivationContext != null ? e.ActivationContext.Identity : (ApplicationIdentity) null;
      Logger.AddInternalState(log, "Creating GetManifestCompletedEventArgs.");
      string text = this._applicationIdentity.ToString();
      this._subId = new DefinitionAppId(text).DeploymentIdentity.ToSubscriptionId();
      this._logFilePath = logFilePath;
      this._isCached = e.IsCached;
      this._name = e.FriendlyName;
      this._actContext = e.ActivationContext;
      Logger.AddInternalState(log, "Application identity=" + text);
      Logger.AddInternalState(log, "Subscription identity=" + (this._subId != null ? this._subId.ToString() : "null"));
      Logger.AddInternalState(log, "IsCached=" + this._isCached.ToString());
      if (this._isCached)
      {
        this._rawDeploymentManifest = e.ActivationContext.DeploymentManifestBytes;
        this._rawApplicationManifest = e.ActivationContext.ApplicationManifestBytes;
      }
      this._activationDescription = activationDescription;
      this._version = this._activationDescription.AppId.DeploymentIdentity.Version;
      this._support = this._activationDescription.DeployManifest.Description.SupportUri;
    }

    internal GetManifestCompletedEventArgs(BindCompletedEventArgs e, Exception error, string logFilePath)
      : base(error, e.Cancelled, e.UserState)
    {
      this._logFilePath = logFilePath;
    }

    internal GetManifestCompletedEventArgs(BindCompletedEventArgs e, string logFilePath)
      : base(e.Error, e.Cancelled, e.UserState)
    {
      this._logFilePath = logFilePath;
    }

    private static XmlReader ManifestToXml(byte[] rawManifest)
    {
      if (rawManifest == null)
        return (XmlReader) null;
      return (XmlReader) new XmlTextReader((Stream) new MemoryStream(rawManifest));
    }
  }
}
