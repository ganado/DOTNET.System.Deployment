// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.SplashPiece
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.ComponentModel;
using System.Windows.Forms;

namespace System.Deployment.Application
{
  internal class SplashPiece : FormPiece
  {
    private PictureBox pictureWait;
    private Label lblNote;
    private Timer splashTimer;
    private TableLayoutPanel overarchingTableLayoutPanel;
    private SplashInfo info;
    private const int initialDelay = 2500;
    private const int showDelay = 1000;

    public SplashPiece(UserInterfaceForm parentForm, SplashInfo info)
    {
      this.info = info;
      this.SuspendLayout();
      this.InitializeComponent();
      this.InitializeContent();
      this.ResumeLayout(false);
      parentForm.SuspendLayout();
      parentForm.Text = Resources.GetString("UI_SplashTitle");
      parentForm.MinimizeBox = false;
      parentForm.MaximizeBox = false;
      parentForm.ControlBox = true;
      parentForm.ResumeLayout(false);
      this.splashTimer = new Timer();
      this.splashTimer.Tick += new EventHandler(this.SplashTimer_Tick);
      if (info.initializedAsWait)
      {
        this.splashTimer.Interval = 2500;
        this.splashTimer.Tag = (object) null;
        this.splashTimer.Enabled = true;
      }
      else
        this.ShowSplash((Form) parentForm);
    }

    public override bool OnClosing()
    {
      bool flag = base.OnClosing();
      this.info.cancelled = true;
      this.End();
      return flag;
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (!disposing)
        return;
      this.End();
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (SplashPiece));
      this.pictureWait = new PictureBox();
      this.lblNote = new Label();
      this.overarchingTableLayoutPanel = new TableLayoutPanel();
      ((ISupportInitialize) this.pictureWait).BeginInit();
      this.overarchingTableLayoutPanel.SuspendLayout();
      this.SuspendLayout();
      componentResourceManager.ApplyResources((object) this.pictureWait, "pictureWait");
      this.pictureWait.Name = "pictureWait";
      this.pictureWait.TabStop = false;
      componentResourceManager.ApplyResources((object) this.lblNote, "lblNote");
      this.lblNote.Name = "lblNote";
      componentResourceManager.ApplyResources((object) this.overarchingTableLayoutPanel, "overarchingTableLayoutPanel");
      this.overarchingTableLayoutPanel.Controls.Add((Control) this.pictureWait, 0, 0);
      this.overarchingTableLayoutPanel.Controls.Add((Control) this.lblNote, 0, 1);
      this.overarchingTableLayoutPanel.Name = "overarchingTableLayoutPanel";
      componentResourceManager.ApplyResources((object) this, "$this");
      this.Controls.Add((Control) this.overarchingTableLayoutPanel);
      this.Name = "SplashPiece";
      ((ISupportInitialize) this.pictureWait).EndInit();
      this.overarchingTableLayoutPanel.ResumeLayout(false);
      this.overarchingTableLayoutPanel.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    private void InitializeContent()
    {
      this.pictureWait.Image = Resources.GetImage("splash.gif");
    }

    private void End()
    {
      this.info.initializedAsWait = false;
      this.splashTimer.Tag = (object) this;
      this.splashTimer.Dispose();
      this.info.pieceReady.Set();
    }

    private void ShowSplash(Form parentForm)
    {
      this.info.initializedAsWait = false;
      parentForm.Visible = true;
      this.splashTimer.Interval = 1000;
      this.splashTimer.Tag = (object) this;
      this.splashTimer.Enabled = true;
      this.info.pieceReady.Reset();
    }

    private void SplashTimer_Tick(object sender, EventArgs e)
    {
      if (!this.splashTimer.Enabled)
        return;
      this.splashTimer.Enabled = false;
      if (this.splashTimer.Tag != null)
        this.info.pieceReady.Set();
      else
        this.ShowSplash(this.FindForm());
    }
  }
}
