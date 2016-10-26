// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.UserInterface
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using Microsoft.Win32;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace System.Deployment.Application
{
  internal class UserInterface : IDisposable
  {
    private ManualResetEvent _appctxExitThreadFinished = new ManualResetEvent(false);
    private ManualResetEvent _uiConstructed = new ManualResetEvent(false);
    private ManualResetEvent _uiReady = new ManualResetEvent(false);
    private UserInterfaceForm _uiForm;
    private ApplicationContext _appctx;
    private Thread _uiThread;
    private SplashInfo _splashInfo;
    private bool _disposed;

    private static string DefaultBrowserExePath
    {
      get
      {
        string str1 = (string) null;
        RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey("http\\shell\\open\\command");
        if (registryKey != null)
        {
          string str2 = (string) registryKey.GetValue(string.Empty);
          if (str2 != null)
          {
            string str3 = str2.Trim();
            if (str3.Length != 0)
            {
              if ((int) str3[0] == 34)
              {
                int num = str3.IndexOf('"', 1);
                if (num != -1)
                  str1 = str3.Substring(1, num - 1);
              }
              else
              {
                int length = str3.IndexOf(' ');
                str1 = length == -1 ? str3 : str3.Substring(0, length);
              }
            }
          }
        }
        return str1;
      }
    }

    public UserInterface(bool wait)
    {
      this._splashInfo = new SplashInfo();
      this._splashInfo.initializedAsWait = wait;
      this._uiThread = new Thread(new ThreadStart(this.UIThread));
      this._uiThread.SetApartmentState(ApartmentState.STA);
      this._uiThread.Name = "UIThread";
      this._uiThread.Start();
    }

    public UserInterface()
      : this(true)
    {
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public ProgressPiece ShowProgress(UserInterfaceInfo info)
    {
      this.WaitReady();
      return (ProgressPiece) this._uiForm.Invoke((Delegate) new UserInterfaceForm.ConstructProgressPieceDelegate(this._uiForm.ConstructProgressPiece), (object) info);
    }

    public UserInterfaceModalResult ShowUpdate(UserInterfaceInfo info)
    {
      this.WaitReady();
      ManualResetEvent manualResetEvent = new ManualResetEvent(false);
      UpdatePiece updatePiece = (UpdatePiece) this._uiForm.Invoke((Delegate) new UserInterfaceForm.ConstructUpdatePieceDelegate(this._uiForm.ConstructUpdatePiece), (object) info, (object) manualResetEvent);
      manualResetEvent.WaitOne();
      return updatePiece.ModalResult;
    }

    public UserInterfaceModalResult ShowMaintenance(UserInterfaceInfo info, MaintenanceInfo maintenanceInfo)
    {
      this.WaitReady();
      ManualResetEvent manualResetEvent = new ManualResetEvent(false);
      MaintenancePiece maintenancePiece = (MaintenancePiece) this._uiForm.Invoke((Delegate) new UserInterfaceForm.ConstructMaintenancePieceDelegate(this._uiForm.ConstructMaintenancePiece), (object) info, (object) maintenanceInfo, (object) manualResetEvent);
      manualResetEvent.WaitOne();
      return maintenancePiece.ModalResult;
    }

    public void ShowMessage(string message, string caption)
    {
      this.WaitReady();
      this._uiForm.Invoke((Delegate) new UserInterfaceForm.ShowSimpleMessageBoxDelegate(this._uiForm.ShowSimpleMessageBox), (object) message, (object) caption);
    }

    public void ShowPlatform(string platformDetectionErrorMsg, Uri supportUrl)
    {
      this.WaitReady();
      ManualResetEvent manualResetEvent = new ManualResetEvent(false);
      this._uiForm.BeginInvoke((Delegate) new UserInterfaceForm.ConstructPlatformPieceDelegate(this._uiForm.ConstructPlatformPiece), (object) platformDetectionErrorMsg, (object) supportUrl, (object) manualResetEvent);
      manualResetEvent.WaitOne();
    }

    public void ShowError(string title, string message, string logFileLocation, string linkUrl, string linkUrlMessage)
    {
      this.WaitReady();
      ManualResetEvent manualResetEvent = new ManualResetEvent(false);
      this._uiForm.BeginInvoke((Delegate) new UserInterfaceForm.ConstructErrorPieceDelegate(this._uiForm.ConstructErrorPiece), (object) title, (object) message, (object) logFileLocation, (object) linkUrl, (object) linkUrlMessage, (object) manualResetEvent);
      manualResetEvent.WaitOne();
    }

    public void Hide()
    {
      this.WaitReady();
      this._uiForm.BeginInvoke((Delegate) new MethodInvoker(((Control) this._uiForm).Hide));
    }

    public void Activate()
    {
      this.WaitReady();
      this._uiForm.BeginInvoke((Delegate) new MethodInvoker(((Form) this._uiForm).Activate));
    }

    public bool SplashCancelled()
    {
      return this._splashInfo.cancelled;
    }

    private void WaitReady()
    {
      this._uiConstructed.WaitOne();
      this._uiReady.WaitOne();
      this._splashInfo.pieceReady.WaitOne();
    }

    private void UIThread()
    {
      System.Windows.Forms.Application.EnableVisualStyles();
      System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
      using (this._uiForm = new UserInterfaceForm(this._uiReady, this._splashInfo))
      {
        this._uiConstructed.Set();
        this._appctx = new ApplicationContext((Form) this._uiForm);
        System.Windows.Forms.Application.Run(this._appctx);
        this._appctxExitThreadFinished.WaitOne();
        System.Windows.Forms.Application.ExitThread();
      }
    }

    private void Dispose(bool disposing)
    {
      if (this._disposed)
        return;
      if (disposing)
      {
        this.WaitReady();
        this._appctx.ExitThread();
        this._appctxExitThreadFinished.Set();
      }
      this._disposed = true;
    }

    public static string GetDisplaySite(Uri sourceUri)
    {
      string str = (string) null;
      if (sourceUri.IsUnc)
      {
        try
        {
          str = Path.GetDirectoryName(sourceUri.LocalPath);
        }
        catch (ArgumentException ex)
        {
        }
      }
      else
      {
        str = sourceUri.Host;
        if (string.IsNullOrEmpty(str))
        {
          try
          {
            str = Path.GetDirectoryName(sourceUri.LocalPath);
          }
          catch (ArgumentException ex)
          {
          }
        }
      }
      return str;
    }

    public static string LimitDisplayTextLength(string displayText)
    {
      if (displayText.Length <= 50)
        return displayText;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(displayText, 0, 47);
      stringBuilder.Append("...");
      return stringBuilder.ToString();
    }

    public static bool IsValidHttpUrl(string url)
    {
      bool flag = false;
      if (url != null && url.Length > 0 && (url.StartsWith(Uri.UriSchemeHttp + Uri.SchemeDelimiter, StringComparison.Ordinal) || url.StartsWith(Uri.UriSchemeHttps + Uri.SchemeDelimiter, StringComparison.Ordinal)))
        flag = true;
      return flag;
    }

    public static void LaunchUrlInBrowser(string url)
    {
      try
      {
        Process.Start(UserInterface.DefaultBrowserExePath, url);
      }
      catch (Win32Exception ex)
      {
      }
    }
  }
}
