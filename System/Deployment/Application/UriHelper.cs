// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.UriHelper
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.IO;
using System.Threading;

namespace System.Deployment.Application
{
  internal static class UriHelper
  {
    private static char[] _directorySeparators = new char[2]
    {
      Path.DirectorySeparatorChar,
      Path.AltDirectorySeparatorChar
    };
    private static object _invalidRelativePathChars;

    private static char[] InvalidRelativePathChars
    {
      get
      {
        if (UriHelper._invalidRelativePathChars == null)
        {
          char[] invalidPathChars = Path.GetInvalidPathChars();
          char[] chArray1 = new char[invalidPathChars.Length + 3];
          invalidPathChars.CopyTo((Array) chArray1, 0);
          int length = invalidPathChars.Length;
          char[] chArray2 = chArray1;
          int index1 = length;
          int num1 = 1;
          int num2 = index1 + num1;
          int volumeSeparatorChar = (int) Path.VolumeSeparatorChar;
          chArray2[index1] = (char) volumeSeparatorChar;
          char[] chArray3 = chArray1;
          int index2 = num2;
          int num3 = 1;
          int num4 = index2 + num3;
          int num5 = 42;
          chArray3[index2] = (char) num5;
          char[] chArray4 = chArray1;
          int index3 = num4;
          int num6 = 1;
          int num7 = index3 + num6;
          int num8 = 63;
          chArray4[index3] = (char) num8;
          Interlocked.CompareExchange(ref UriHelper._invalidRelativePathChars, (object) chArray1, (object) null);
        }
        return (char[]) UriHelper._invalidRelativePathChars;
      }
    }

    public static void ValidateSupportedScheme(Uri uri)
    {
      if (!UriHelper.IsSupportedScheme(uri))
        throw new InvalidDeploymentException(ExceptionTypes.UriSchemeNotSupported, Resources.GetString("Ex_NotSupportedUriScheme"));
    }

    public static void ValidateSupportedSchemeInArgument(Uri uri, string argumentName)
    {
      if (!UriHelper.IsSupportedScheme(uri))
        throw new ArgumentException(Resources.GetString("Ex_NotSupportedUriScheme"), argumentName);
    }

    public static bool IsSupportedScheme(Uri uri)
    {
      if (!(uri.Scheme == Uri.UriSchemeFile) && !(uri.Scheme == Uri.UriSchemeHttp))
        return uri.Scheme == Uri.UriSchemeHttps;
      return true;
    }

    public static Uri UriFromRelativeFilePath(Uri baseUri, string path)
    {
      if (!UriHelper.IsValidRelativeFilePath(path))
        throw new ArgumentException(Resources.GetString("Ex_InvalidRelativePath"));
      if (path.IndexOf('%') >= 0)
        path = path.Replace("%", Uri.HexEscape('%'));
      if (path.IndexOf('#') >= 0)
        path = path.Replace("#", Uri.HexEscape('#'));
      Uri uri = new Uri(baseUri, path);
      UriHelper.ValidateSupportedScheme(uri);
      return uri;
    }

    public static bool IsValidRelativeFilePath(string path)
    {
      if (path == null || path.Length == 0 || (path.IndexOfAny(UriHelper.InvalidRelativePathChars) >= 0 || Path.IsPathRooted(path)))
        return false;
      string str = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
      string fullPath = Path.GetFullPath(Path.Combine(Path.DirectorySeparatorChar.ToString(), str));
      string pathRoot = Path.GetPathRoot(fullPath);
      string strA = fullPath.Substring(pathRoot.Length);
      if (strA.Length > 0 && (int) strA[0] == 92)
        strA = strA.Substring(1);
      return string.Compare(strA, str, StringComparison.Ordinal) == 0;
    }

    public static string NormalizePathDirectorySeparators(string path)
    {
      if (path == null)
        return (string) null;
      return path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
    }

    public static bool PathContainDirectorySeparators(string path)
    {
      if (path == null)
        return false;
      return path.IndexOfAny(UriHelper._directorySeparators) >= 0;
    }
  }
}
