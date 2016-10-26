// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.Manifest.FileAssociation
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Deployment.Internal.Isolation.Manifest;
using System.Text;

namespace System.Deployment.Application.Manifest
{
  internal class FileAssociation
  {
    private readonly string _extension;
    private readonly string _description;
    private readonly string _progId;
    private readonly string _defaultIcon;
    private readonly string _parameter;

    public string Extension
    {
      get
      {
        return this._extension;
      }
    }

    public string Description
    {
      get
      {
        return this._description;
      }
    }

    public string ProgID
    {
      get
      {
        return this._progId;
      }
    }

    public string DefaultIcon
    {
      get
      {
        return this._defaultIcon;
      }
    }

    public string Parameter
    {
      get
      {
        return this._parameter;
      }
    }

    public FileAssociation(FileAssociationEntry fileAssociationEntry)
    {
      this._extension = fileAssociationEntry.Extension;
      this._description = fileAssociationEntry.Description;
      this._progId = fileAssociationEntry.ProgID;
      this._defaultIcon = fileAssociationEntry.DefaultIcon;
      this._parameter = fileAssociationEntry.Parameter;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("(" + this._extension + ",");
      stringBuilder.Append(this._description + ",");
      stringBuilder.Append(this._progId + ",");
      stringBuilder.Append(this._defaultIcon + ",");
      stringBuilder.Append(this._parameter + ")");
      return stringBuilder.ToString();
    }
  }
}
