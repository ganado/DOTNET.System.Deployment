// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.Manifest.Description
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Deployment.Internal.Isolation.Manifest;

namespace System.Deployment.Application.Manifest
{
  internal class Description
  {
    private readonly string _publisher;
    private readonly string _product;
    private readonly string _suiteName;
    private readonly Uri _supportUri;
    private readonly Uri _errorReportUri;
    private readonly string _iconFile;
    private readonly string _iconFileFS;
    private readonly string _filteredPublisher;
    private readonly string _filteredProduct;
    private readonly string _filteredSuiteName;

    public string Publisher
    {
      get
      {
        return this._publisher;
      }
    }

    public string Product
    {
      get
      {
        return this._product;
      }
    }

    public Uri SupportUri
    {
      get
      {
        return this._supportUri;
      }
    }

    public string SupportUrl
    {
      get
      {
        if (!(this._supportUri != (Uri) null))
          return (string) null;
        return this._supportUri.AbsoluteUri;
      }
    }

    public string IconFile
    {
      get
      {
        return this._iconFile;
      }
    }

    public string IconFileFS
    {
      get
      {
        return this._iconFileFS;
      }
    }

    public Uri ErrorReportUri
    {
      get
      {
        return this._errorReportUri;
      }
    }

    public string ErrorReportUrl
    {
      get
      {
        if (!(this._errorReportUri != (Uri) null))
          return (string) null;
        return this._errorReportUri.AbsoluteUri;
      }
    }

    public string FilteredPublisher
    {
      get
      {
        return this._filteredPublisher;
      }
    }

    public string FilteredProduct
    {
      get
      {
        return this._filteredProduct;
      }
    }

    public string FilteredSuiteName
    {
      get
      {
        return this._filteredSuiteName;
      }
    }

    public Description(DescriptionMetadataEntry descriptionMetadataEntry)
    {
      this._publisher = descriptionMetadataEntry.Publisher;
      this._product = descriptionMetadataEntry.Product;
      this._suiteName = descriptionMetadataEntry.SuiteName;
      if (this._suiteName == null)
        this._suiteName = "";
      this._supportUri = AssemblyManifest.UriFromMetadataEntry(descriptionMetadataEntry.SupportUrl, "Ex_DescriptionSupportUrlNotValid");
      this._errorReportUri = AssemblyManifest.UriFromMetadataEntry(descriptionMetadataEntry.ErrorReportUrl, "Ex_DescriptionErrorReportUrlNotValid");
      this._iconFile = descriptionMetadataEntry.IconFile;
      if (this._iconFile != null)
        this._iconFileFS = System.Deployment.Application.UriHelper.NormalizePathDirectorySeparators(this._iconFile);
      this._filteredPublisher = PathTwiddler.FilterString(this._publisher, ' ', false);
      this._filteredProduct = PathTwiddler.FilterString(this._product, ' ', false);
      this._filteredSuiteName = PathTwiddler.FilterString(this._suiteName, ' ', false);
    }
  }
}
