// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.DownloadOptions
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Globalization;

namespace System.Deployment.Application
{
  internal class DownloadOptions
  {
    public bool Background;
    public bool EnforceSizeLimit;
    public ulong SizeLimit;
    public ulong Size;

    public override string ToString()
    {
      return " Background = " + this.Background.ToString() + " EnforceSizeLimit = " + this.EnforceSizeLimit.ToString() + " SizeLimit =" + this.SizeLimit.ToString((IFormatProvider) CultureInfo.InvariantCulture) + " Size =" + this.Size.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }
  }
}
