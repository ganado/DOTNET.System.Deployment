// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.PathTwiddler
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.IO;
using System.Threading;

namespace System.Deployment.Application
{
  internal static class PathTwiddler
  {
    private static object _invalidFileDirNameChars;

    private static char[] InvalidFileDirNameChars
    {
      get
      {
        if (PathTwiddler._invalidFileDirNameChars == null)
          Interlocked.CompareExchange(ref PathTwiddler._invalidFileDirNameChars, (object) Path.GetInvalidFileNameChars(), (object) null);
        return (char[]) PathTwiddler._invalidFileDirNameChars;
      }
    }

    public static string FilterString(string input, char chReplace, bool fMultiReplace)
    {
      return PathTwiddler.FilterString(input, PathTwiddler.InvalidFileDirNameChars, chReplace, fMultiReplace);
    }

    private static string FilterString(string input, char[] toFilter, char chReplacement, bool fMultiReplace)
    {
      int length = 0;
      bool flag1 = false;
      bool flag2 = false;
      if (input == null)
        return (string) null;
      char[] charArray = input.ToCharArray();
      char[] chArray = new char[charArray.Length];
      Array.Sort<char>(toFilter);
      for (int index = 0; index < charArray.Length; ++index)
      {
        if (Array.BinarySearch<char>(toFilter, charArray[index]) < 0)
        {
          chArray[length++] = charArray[index];
          flag2 = true;
          if (flag1)
            flag1 = false;
        }
        else if (fMultiReplace || !flag1)
        {
          chArray[length++] = chReplacement;
          flag1 = true;
        }
      }
      if (!flag2 || length <= 0)
        return (string) null;
      return new string(chArray, 0, length);
    }
  }
}
