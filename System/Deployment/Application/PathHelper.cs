// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.PathHelper
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace System.Deployment.Application
{
  internal static class PathHelper
  {
    private const int MAX_PATH = 260;
    private const int ERROR_FILE_NOT_FOUND = 2;
    private const int ERROR_INVALID_PARAMETER = 87;
    private static object _shortShimDllPath;

    public static string ShortShimDllPath
    {
      get
      {
        if (PathHelper._shortShimDllPath == null)
        {
          string longPath = Path.Combine(Environment.SystemDirectory, "dfshim.dll");
          Interlocked.CompareExchange(ref PathHelper._shortShimDllPath, (object) PathHelper.GetShortPath(longPath), (object) null);
        }
        return (string) PathHelper._shortShimDllPath;
      }
    }

    public static string GetShortPath(string longPath)
    {
      StringBuilder ShortPath = new StringBuilder(260);
      int shortPathName = NativeMethods.GetShortPathName(longPath, ShortPath, ShortPath.Capacity);
      if (shortPathName == 0)
        PathHelper.GetShortPathNameThrowExceptionForLastError(longPath);
      if (shortPathName >= ShortPath.Capacity)
      {
        ShortPath.Capacity = shortPathName + 1;
        if (NativeMethods.GetShortPathName(longPath, ShortPath, ShortPath.Capacity) == 0)
          PathHelper.GetShortPathNameThrowExceptionForLastError(longPath);
      }
      return ShortPath.ToString();
    }

    public static string GenerateRandomPath(uint segmentCount)
    {
      if ((int) segmentCount == 0)
        return (string) null;
      uint num = 11U * segmentCount;
      byte[] numArray = new byte[(int) (uint) Math.Ceiling((double) num * 0.625)];
      new RNGCryptoServiceProvider().GetBytes(numArray);
      string str = Base32String.FromBytes(numArray);
      if ((long) str.Length < (long) num)
        throw new DeploymentException(Resources.GetString("Ex_TempPathRandomStringTooShort"));
      if (str.IndexOf('\\') >= 0)
        throw new DeploymentException(Resources.GetString("Ex_TempPathRandomStringInvalid"));
      for (int index = (int) segmentCount - 1; index > 0; --index)
      {
        int startIndex = index * 11;
        if (startIndex >= str.Length)
          throw new DeploymentException(Resources.GetString("Ex_TempPathRandomStringInvalid"));
        str = str.Insert(startIndex, "\\");
      }
      string[] strArray = str.Split('\\');
      if ((long) strArray.Length < (long) segmentCount)
        throw new DeploymentException(Resources.GetString("Ex_TempPathRandomStringInvalid"));
      string path1 = (string) null;
      for (uint index = 0; index < segmentCount; ++index)
      {
        if (strArray[(int) index].Length < 11)
          throw new DeploymentException(Resources.GetString("Ex_TempPathRandomStringInvalid"));
        string path2 = strArray[(int) index].Substring(0, 11).Insert(8, ".");
        path1 = path1 != null ? Path.Combine(path1, path2) : path2;
      }
      return path1;
    }

    public static string GetRootSegmentPath(string path, uint segmentCount)
    {
      if ((int) segmentCount == 0)
        throw new ArgumentException("segmentCount");
      if ((int) segmentCount == 1)
        return path;
      return PathHelper.GetRootSegmentPath(Path.GetDirectoryName(path), segmentCount - 1U);
    }

    private static void GetShortPathNameThrowExceptionForLastError(string path)
    {
      int lastWin32Error = Marshal.GetLastWin32Error();
      switch (lastWin32Error)
      {
        case 2:
          throw new FileNotFoundException(path);
        case 87:
          throw new InvalidOperationException(Resources.GetString("Ex_ShortFileNameNotSupported"));
        default:
          throw new Win32Exception(lastWin32Error);
      }
    }
  }
}
