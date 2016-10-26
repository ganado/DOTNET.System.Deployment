// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.PlatformSpecific
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

namespace System.Deployment.Application
{
  internal static class PlatformSpecific
  {
    public static bool OnWin9x
    {
      get
      {
        return Environment.OSVersion.Platform == PlatformID.Win32Windows;
      }
    }

    public static bool OnWinMe
    {
      get
      {
        OperatingSystem osVersion = Environment.OSVersion;
        return osVersion.Platform == PlatformID.Win32Windows && osVersion.Version.Major == 4 && osVersion.Version.Minor == 90;
      }
    }

    public static bool OnXPOrAbove
    {
      get
      {
        OperatingSystem osVersion = Environment.OSVersion;
        if (osVersion.Platform != PlatformID.Win32NT)
          return false;
        if (osVersion.Version.Major != 5 || osVersion.Version.Minor < 1)
          return osVersion.Version.Major >= 6;
        return true;
      }
    }

    public static bool OnWindows2003
    {
      get
      {
        OperatingSystem osVersion = Environment.OSVersion;
        if (osVersion.Platform == PlatformID.Win32NT && osVersion.Version.Major == 5)
          return osVersion.Version.Minor == 2;
        return false;
      }
    }

    public static bool OnVistaOrAbove
    {
      get
      {
        OperatingSystem osVersion = Environment.OSVersion;
        if (osVersion.Platform == PlatformID.Win32NT)
          return osVersion.Version.Major >= 6;
        return false;
      }
    }
  }
}
