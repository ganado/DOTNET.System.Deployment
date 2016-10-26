// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.Utilities
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Policy;

namespace System.Deployment.Application
{
  internal static class Utilities
  {
    public static int CompareWithNullEqEmpty(string s1, string s2, StringComparison comparisonType)
    {
      if (string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2))
        return 0;
      return string.Compare(s1, s2, comparisonType);
    }

    public static bool DoesRegistryKeyExist(RegistryKey regRoot, string regKey)
    {
      bool flag = false;
      using (RegistryKey registryKey = regRoot.OpenSubKey(regKey, false))
      {
        if (registryKey != null)
          flag = true;
      }
      return flag;
    }

    public static string BuildTFM(string targetVersion, string profile)
    {
      return string.IsNullOrEmpty(profile) || "Full".Equals(profile, StringComparison.OrdinalIgnoreCase) ? string.Format(".NETFramework,Version=v{0}", (object) targetVersion) : string.Format(".NETFramework,Version=v{0},Profile={1}", (object) targetVersion, (object) profile);
    }

    public static void SetMarkOfTheWeb(string path)
    {
      if (!File.Exists(path))
        return;
      string lpFileName = path + ":Zone.Identifier";
      SafeFileHandle handle = (SafeFileHandle) null;
      Stream stream = (Stream) null;
      TextWriter textWriter = (TextWriter) null;
      try
      {
        handle = NativeMethods.CreateFile(lpFileName, 1073741824U, 2U, IntPtr.Zero, 2U, 0U, IntPtr.Zero);
        if (!handle.IsInvalid)
        {
          stream = (Stream) new FileStream(handle, FileAccess.Write);
          textWriter = (TextWriter) new StreamWriter(stream);
          textWriter.WriteLine("[ZoneTransfer]");
          textWriter.WriteLine("ZoneId=3");
          Logger.AddInternalState(string.Format("Set \"MOTW\" for file: {0}", (object) path));
        }
        else
          Logger.AddWarningInformation(string.Format("Failed to create alternate file stream, with error {0}", (object) Marshal.GetLastWin32Error()));
      }
      catch (Exception ex)
      {
        Logger.AddWarningInformation(string.Format("Failed to set \"MOTW\" for file: {0}", (object) path));
      }
      finally
      {
        if (textWriter != null)
          textWriter.Close();
        if (stream != null)
          stream.Close();
        if (handle != null)
          handle.Close();
      }
    }

    public static bool IsAppRepCheckRequired(string url)
    {
      if (Zone.CreateFromUrl(url).SecurityZone != SecurityZone.Internet)
        return false;
      Logger.AddInternalState("AppRep is required.");
      return true;
    }
  }
}
