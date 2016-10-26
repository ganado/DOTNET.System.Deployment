// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.Hash
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Deployment.Internal.Isolation.Manifest;
using System.Globalization;

namespace System.Deployment.Application
{
  internal class Hash
  {
    private byte[] _digestValue;
    private CMS_HASH_DIGESTMETHOD _digestMethod;
    private CMS_HASH_TRANSFORM _transform;

    public byte[] DigestValue
    {
      get
      {
        return this._digestValue;
      }
    }

    public CMS_HASH_DIGESTMETHOD DigestMethod
    {
      get
      {
        return this._digestMethod;
      }
    }

    public CMS_HASH_TRANSFORM Transform
    {
      get
      {
        return this._transform;
      }
    }

    public string CompositString
    {
      get
      {
        return this.DigestMethodCodeString + this.TranformCodeString + HexString.FromBytes(this.DigestValue);
      }
    }

    protected string TranformCodeString
    {
      get
      {
        return Hash.ToCodedString((uint) this.Transform);
      }
    }

    protected string DigestMethodCodeString
    {
      get
      {
        return Hash.ToCodedString((uint) this.DigestMethod);
      }
    }

    public Hash(byte[] digestValue, CMS_HASH_DIGESTMETHOD digestMethod, CMS_HASH_TRANSFORM transform)
    {
      if (digestValue == null)
        throw new ArgumentException(Resources.GetString("Ex_HashNullDigestValue"));
      this._digestValue = digestValue;
      this._digestMethod = digestMethod;
      this._transform = transform;
    }

    protected static string ToCodedString(uint value)
    {
      if (value > (uint) byte.MaxValue)
        throw new ArgumentException(Resources.GetString("Ex_CodeLimitExceeded"));
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:x2}", new object[1]
      {
        (object) value
      });
    }
  }
}
