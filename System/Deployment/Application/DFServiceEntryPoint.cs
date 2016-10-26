// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.DFServiceEntryPoint
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using Microsoft.Win32;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace System.Deployment.Application
{
  internal static class DFServiceEntryPoint
  {
    private static DFServiceEntryPoint.CreateDeploymentServiceComDelegate s_createDeploymentServiceComDelegate;
    private static DFServiceEntryPoint.RegisterDeploymentServiceComDelegate RegisterDeploymentServiceCom;
    private static DFServiceEntryPoint.UnregisterDeploymentServiceComDelegate UnregisterDeploymentServiceCom;
    private static IntPtr DfdllHandle;
    private static DFServiceEntryPoint.DfsvcForm _dfsvcForm;

    private static void MessageLoopThread()
    {
      if (DFServiceEntryPoint._dfsvcForm != null)
        return;
      DFServiceEntryPoint._dfsvcForm = new DFServiceEntryPoint.DfsvcForm();
      SystemEvents.SessionEnded += new SessionEndedEventHandler(DFServiceEntryPoint._dfsvcForm.SessionEndedEventHandler);
      SystemEvents.SessionEnding += new SessionEndingEventHandler(DFServiceEntryPoint._dfsvcForm.SessionEndingEventHandler);
      System.Windows.Forms.Application.Run((Form) DFServiceEntryPoint._dfsvcForm);
    }

    private static object GetMethodDelegate(IntPtr handle, string methodName, System.Type methodDelegateType)
    {
      IntPtr procAddress = NativeMethods.GetProcAddress(handle, methodName);
      if (procAddress == IntPtr.Zero)
        throw new Win32Exception(Marshal.GetLastWin32Error());
      return (object) Marshal.GetDelegateForFunctionPointer(procAddress, methodDelegateType);
    }

    private static void ObtainDfdllExports()
    {
      DFServiceEntryPoint.DfdllHandle = NativeMethods.LoadLibrary(Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "dfdll.dll"));
      if (DFServiceEntryPoint.DfdllHandle == IntPtr.Zero)
        throw new Win32Exception(Marshal.GetLastWin32Error());
      DFServiceEntryPoint.RegisterDeploymentServiceCom = (DFServiceEntryPoint.RegisterDeploymentServiceComDelegate) DFServiceEntryPoint.GetMethodDelegate(DFServiceEntryPoint.DfdllHandle, "RegisterDeploymentServiceCom", typeof (DFServiceEntryPoint.RegisterDeploymentServiceComDelegate));
      DFServiceEntryPoint.UnregisterDeploymentServiceCom = (DFServiceEntryPoint.UnregisterDeploymentServiceComDelegate) DFServiceEntryPoint.GetMethodDelegate(DFServiceEntryPoint.DfdllHandle, "UnregisterDeploymentServiceCom", typeof (DFServiceEntryPoint.UnregisterDeploymentServiceComDelegate));
    }

    public static void Initialize(string[] args)
    {
      CodeMarker_Singleton.Instance.CodeMarker(523);
      if (PlatformSpecific.OnWin9x)
        new Thread(new ThreadStart(DFServiceEntryPoint.MessageLoopThread)).Start();
      DFServiceEntryPoint.ObtainDfdllExports();
      DFServiceEntryPoint.s_createDeploymentServiceComDelegate = (DFServiceEntryPoint.CreateDeploymentServiceComDelegate) (() => (IManagedDeploymentServiceCom) new DeploymentServiceComWrapper());
      int errorCode = DFServiceEntryPoint.RegisterDeploymentServiceCom(DFServiceEntryPoint.s_createDeploymentServiceComDelegate);
      if (errorCode < 0)
        throw Marshal.GetExceptionForHR(errorCode);
      CodeMarker_Singleton.Instance.CodeMarker(524);
      bool flag = LifetimeManager.WaitForEnd();
      if (DFServiceEntryPoint._dfsvcForm != null)
        DFServiceEntryPoint._dfsvcForm.Invoke((Delegate) new DFServiceEntryPoint.DfsvcForm.CloseFormDelegate(DFServiceEntryPoint._dfsvcForm.CloseForm), (object) true);
      int num = DFServiceEntryPoint.UnregisterDeploymentServiceCom();
      if (!flag && PlatformSpecific.OnWin9x)
        Thread.Sleep(5000);
      CodeMarker_Singleton.Instance.UninitializePerformanceDLL(63);
      Environment.Exit(0);
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate IManagedDeploymentServiceCom CreateDeploymentServiceComDelegate();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int RegisterDeploymentServiceComDelegate([MarshalAs(UnmanagedType.FunctionPtr)] DFServiceEntryPoint.CreateDeploymentServiceComDelegate createDeploymentServiceComDelegate);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int UnregisterDeploymentServiceComDelegate();

    private class DfsvcForm : Form
    {
      private Container components;
      private object _lock;
      private bool _lifetimeManagerTerminated;
      private bool _formClosed;

      public DfsvcForm()
      {
        this._lock = new object();
        this.InitializeComponent();
      }

      protected override void Dispose(bool disposing)
      {
        if (disposing && this.components != null)
          this.components.Dispose();
        base.Dispose(disposing);
      }

      private void InitializeComponent()
      {
        this.ClientSize = new Size(292, 266);
        this.ShowInTaskbar = false;
        this.WindowState = FormWindowState.Minimized;
        this.TopMost = true;
        this.Closing += new CancelEventHandler(this.DfsvcForm_Closing);
        this.Closed += new EventHandler(this.DfsvcForm_Closed);
      }

      private void DfsvcForm_Closing(object sender, CancelEventArgs e)
      {
        e.Cancel = false;
        this.TerminateLifetimeManager(true);
      }

      private void DfsvcForm_Closed(object sender, EventArgs e)
      {
        this.TerminateLifetimeManager(true);
      }

      public void SessionEndedEventHandler(object sender, SessionEndedEventArgs e)
      {
        this.TerminateLifetimeManager(false);
      }

      public void SessionEndingEventHandler(object sender, SessionEndingEventArgs e)
      {
        e.Cancel = false;
        this.TerminateLifetimeManager(false);
      }

      public void CloseForm(bool lifetimeManagerAlreadyTerminated)
      {
        if (this._formClosed)
          return;
        lock (this._lock)
        {
          if (lifetimeManagerAlreadyTerminated)
            this._lifetimeManagerTerminated = true;
          if (this._formClosed)
            return;
          this._formClosed = true;
          this.Close();
        }
      }

      private void TerminateLifetimeManager(bool formAlreadyClosed)
      {
        if (this._lifetimeManagerTerminated)
          return;
        lock (this._lock)
        {
          if (formAlreadyClosed)
            this._formClosed = true;
          if (this._lifetimeManagerTerminated)
            return;
          this._lifetimeManagerTerminated = true;
          LifetimeManager.EndImmediately();
        }
      }

      public delegate void CloseFormDelegate(bool lifetimeManagerAlreadyTerminated);
    }
  }
}
