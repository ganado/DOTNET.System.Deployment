// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.UserInterfaceForm
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace System.Deployment.Application
{
  internal class UserInterfaceForm : Form
  {
    private FormPiece currentPiece;
    private SplashInfo splashPieceInfo;
    private ManualResetEvent onLoadEvent;

    public UserInterfaceForm(ManualResetEvent readyEvent, SplashInfo splashInfo)
    {
      this.onLoadEvent = readyEvent;
      this.splashPieceInfo = splashInfo;
      this.SuspendLayout();
      this.InitializeComponent();
      this.InitializeContent();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    public ProgressPiece ConstructProgressPiece(UserInterfaceInfo info)
    {
      return new ProgressPiece(this, info);
    }

    public UpdatePiece ConstructUpdatePiece(UserInterfaceInfo info, ManualResetEvent modalEvent)
    {
      return new UpdatePiece(this, info, modalEvent);
    }

    public ErrorPiece ConstructErrorPiece(string title, string message, string logFileLocation, string linkUrl, string linkUrlMessage, ManualResetEvent modalEvent)
    {
      return new ErrorPiece(this, title, message, logFileLocation, linkUrl, linkUrlMessage, modalEvent);
    }

    public PlatformPiece ConstructPlatformPiece(string platformDetectionErrorMsg, Uri supportUrl, ManualResetEvent modalEvent)
    {
      return new PlatformPiece(this, platformDetectionErrorMsg, supportUrl, modalEvent);
    }

    public MaintenancePiece ConstructMaintenancePiece(UserInterfaceInfo info, MaintenanceInfo maintenanceInfo, ManualResetEvent modalEvent)
    {
      return new MaintenancePiece(this, info, maintenanceInfo, modalEvent);
    }

    public void ShowSimpleMessageBox(string message, string caption)
    {
      MessageBoxOptions options = (MessageBoxOptions) 0;
      if (this.IsRightToLeft((Control) this))
        options |= MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading;
      int num = (int) MessageBox.Show((IWin32Window) this, message, caption, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1, options);
    }

    public void SwitchUserInterfacePiece(FormPiece piece)
    {
      FormPiece currentPiece = this.currentPiece;
      this.currentPiece = piece;
      this.currentPiece.Dock = DockStyle.Fill;
      this.SuspendLayout();
      this.Controls.Add((Control) this.currentPiece);
      if (currentPiece != null)
      {
        this.Controls.Remove((Control) currentPiece);
        currentPiece.Dispose();
      }
      this.ClientSize = this.currentPiece.ClientSize;
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      this.onLoadEvent.Set();
    }

    protected override void OnVisibleChanged(EventArgs e)
    {
      base.OnVisibleChanged(e);
      if (!this.Visible || Form.ActiveForm == this)
        return;
      this.Activate();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      base.OnClosing(e);
      if (!this.currentPiece.OnClosing())
      {
        e.Cancel = true;
      }
      else
      {
        e.Cancel = true;
        this.Hide();
      }
    }

    protected override void SetVisibleCore(bool value)
    {
      if (this.splashPieceInfo.initializedAsWait)
        base.SetVisibleCore(false);
      else
        base.SetVisibleCore(value);
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (!disposing)
        return;
      this.Icon.Dispose();
      this.Icon = (Icon) null;
      if (this.currentPiece == null)
        return;
      this.currentPiece.Dispose();
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserInterfaceForm));
      this.SuspendLayout();
      componentResourceManager.ApplyResources((object) this, "$this");
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ControlBox = false;
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "UserInterfaceForm";
      this.ShowIcon = false;
      this.ResumeLayout(false);
    }

    private void InitializeContent()
    {
      this.Icon = Resources.GetIcon("form.ico");
      this.Font = SystemFonts.MessageBoxFont;
      this.currentPiece = (FormPiece) new SplashPiece(this, this.splashPieceInfo);
      this.Controls.Add((Control) this.currentPiece);
    }

    private bool IsRightToLeft(Control control)
    {
      if (control.RightToLeft == RightToLeft.Yes)
        return true;
      if (control.RightToLeft == RightToLeft.No || control.RightToLeft != RightToLeft.Inherit || control.Parent == null)
        return false;
      return this.IsRightToLeft(control.Parent);
    }

    public delegate ProgressPiece ConstructProgressPieceDelegate(UserInterfaceInfo info);

    public delegate UpdatePiece ConstructUpdatePieceDelegate(UserInterfaceInfo info, ManualResetEvent modalEvent);

    public delegate ErrorPiece ConstructErrorPieceDelegate(string title, string message, string logFileLocation, string linkUrl, string linkUrlMessage, ManualResetEvent modalEvent);

    public delegate PlatformPiece ConstructPlatformPieceDelegate(string platformDetectionErrorMsg, Uri supportUrl, ManualResetEvent modalEvent);

    public delegate MaintenancePiece ConstructMaintenancePieceDelegate(UserInterfaceInfo info, MaintenanceInfo maintenanceInfo, ManualResetEvent modalEvent);

    public delegate void ShowSimpleMessageBoxDelegate(string messsage, string caption);
  }
}
