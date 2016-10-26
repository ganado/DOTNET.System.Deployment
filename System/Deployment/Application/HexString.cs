// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.HexString
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Globalization;
using System.Text;

namespace System.Deployment.Application
{
  internal static class HexString
  {
    public static string FromBytes(byte[] bytes)
    {
      StringBuilder stringBuilder = new StringBuilder(bytes.Length * 2);
      for (int index = 0; index < bytes.Length; ++index)
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0:x2}", new object[1]
        {
          (object) bytes[index]
        });
      return stringBuilder.ToString();
    }
  }
}
