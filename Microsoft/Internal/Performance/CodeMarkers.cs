// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.Performance.CodeMarkers
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Internal.Performance
{
  internal sealed class CodeMarkers
  {
    public static readonly CodeMarkers Instance = new CodeMarkers();
    private const string AtomName = "VSCodeMarkersEnabled";
    private const string DllName = "Microsoft.Internal.Performance.CodeMarkers.dll";
    private CodeMarkers.State state;

    public bool IsEnabled
    {
      get
      {
        return this.state == CodeMarkers.State.Enabled;
      }
    }

    private CodeMarkers()
    {
      this.state = (int) CodeMarkers.NativeMethods.FindAtom("VSCodeMarkersEnabled") != 0 ? CodeMarkers.State.Enabled : CodeMarkers.State.Disabled;
    }

    public bool CodeMarker(int nTimerID)
    {
      if (!this.IsEnabled)
        return false;
      try
      {
        CodeMarkers.NativeMethods.DllPerfCodeMarker(nTimerID, (byte[]) null, 0);
      }
      catch (DllNotFoundException ex)
      {
        this.state = CodeMarkers.State.DisabledDueToDllImportException;
        return false;
      }
      return true;
    }

    public bool CodeMarkerEx(int nTimerID, byte[] aBuff)
    {
      if (!this.IsEnabled)
        return false;
      if (aBuff == null)
        throw new ArgumentNullException("aBuff");
      try
      {
        CodeMarkers.NativeMethods.DllPerfCodeMarker(nTimerID, aBuff, aBuff.Length);
      }
      catch (DllNotFoundException ex)
      {
        this.state = CodeMarkers.State.DisabledDueToDllImportException;
        return false;
      }
      return true;
    }

    public bool CodeMarkerEx(int nTimerID, Guid guidData)
    {
      return this.CodeMarkerEx(nTimerID, guidData.ToByteArray());
    }

    public bool CodeMarkerEx(int nTimerID, string stringData)
    {
      return this.CodeMarkerEx(nTimerID, Encoding.Unicode.GetBytes(stringData));
    }

    public bool CodeMarkerEx(int nTimerID, uint uintData)
    {
      return this.CodeMarkerEx(nTimerID, BitConverter.GetBytes(uintData));
    }

    public bool CodeMarkerEx(int nTimerID, ulong ulongData)
    {
      return this.CodeMarkerEx(nTimerID, BitConverter.GetBytes(ulongData));
    }

    public bool InitPerformanceDll(int iApp, string strRegRoot)
    {
      return this.InitPerformanceDll(iApp, strRegRoot, RegistryView.Default);
    }

    public bool InitPerformanceDll(int iApp, string strRegRoot, RegistryView registryView)
    {
      if (this.IsEnabled)
        return true;
      if (!CodeMarkers.UseCodeMarkers(strRegRoot, registryView))
      {
        this.state = CodeMarkers.State.DisabledViaRegistryCheck;
        return false;
      }
      try
      {
        int num = (int) CodeMarkers.NativeMethods.AddAtom("VSCodeMarkersEnabled");
        CodeMarkers.NativeMethods.DllInitPerf(iApp);
        this.state = CodeMarkers.State.Enabled;
      }
      catch (DllNotFoundException ex)
      {
        this.state = CodeMarkers.State.DisabledDueToDllImportException;
        return false;
      }
      return true;
    }

    private static bool UseCodeMarkers(string regRoot, RegistryView registryView)
    {
      if (regRoot == null)
        throw new ArgumentNullException("regRoot");
      using (RegistryKey registryKey1 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
      {
        using (RegistryKey registryKey2 = registryKey1.OpenSubKey(regRoot + "\\Performance"))
        {
          if (registryKey2 != null)
            return !string.IsNullOrEmpty(registryKey2.GetValue(string.Empty).ToString());
        }
      }
      return false;
    }

    public void UninitializePerformanceDLL(int iApp)
    {
      if (!this.IsEnabled)
        return;
      this.state = CodeMarkers.State.Disabled;
      ushort atom = CodeMarkers.NativeMethods.FindAtom("VSCodeMarkersEnabled");
      if ((int) atom != 0)
      {
        int num = (int) CodeMarkers.NativeMethods.DeleteAtom(atom);
      }
      try
      {
        CodeMarkers.NativeMethods.DllUnInitPerf(iApp);
      }
      catch (DllNotFoundException ex)
      {
      }
    }

    private static class NativeMethods
    {
      [DllImport("Microsoft.Internal.Performance.CodeMarkers.dll", EntryPoint = "InitPerf")]
      public static extern void DllInitPerf(int iApp);

      [DllImport("Microsoft.Internal.Performance.CodeMarkers.dll", EntryPoint = "UnInitPerf")]
      public static extern void DllUnInitPerf(int iApp);

      [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
      public static extern ushort AddAtom([MarshalAs(UnmanagedType.LPWStr)] string lpString);

      [DllImport("kernel32.dll")]
      public static extern ushort DeleteAtom(ushort atom);

      [DllImport("Microsoft.Internal.Performance.CodeMarkers.dll", EntryPoint = "PerfCodeMarker")]
      public static extern void DllPerfCodeMarker(int nTimerID, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] aUserParams, int cbParams);

      [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
      public static extern ushort FindAtom([MarshalAs(UnmanagedType.LPWStr)] string lpString);
    }

    private enum State
    {
      Enabled,
      Disabled,
      DisabledDueToDllImportException,
      DisabledViaRegistryCheck,
    }
  }
}
